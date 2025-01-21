import React from 'react'

import { Layout } from '~/components/layouts'
import { Navbar } from '~/components/layouts/navigation'

type AppLayoutProps = React.PropsWithChildren

const AppLayout: React.FC<AppLayoutProps> = ({ children }) => (
  <Layout.Root>
    <Layout.Container>
      <Navbar.Root />

      <Layout.Section slim>{children}</Layout.Section>
    </Layout.Container>
  </Layout.Root>
)

export default AppLayout
