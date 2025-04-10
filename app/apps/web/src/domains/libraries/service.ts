import React from 'react'
import { graphql } from 'relay-runtime'

import type { service_library_Query, service_library_Query$data } from '~/__generated__/service_library_Query.graphql'
import type { Result } from '~/utilities/result-pattern'
import { query } from '~/libraries/relay/server'
import { CookieAdapter } from '~/services/cookie-adapter'
import { failure, isFailure, success } from '~/utilities/result-pattern'

export type Library = NonNullable<service_library_Query$data['library']>

const COOKIE_NAME = 'library:slug'

const QUERY = graphql`
  query service_library_Query($slug: String!) {
    library(where: { slug: { eq: $slug } }) {
      id
      slug
      name
      directory {
        pathInfo {
          fullName
          directorySeparatorChar
        }
      }
    }
  }
`

export class LibraryService {
  static get = React.cache(async (): Promise<Result<Library | null, string>> => {
    const result = await CookieAdapter.get(COOKIE_NAME)
    if (isFailure(result)) {
      return result
    }

    if (!result.value?.length) {
      return success(null)
    }

    try {
      const {
        data: { library },
      } = await query<service_library_Query>(QUERY, {
        slug: result.value,
      })

      return success(library ?? null)
    } catch (error) {
      return failure(`an unexpected error occurred: ${error instanceof Error ? error.message : String(error)}`)
    }
  })

  static set = async (slug: string | null) => CookieAdapter.set(COOKIE_NAME, slug)
}
