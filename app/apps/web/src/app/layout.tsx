import React from 'react'
import { GeistMono } from 'geist/font/mono'
import { GeistSans } from 'geist/font/sans'

import AppProviders from '~/app/provider'

import '~/app/globals.css'

import { cn } from '@giantnodes/react'
import { graphql } from 'relay-runtime'

import type { layout_AppLayoutQuery } from '~/__generated__/layout_AppLayoutQuery.graphql'
import { Layout } from '~/components/layouts'
import { Navbar, Sidebar } from '~/components/layouts/navigation'
import { LibraryService } from '~/domains/libraries/service'
import { LibraryProvider } from '~/domains/libraries/use-library.hook'
import HydrationBoundary from '~/libraries/relay/HydrationBoundary'
import { query } from '~/libraries/relay/server'
import { isSuccess } from '~/utilities/result-pattern'

type AppLayoutProps = React.PropsWithChildren

const QUERY = graphql`
  query layout_AppLayoutQuery {
    ...sidebarFragment_query
  }
`

const AppLayout: React.FC<AppLayoutProps> = async ({ children }) => {
  const [library, { data, ...operation }] = await Promise.all([
    LibraryService.get(),
    query<layout_AppLayoutQuery>(QUERY),
  ])

  return (
    <html lang="en">
      <head>
        <script src="https://unpkg.com/react-scan/dist/auto.global.js" async />
      </head>
      <body className={cn('min-h-screen bg-background font-sans antialiased', GeistSans.variable, GeistMono.variable)}>
        <AppProviders>
          <HydrationBoundary operation={operation}>
            <LibraryProvider library={isSuccess(library) ? library.value : null}>
              <Layout.Root>
                <Sidebar.Root $key={data} />

                <Layout.Container>
                  <Navbar.Root />

                  {children}
                </Layout.Container>
              </Layout.Root>
            </LibraryProvider>
          </HydrationBoundary>
        </AppProviders>
      </body>
    </html>
  )
}

export default AppLayout
