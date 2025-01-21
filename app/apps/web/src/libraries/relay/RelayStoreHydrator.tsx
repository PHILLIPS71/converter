'use client'

// https://github.com/facebook/relay/issues/4599
// https://github.com/mizdra/poc-nextjs-app-router-relay
import type { OperationDescriptor, RecordMap } from 'relay-runtime/lib/store/RelayStoreTypes'
import React from 'react'
import { useRelayEnvironment } from 'react-relay'
import { RecordSource } from 'relay-runtime'

type RelayStoreHydratorProps = React.PropsWithChildren & {
  operation: {
    recordMap: RecordMap
    operationDescriptor: OperationDescriptor
  }
}

/**
 * Publishes the records to the Relay store, allowing the Client Component to take over the Relay Store state
 * from the Server Component.
 *
 * @remarks
 * This component should be used to bridge the gap between Server and Client Components when using Relay in a
 * server-side rendering context.
 *
 * @param props - The component props
 * @returns The wrapped child components
 *
 * @example
 * ```tsx
 * // In a Server Component
 * import { query } from '~/libraries/relay/server'
 *
 * const { data, ...operation } = await query(Query, { id: "cbf3d503-4af5-4502-8a36-9b6b99a9364d" });
 *
 * return <RelayStoreHydrator operation={operation} />{children}</RelayStoreHydrator>
 * ```
 */
const RelayStoreHydrator: React.FC<RelayStoreHydratorProps> = ({ children, operation }) => {
  const environment = useRelayEnvironment()
  const store = environment.getStore()

  // publish records only when the operation's records are stale or missing
  if (store.check(operation.operationDescriptor).status !== 'available') {
    // NOTE: This is a side effect outside useEffect, but it's safe because publishing the same record map to
    // the Relay Store will result in the same state.
    store.publish(new RecordSource(operation.recordMap))
  }

  // retain the records to prevent garbage collection while the component is mounted
  React.useEffect(() => {
    const disposable = store.retain(operation.operationDescriptor)
    return () => disposable.dispose()
  }, [store, operation.operationDescriptor])

  return children
}

export default RelayStoreHydrator
