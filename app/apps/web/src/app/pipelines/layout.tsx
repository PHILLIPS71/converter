import React from 'react'
import { graphql } from 'relay-runtime'

import type { layout_pipelines_Query } from '~/__generated__/layout_pipelines_Query.graphql'
import { Layout } from '~/components/layouts'
import { PipelineSidebar } from '~/domains/pipelines/layout'
import HydrationBoundary from '~/libraries/relay/HydrationBoundary'
import { query } from '~/libraries/relay/server'

type PipelineLayoutProps = React.PropsWithChildren

const QUERY = graphql`
  query layout_pipelines_Query {
    ...sidebar_pipeline_query
  }
`

const PipelineLayout: React.FC<PipelineLayoutProps> = async ({ children }) => {
  const { data, ...operation } = await query<layout_pipelines_Query>(QUERY)

  return (
    <HydrationBoundary operation={operation}>
      <div className="flex flex-row h-full">
        <PipelineSidebar $key={data} />

        <Layout.Section size="lg">{children}</Layout.Section>
      </div>
    </HydrationBoundary>
  )
}

export default PipelineLayout
