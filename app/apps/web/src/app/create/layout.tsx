import React from 'react'

import { Layout } from '~/components/layouts'
import { Navbar } from '~/components/layouts/navigation'

type CreatePageLayoutProps = React.PropsWithChildren

const CreatePageLayout: React.FC<CreatePageLayoutProps> = ({ children }) => (
  <Layout.Root>
    <Layout.Container>
      <Navbar.Root />

      <Layout.Section slim>{children}</Layout.Section>
    </Layout.Container>
  </Layout.Root>
)

export default CreatePageLayout
