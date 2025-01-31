import React from 'react'

import { Layout } from '~/components/layouts'
import { Navbar } from '~/components/layouts/navigation'

type ExplorePageLayoutProps = React.PropsWithChildren

const ExplorePageLayout: React.FC<ExplorePageLayoutProps> = ({ children }) => (
  <Layout.Root>
    <Layout.Container>
      <Navbar.Root />

      <Layout.Section size="lg">{children}</Layout.Section>
    </Layout.Container>
  </Layout.Root>
)

export default ExplorePageLayout
