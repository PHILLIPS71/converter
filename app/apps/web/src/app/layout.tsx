import React from 'react'
import { GeistMono } from 'geist/font/mono'
import { GeistSans } from 'geist/font/sans'

import AppProviders from '~/app/provider'
import { Layout } from '~/components/layouts'
import { Navbar, Sidebar } from '~/components/layouts/navigation'

import '~/app/globals.css'

import { cn } from '@giantnodes/react'

type AppLayoutProps = React.PropsWithChildren

const AppLayout: React.FC<AppLayoutProps> = ({ children }) => (
  <html lang="en">
    <body className={cn('min-h-screen bg-background font-sans antialiased', GeistSans.variable, GeistMono.variable)}>
      <AppProviders>
        <Layout.Root navbar={<Navbar.Root />}>
          <Layout.Section sidebar={<Sidebar.Root />}>
            <Layout.Content>{children}</Layout.Content>
          </Layout.Section>
        </Layout.Root>
      </AppProviders>
    </body>
  </html>
)

export default AppLayout
