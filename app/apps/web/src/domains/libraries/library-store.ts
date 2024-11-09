'server only'

import { ResponseCookie } from 'next/dist/compiled/@edge-runtime/cookies'
import { cookies } from 'next/headers'
import { graphql } from 'relay-runtime'

import { libraryStoreQuery } from '~/__generated__/libraryStoreQuery.graphql'
import { query } from '~/libraries/relay/server'
import { Result, success } from '~/utilities/result-pattern'

export type Library = libraryStoreQuery['response']['library']

const STORAGE_KEY = 'library:slug'

const QUERY = graphql`
  query libraryStoreQuery($slug: String!) {
    library(where: { slug: { eq: $slug } }) {
      id
      name
      slug
      directory {
        pathInfo {
          fullName
        }
      }
    }
  }
`

/**
 * Retrieves the current library slug from cookies
 *
 * @returns Promise that resolves to the library slug if found, null otherwise
 */
export const get = async (): Promise<Library | null> => {
  const store = await cookies()

  const slug = store.get(STORAGE_KEY)?.value
  if (slug == null) {
    return null
  }

  const { data } = await query<libraryStoreQuery>(QUERY, { slug })
  if (data.library == null) {
    store.delete(STORAGE_KEY)
    return null
  }

  return data.library
}

/**
 * Stores a library slug in cookies or removes it if null is provided
 *
 * @param slug - The library slug to store, or null to clear the selection
 */
export const set = async (slug: string | null): Promise<Result<ResponseCookie | null, never>> => {
  const store = await cookies()

  if (slug != null) {
    store.set(STORAGE_KEY, slug)
  } else {
    store.delete(STORAGE_KEY)
  }

  return success(store.get(STORAGE_KEY) ?? null)
}
