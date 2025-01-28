'use client'

import React from 'react'
import { usePathname, useRouter } from 'next/navigation'

import type { Library } from '~/domains/libraries/library-store'
import { setLibrary as setLibraryAction } from '~/actions/set-library'
import { create } from '~/utilities/create-context'
import { isSuccess } from '~/utilities/result-pattern'

type UseLibraryProps = {
  library: Library | null
}

type LibraryContextType = ReturnType<typeof useLibraryValue>

const useLibraryValue = (props: UseLibraryProps) => {
  const router = useRouter()
  const pathname = usePathname()

  const [library, setLibrary] = React.useState<Library | null>(props.library)

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

      // correct the url pathname to point to the updated library slug
      if (pathname.startsWith('/explore')) {
        router.replace(state.value ? `/explore/${state.value.slug}` : '/', { scroll: false })
      }
    }
  }, [pathname, router, state])

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
