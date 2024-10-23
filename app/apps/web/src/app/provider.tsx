'use client'

import React from 'react'
import { useRouter } from 'next/navigation'
import { DesignSystemProvider } from '@giantnodes/react'
import { RelayEnvironmentProvider } from 'react-relay'

import { LibraryProvider } from '~/domains/libraries/use-library.hook'
import { environment } from '~/libraries/relay/environment'

type AppProviderProps = React.PropsWithChildren

const AppProvider: React.FC<AppProviderProps> = ({ children }) => {
  const router = useRouter()

  return (
    <RelayEnvironmentProvider environment={environment}>
      <DesignSystemProvider attribute="class" defaultTheme="dark" navigate={router.push} enableSystem>
        <LibraryProvider>{children}</LibraryProvider>
      </DesignSystemProvider>
    </RelayEnvironmentProvider>
  )
}

export default AppProvider