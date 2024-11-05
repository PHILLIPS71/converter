'use client'

import React from 'react'

import { setLibraryId } from '~/actions/set-library'
import { create } from '~/utilities/create-context'

type UseLibraryProps = {
  id: string | null
}

type LibraryContextType = ReturnType<typeof useLibraryValue>

const useLibraryValue = (props: UseLibraryProps) => {
  const [id, setId] = React.useState<string | null>(props.id)
  const [, action] = React.useActionState(setLibraryId, null)
  const [isPending, transition] = React.useTransition()

  const set = React.useCallback(
    (value: string | null) => {
      setId(value)

      transition(() => action(value))
    },
    [action, transition]
  )

  return {
    id,
    setId: set,
    isPending,
  }
}

export const [LibraryProvider, useLibrary] = create<LibraryContextType, UseLibraryProps>(useLibraryValue, {
  name: 'LibraryContext',
  strict: true,
  errorMessage: 'useLibrary: `context` is undefined. Seems you forgot to wrap component within the <LibraryProvider />',
})
