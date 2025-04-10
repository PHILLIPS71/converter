'use client'

import React from 'react'

import type { Library } from '~/domains/libraries/service'
import { LibraryService } from '~/domains/libraries/service'
import { create } from '~/utilities/create-context'
import { isSuccess } from '~/utilities/result-pattern'

type UseLibraryProps = {
  library: Library | null
}

type LibraryContextType = ReturnType<typeof useLibraryValue>

const useLibraryValue = (props: UseLibraryProps) => {
  const [library, setLibrary] = React.useState<Library | null>(props.library)

  const set = React.useCallback(async (value: Library | null): Promise<void> => {
    const result = await LibraryService.set(value?.slug ?? null)
    if (isSuccess(result)) {
      setLibrary(value)
    }
  }, [])

  return {
    library,
    set,
  }
}

export const [LibraryProvider, useLibrary] = create<LibraryContextType, UseLibraryProps>(useLibraryValue, {
  name: 'LibraryContext',
  strict: true,
  errorMessage: 'useLibrary: `context` is undefined. Seems you forgot to wrap component within the <LibraryProvider />',
})
