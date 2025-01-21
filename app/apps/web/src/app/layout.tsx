import React from 'react'
import { GeistSans } from 'geist/font/sans'

import AppProviders from '~/app/provider'
import { Sidebar } from '~/components/layouts/navigation'

import '~/app/globals.css'

import { cn } from '@giantnodes/react'
import { graphql } from 'relay-runtime'

import type { layout_AppLayoutQuery } from '~/__generated__/layout_AppLayoutQuery.graphql'
import { LayoutProvider } from '~/components/layouts/use-layout'
import * as LibraryStore from '~/domains/libraries/library-store'
import { LibraryProvider } from '~/domains/libraries/use-library.hook'
import RelayStoreHydrator from '~/libraries/relay/RelayStoreHydrator'
import { query } from '~/libraries/relay/server'

const QUERY = graphql`
  query layout_AppLayoutQuery {
    ...sidebarDefaultFragment_query
  }
`

type AppLayoutProps = React.PropsWithChildren

const AppLayout: React.FC<AppLayoutProps> = async ({ children }) => {
  const [{ data, ...operation }, library] = await Promise.all([query<layout_AppLayoutQuery>(QUERY), LibraryStore.get()])

  return (
    <html lang="en">
      <head>
        <script src="https://unpkg.com/react-scan/dist/auto.global.js" async />
      </head>
      <body className={cn('min-h-screen bg-background font-sans antialiased', GeistSans.variable)}>
        <div className="min-h-screen flex flex-row w-full">
          <LibraryProvider library={library}>
            <AppProviders>
              <RelayStoreHydrator operation={operation}>
                <LayoutProvider>
                  <Sidebar.Root $key={data} />

                  {children}
                </LayoutProvider>
              </RelayStoreHydrator>
            </AppProviders>
          </LibraryProvider>
        </div>
      </body>
    </html>
  )
}

export default AppLayout
