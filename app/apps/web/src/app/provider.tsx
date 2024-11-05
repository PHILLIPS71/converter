'use client'

import React from 'react'
import { useRouter } from 'next/navigation'
import { DesignSystemProvider } from '@giantnodes/react'
import { RelayEnvironmentProvider } from 'react-relay'

import { LibraryProvider } from '~/domains/libraries/use-library.hook'
import { environment } from '~/libraries/relay/environment'

type AppProviderProps = React.PropsWithChildren & {
  library: string | null
}

const AppProvider: React.FC<AppProviderProps> = ({ children, library }) => {
  const router = useRouter()

  return (
    <LibraryProvider id={library}>
      <RelayEnvironmentProvider environment={environment}>
        <DesignSystemProvider attribute="class" defaultTheme="dark" navigate={router.push} enableSystem>
          {children}
        </DesignSystemProvider>
      </RelayEnvironmentProvider>
    </LibraryProvider>
  )
}

export default AppProvider
