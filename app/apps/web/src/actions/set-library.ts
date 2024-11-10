'use server'

import type { Library } from '~/domains/libraries/library-store'
import type { Result } from '~/utilities/result-pattern'
import * as LibraryStore from '~/domains/libraries/library-store'
import { failure, success } from '~/utilities/result-pattern'

export const setLibrary = async (_: unknown, slug: string | null): Promise<Result<Library, string>> => {
  try {
    await LibraryStore.set(slug)

    const library = await LibraryStore.get()
    return success(library)
  } catch (error) {
    console.error('an expected error occurred', error)
    return failure('an expected error occurred')
  }
}
