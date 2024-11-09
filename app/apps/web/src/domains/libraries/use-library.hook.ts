'use client'

import React from 'react'

import { setLibrary as setLibraryAction } from '~/actions/set-library'
import { Library } from '~/domains/libraries/library-store'
import { create } from '~/utilities/create-context'
import { isSuccess } from '~/utilities/result-pattern'

type UseLibraryProps = {
  library: Library
}

type LibraryContextType = ReturnType<typeof useLibraryValue>

const useLibraryValue = (props: UseLibraryProps) => {
  const [library, setLibrary] = React.useState<Library>(props.library)
  const [state, action] = React.useActionState(setLibraryAction, null)
  const [isPending, transition] = React.useTransition()

  const set = React.useCallback(
    (value: string | null) => {
      transition(() => action(value))
    },
    [action, transition]
  )

  React.useEffect(() => {
    if (isSuccess(state)) {
      setLibrary(state.value)
    }
  }, [state])

  return {
    library,
    setLibrary: set,
    isPending,
  }
}

export const [LibraryProvider, useLibrary] = create<LibraryContextType, UseLibraryProps>(useLibraryValue, {
  name: 'LibraryContext',
  strict: true,
  errorMessage: 'useLibrary: `context` is undefined. Seems you forgot to wrap component within the <LibraryProvider />',
})
