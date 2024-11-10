import React from 'react'
import { GeistSans } from 'geist/font/sans'

import AppProviders from '~/app/provider'
import { Layout } from '~/components/layouts'
import { Navbar, Sidebar } from '~/components/layouts/navigation'

import '~/app/globals.css'

import { cn } from '@giantnodes/react'
import { graphql } from 'relay-runtime'

import type { layout_AppLayoutQuery } from '~/__generated__/layout_AppLayoutQuery.graphql'
import * as LibraryStore from '~/domains/libraries/library-store'
import { LibraryProvider } from '~/domains/libraries/use-library.hook'
import RelayStoreHydrator from '~/libraries/relay/RelayStoreHydrator'
import { query } from '~/libraries/relay/server'

const QUERY = graphql`
  query layout_AppLayoutQuery {
    ...sidebarDefaultFragment
  }
`

type AppLayoutProps = React.PropsWithChildren

const AppLayout: React.FC<AppLayoutProps> = async ({ children }) => {
  const [{ data, ...operation }, library] = await Promise.all([query<layout_AppLayoutQuery>(QUERY), LibraryStore.get()])

  return (
    <html lang="en">
      <body className={cn('min-h-screen bg-background font-sans antialiased', GeistSans.variable)}>
        <LibraryProvider library={library}>
          <AppProviders>
            <RelayStoreHydrator operation={operation}>
              <Layout.Root navbar={<Navbar.Root />}>
                <Layout.Section sidebar={<Sidebar.Root $key={data} />}>
                  <Layout.Content>{children}</Layout.Content>
                </Layout.Section>
              </Layout.Root>
            </RelayStoreHydrator>
          </AppProviders>
        </LibraryProvider>
      </body>
    </html>
  )
}

export default AppLayout
