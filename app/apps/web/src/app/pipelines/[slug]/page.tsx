import React from 'react'
import { notFound } from 'next/navigation'
import { Card, Input, Typography } from '@giantnodes/react'
import { IconSearch } from '@tabler/icons-react'
import { graphql } from 'relay-runtime'

import type { page_PipelineSlugQuery } from '~/__generated__/page_PipelineSlugQuery.graphql'
import PipelineExecutionTable from '~/domains/pipelines/pipeline-execution-table'
import PipelineMenu from '~/domains/pipelines/pipeline-menu'
import RelayStoreHydrator from '~/libraries/relay/RelayStoreHydrator'
import { query } from '~/libraries/relay/server'

const QUERY = graphql`
  query page_PipelineSlugQuery($slug: String!) {
    pipeline(where: { slug: { eq: $slug } }) {
      id
      name
      description
      ...pipelineMenuFragment
    }
    ...pipelineExecutionTableFragment @arguments(where: { pipeline: { slug: { eq: $slug } } })
  }
`

type PipelineSlugPageProps = {
  params: Promise<{
    slug: string
  }>
}

const PipelineSlugPage: React.FC<PipelineSlugPageProps> = async ({ params }) => {
  const { slug } = await params
  const { data, ...operation } = await query<page_PipelineSlugQuery>(QUERY, { slug })

  if (data.pipeline == null) {
    console.warn(`the pipeline with slug '${slug}' was not found`)
    return notFound()
  }

  return (
    <RelayStoreHydrator operation={operation}>
      <div className="flex flex-col gap-6">
        <div className="flex flex-row justify-between items-start gap-6">
          <div className="flex flex-col flex-1 min-w-0 w-0">
            <Typography.HeadingLevel>
              <Typography.Heading className="truncate" level={6}>
                {data.pipeline.name}
              </Typography.Heading>
              <Typography.Paragraph className="truncate" size="sm" variant="subtitle">
                {data.pipeline.description}
              </Typography.Paragraph>
            </Typography.HeadingLevel>
          </div>

          <div className="flex flex-row gap-2">
            <Input.Root className="w-80" size="xs">
              <Input.Addon>
                <IconSearch size={18} strokeWidth={1} />
              </Input.Addon>
              <Input.Text aria-label="search" placeholder="Search pipeline runs" type="text" />
            </Input.Root>

            <PipelineMenu $key={data.pipeline} />
          </div>
        </div>

        <Card.Root>
          <PipelineExecutionTable $key={data} />
        </Card.Root>
      </div>
    </RelayStoreHydrator>
  )
}

export default PipelineSlugPage
