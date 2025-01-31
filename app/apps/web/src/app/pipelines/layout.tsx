import React from 'react'
import { graphql } from 'relay-runtime'

import type { layout_PipelineLayoutQuery } from '~/__generated__/layout_PipelineLayoutQuery.graphql'
import { Layout } from '~/components/layouts'
import PipelineSidebar from '~/domains/pipelines/pipeline-sidebar'
import RelayStoreHydrator from '~/libraries/relay/RelayStoreHydrator'
import { query } from '~/libraries/relay/server'

type PipelineLayoutProps = React.PropsWithChildren

const QUERY = graphql`
  query layout_PipelineLayoutQuery {
    ...pipelineSidebarFragment_query
  }
`

const PipelineLayout: React.FC<PipelineLayoutProps> = async ({ children }) => {
  const { data, ...operation } = await query<layout_PipelineLayoutQuery>(QUERY)

  return (
    <RelayStoreHydrator operation={operation}>
      <PipelineSidebar $key={data} />

      <Layout.Root>
        <Layout.Container>
          <Layout.Section size="lg">{children}</Layout.Section>
        </Layout.Container>
      </Layout.Root>
    </RelayStoreHydrator>
  )
}

export default PipelineLayout
