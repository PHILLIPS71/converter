'server only'

// https://github.com/facebook/relay/issues/4599
// https://github.com/mizdra/poc-nextjs-app-router-relay
import type { CacheConfig, FetchQueryFetchPolicy, GraphQLTaggedNode, OperationType } from 'relay-runtime'
import type { OperationDescriptor, RecordMap } from 'relay-runtime/lib/store/RelayStoreTypes'
import { createOperationDescriptor, fetchQuery, getRequest } from 'relay-runtime'

import { create } from '~/libraries/relay/environment'

type RelayQueryOptions = {
  networkCacheConfig?: CacheConfig
  fetchPolicy?: FetchQueryFetchPolicy
}

type RelayQueryResult<T extends OperationType> = {
  data: T['response']
  recordMap: RecordMap
  operationDescriptor: OperationDescriptor
}

/**
 * Executes a Relay query and returns the data along with the record map and operation descriptor.
 *
 * @template TOperation - The OperationType for the query
 * @param node - The GraphQL query node
 * @param variables - The variables for the query
 * @param options - Additional options for the query
 * @returns A promise resolving to an object containing the query data, record map, and operation descriptor
 *
 * @remarks
 * Designed for server components. Fetches data and prepares information to hydrate he Relay store on the client
 * side, bridging server-side rendering with client-side Relay.
 *
 * @example
 * // In a Server Component
 * import { query } from '~/libraries/relay/server'
 *
 * const { data, ...operation } = await query(Query, { id: "cbf3d503-4af5-4502-8a36-9b6b99a9364d" });
 *
 * return <RelayStoreHydrator operation={operation}>{children}</RelayStoreHydrator>
 */
export const query = async <TOperation extends OperationType>(
  node: GraphQLTaggedNode,
  variables: TOperation['variables'] = {},
  options: RelayQueryOptions = {}
): Promise<RelayQueryResult<TOperation>> => {
  const environment = create()
  const observable = fetchQuery<TOperation>(environment, node, variables, options)

  const data = await observable.toPromise()
  if (data == null) {
    throw new Error('Query returned null or undefined data')
  }

  const store = environment.getStore()
  const recordMap = store.getSource().toJSON()

  const request = getRequest(node)
  const operationDescriptor = createOperationDescriptor(request, variables, options.networkCacheConfig)

  return { data, recordMap, operationDescriptor }
}
