'use server'

import type { Result } from '~/utilities/result-pattern'
import * as LibraryStore from '~/domains/libraries/library-store'
import { failure, success } from '~/utilities/result-pattern'

type SetLibraryState = unknown

type SetLibraryResult = Result<void, string>

export const setLibrary = async (_: SetLibraryState, slug: string | null): Promise<SetLibraryResult> => {
  try {
    await LibraryStore.set(slug)

    return success()
  } catch (error) {
    console.error('an expected error occurred', error)
    return failure('an expected error occurred')
  }
}
