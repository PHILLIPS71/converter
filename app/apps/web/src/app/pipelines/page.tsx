import React from 'react'
import { Card } from '@giantnodes/react'
import { graphql } from 'relay-runtime'

import type { page_PipelineQuery } from '~/__generated__/page_PipelineQuery.graphql'
import PipelineExecutionTable from '~/domains/pipelines/pipeline-execution-table'
import RelayStoreHydrator from '~/libraries/relay/RelayStoreHydrator'
import { query } from '~/libraries/relay/server'

const QUERY = graphql`
  query page_PipelineQuery {
    ...pipelineExecutionTableFragment
  }
`

const PipelinePage: React.FC = async () => {
  const { data, ...operation } = await query<page_PipelineQuery>(QUERY)

  return (
    <RelayStoreHydrator operation={operation}>
      <Card.Root>
        <PipelineExecutionTable $key={data} />
      </Card.Root>
    </RelayStoreHydrator>
  )
}

export default PipelinePage
