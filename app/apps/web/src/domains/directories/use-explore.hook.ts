'use client'

import type { Selection } from '@giantnodes/react'
import React from 'react'

import { create } from '~/utilities/create-context'

type ExploreContextType = ReturnType<typeof useExploreValue>

const useExploreValue = () => {
  const [keys, setKeys] = React.useState<Selection>(new Set<string>())

  return {
    keys,
    setKeys,
  }
}

export const [ExploreProvider, useExplore] = create<ExploreContextType>(useExploreValue, {
  name: 'ExploreContext',
  strict: true,
  errorMessage: 'useExplore: `context` is undefined. Seems you forgot to wrap component within <ExploreProvider />',
})
