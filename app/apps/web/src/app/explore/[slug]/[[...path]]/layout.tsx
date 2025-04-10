import React from 'react'

import { Layout } from '~/components/layouts'
import { ExploreProvider } from '~/domains/directories/explore'

type ExploreLayoutProps = React.PropsWithChildren

const ExploreLayout: React.FC<ExploreLayoutProps> = ({ children }) => (
  <Layout.Section size="lg">
    <ExploreProvider>{children}</ExploreProvider>
  </Layout.Section>
)

export default ExploreLayout
