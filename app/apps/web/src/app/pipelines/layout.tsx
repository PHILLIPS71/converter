import React from 'react'
import { graphql } from 'relay-runtime'

import type { layout_PipelineLayoutQuery } from '~/__generated__/layout_PipelineLayoutQuery.graphql'
import PipelineSidebar from '~/domains/pipelines/pipeline-sidebar'
import RelayStoreHydrator from '~/libraries/relay/RelayStoreHydrator'
import { query } from '~/libraries/relay/server'

type PipelineLayoutProps = React.PropsWithChildren

const QUERY = graphql`
  query layout_PipelineLayoutQuery {
    ...pipelineSidebarCollectionFragment
  }
`

const PipelineLayout: React.FC<PipelineLayoutProps> = async ({ children }) => {
  const { data, ...operation } = await query<layout_PipelineLayoutQuery>(QUERY)

  return (
    <RelayStoreHydrator operation={operation}>
      <div className="flex flex-row flex-grow overflow-x-hidden">
        <PipelineSidebar $key={data} />

        <div className="max-w-6xl mx-auto w-full py-4 px-5 overflow-y-auto">{children}</div>
      </div>
    </RelayStoreHydrator>
  )
}

export default PipelineLayout
