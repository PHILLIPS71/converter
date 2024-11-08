'use client'

import React from 'react'

import { setLibrary } from '~/actions/set-library'
import { create } from '~/utilities/create-context'

type UseLibraryProps = {
  slug: string | null
}

type LibraryContextType = ReturnType<typeof useLibraryValue>

const useLibraryValue = (props: UseLibraryProps) => {
  const [slug, setSlug] = React.useState<string | null>(props.slug)
  const [, action] = React.useActionState(setLibrary, null)
  const [isPending, transition] = React.useTransition()

  const set = React.useCallback(
    (value: string | null) => {
      setSlug(value)

      transition(() => action(value))
    },
    [action, transition]
  )

  return {
    slug,
    setSlug: set,
    isPending,
  }
}

export const [LibraryProvider, useLibrary] = create<LibraryContextType, UseLibraryProps>(useLibraryValue, {
  name: 'LibraryContext',
  strict: true,
  errorMessage: 'useLibrary: `context` is undefined. Seems you forgot to wrap component within the <LibraryProvider />',
})
