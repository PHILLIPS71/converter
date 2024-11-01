'use client'

import React from 'react'

import { create } from '~/utilities/create-context'

type UseLibraryProps = object

type LibraryContextType = ReturnType<typeof useLibraryValue>

const LOCAL_STORAGE_KEY = 'library:id'

const useLibraryValue = (_: UseLibraryProps) => {
  const [id, setId] = React.useState(() => {
    if (typeof window === 'undefined') return null
    return localStorage.getItem(LOCAL_STORAGE_KEY)
  })

  React.useEffect(() => {
    if (id == null) localStorage.removeItem(LOCAL_STORAGE_KEY)
    else localStorage.setItem(LOCAL_STORAGE_KEY, id)
  }, [id])

  React.useEffect(() => {
    const onStorageChange = (event: StorageEvent) => {
      if (event.key === LOCAL_STORAGE_KEY) {
        setId(event.newValue)
      }
    }

    window.addEventListener('storage', onStorageChange)
    return () => window.removeEventListener('storage', onStorageChange)
  }, [])

  return {
    id,
    setId,
  }
}

export const [LibraryProvider, useLibrary] = create<LibraryContextType, UseLibraryProps>(useLibraryValue, {
  name: 'LibraryContext',
  strict: true,
  errorMessage: 'useLibrary: `context` is undefined. Seems you forgot to wrap component within the <LibraryProvider />',
})
