import type { CacheConfig, GraphQLResponse, RequestParameters, SubscribeFunction, Variables } from 'relay-runtime'
import { createClient } from 'graphql-ws'
import { Network, Observable, QueryResponseCache } from 'relay-runtime'

import { env } from '~/env'

const IS_SERVER = typeof window === typeof undefined

const API_ENDPOINT = `${env.NEXT_PUBLIC_API_URI}/graphql`
const CACHE_TTL = 5 * 1000

const websocket = createClient({
  url: API_ENDPOINT,
})

export const cache: QueryResponseCache | null = IS_SERVER ? null : new QueryResponseCache({ size: 100, ttl: CACHE_TTL })

export const execute = async (
  parameters: RequestParameters,
  variables: Variables,
  headers?: HeadersInit
): Promise<GraphQLResponse> => {
  const response = await fetch(API_ENDPOINT, {
    method: 'POST',
    cache: 'no-store',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
      ...headers,
    },
    body: JSON.stringify({
      query: parameters.text,
      variables,
    }),
  })

  const json = (await response.json()) as GraphQLResponse
  console.log(JSON.stringify(json))
  return json
}

export const subscribe: SubscribeFunction = (
  parameters: RequestParameters,
  variables?: Record<string, unknown> | null
) => {
  const query = parameters.text

  if (!query) throw new Error('parameters.text is required but was not provided')

  return Observable.create((sink) =>
    websocket.subscribe(
      {
        operationName: parameters.name,
        query,
        variables,
      },
      sink
    )
  )
}

export const create = (headers?: HeadersInit) => {
  const fetch = async (parameters: RequestParameters, variables: Variables, config: CacheConfig) => {
    const { force } = config

    const isQuery = parameters.operationKind === 'query'
    const key = parameters.id ?? parameters.cacheID

    if (cache != null && isQuery && !force) {
      const cached = cache.get(key, variables)

      if (cached != null) {
        return Promise.resolve(cached)
      }
    }

    return execute(parameters, variables, headers)
  }

  return Network.create(fetch, subscribe)
}
