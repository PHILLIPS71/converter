import React from 'react'

import { Layout } from '~/components/layouts'
import { Navbar } from '~/components/layouts/navigation'

type DashboardPageLayoutProps = React.PropsWithChildren

const DashboardPageLayout: React.FC<DashboardPageLayoutProps> = ({ children }) => (
  <Layout.Root>
    <Layout.Container>
      <Navbar.Root />

      <Layout.Section>{children}</Layout.Section>
    </Layout.Container>
  </Layout.Root>
)

export default DashboardPageLayout
