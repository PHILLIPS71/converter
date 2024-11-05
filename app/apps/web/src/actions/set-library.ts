'use server'

import type { Result } from '~/utilities/result-pattern'
import * as LibraryStore from '~/domains/libraries/library-store'
import { failure, success } from '~/utilities/result-pattern'

type SetLibraryIdState = unknown

type SetLibraryIdResult = Result<void, string>

export const setLibraryId = async (_: SetLibraryIdState, id: string | null): Promise<SetLibraryIdResult> => {
  try {
    await LibraryStore.set(id)

    return success()
  } catch (error) {
    console.error('an expected error occurred', error)
    return failure('an expected error occurred')
  }
}
