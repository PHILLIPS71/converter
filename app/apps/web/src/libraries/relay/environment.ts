import { Environment, RecordSource, Store } from 'relay-runtime'

import * as network from '~/libraries/relay/network'

const IS_SERVER = typeof window === typeof undefined

export const create = (headers?: HeadersInit) =>
  new Environment({
    network: network.create(headers),
    store: new Store(RecordSource.create()),
    isServer: IS_SERVER,
  })

export const environment = create()
