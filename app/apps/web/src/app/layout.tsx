import React from 'react'
import { GeistMono } from 'geist/font/mono'
import { GeistSans } from 'geist/font/sans'

import AppProviders from '~/app/provider'
import { Layout } from '~/components/layouts'
import { Navbar, Sidebar } from '~/components/layouts/navigation'

import '~/app/globals.css'

import { cn } from '@giantnodes/react'
import { graphql } from 'relay-runtime'

import type { layout_AppLayoutQuery } from '~/__generated__/layout_AppLayoutQuery.graphql'
import RelayStoreHydrator from '~/libraries/relay/RelayStoreHydrator'
import { query } from '~/libraries/relay/server'

const QUERY = graphql`
  query layout_AppLayoutQuery {
    ...sidebarDefaultFragment
  }
`

type AppLayoutProps = React.PropsWithChildren

const AppLayout: React.FC<AppLayoutProps> = async ({ children }) => {
  const { data, ...operation } = await query<layout_AppLayoutQuery>(QUERY)

  return (
    <html lang="en">
      <body className={cn('min-h-screen bg-background font-sans antialiased', GeistSans.variable, GeistMono.variable)}>
        <AppProviders>
          <RelayStoreHydrator operation={operation}>
            <Layout.Root navbar={<Navbar.Root />}>
              <Layout.Section sidebar={<Sidebar.Root $key={data} />}>
                <Layout.Content>{children}</Layout.Content>
              </Layout.Section>
            </Layout.Root>
          </RelayStoreHydrator>
        </AppProviders>
      </body>
    </html>
  )
}
export default AppLayout
