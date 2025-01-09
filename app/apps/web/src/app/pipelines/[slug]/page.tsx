import React from 'react'
import { notFound } from 'next/navigation'
import { Card, Chip, Input, Table, Typography } from '@giantnodes/react'
import { IconCalendar, IconCircleCheckFilled, IconCircleXFilled, IconSearch, IconStopwatch } from '@tabler/icons-react'
import { graphql } from 'relay-runtime'

import type { page_PipelineSlugQuery } from '~/__generated__/page_PipelineSlugQuery.graphql'
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
          <Table.Root aria-label="explore table" size="sm">
            <Table.Head>
              <Table.Column key="name" isRowHeader>
                155 pipeline runs
              </Table.Column>
              <Table.Column />
              <Table.Column />
            </Table.Head>

            <Table.Body>
              <Table.Row>
                <Table.Cell>
                  <div className="flex items-center gap-2">
                    <IconCircleCheckFilled className="text-brand" size={20} strokeWidth={1} />

                    <div className="flex flex-col">
                      <Typography.Paragraph className="font-semibold">
                        1000-lb Sisters - S01E01 - Meet the Slaton Sisters.mp4
                      </Typography.Paragraph>
                      <Typography.Text size="xs" variant="subtitle">
                        Z:\media\tvshows\1000-lb Sisters\Season 1
                      </Typography.Text>
                    </div>
                  </div>
                </Table.Cell>
                <Table.Cell>
                  <Chip color="info" size="sm">
                    pipeline #1
                  </Chip>
                </Table.Cell>
                <Table.Cell>
                  <div className="flex flex-col gap-1">
                    <Typography.Text className="flex gap-2" size="xs">
                      <IconCalendar size={18} strokeWidth={1} /> 3 weeks ago
                    </Typography.Text>

                    <Typography.Text className="flex gap-2" size="xs">
                      <IconStopwatch size={18} strokeWidth={1} /> 22 minutes
                    </Typography.Text>
                  </div>
                </Table.Cell>
              </Table.Row>

              <Table.Row>
                <Table.Cell>
                  <div className="flex items-center gap-2">
                    <IconCircleXFilled className="text-danger" size={20} />

                    <div className="flex flex-col">
                      <Typography.Paragraph className="font-semibold">
                        1000-lb Sisters - S01E01 - Meet the Slaton Sisters.mp4
                      </Typography.Paragraph>
                      <Typography.Text size="xs" variant="subtitle">
                        Z:\media\tvshows\1000-lb Sisters\Season 1
                      </Typography.Text>
                    </div>
                  </div>
                </Table.Cell>
                <Table.Cell>
                  <Chip color="info" size="sm">
                    pipeline #1
                  </Chip>
                </Table.Cell>
                <Table.Cell>
                  <div className="flex flex-col gap-1">
                    <Typography.Text className="flex gap-2" size="xs">
                      <IconCalendar size={18} strokeWidth={1} /> 3 weeks ago
                    </Typography.Text>

                    <Typography.Text className="flex gap-2" size="xs">
                      <IconStopwatch size={18} strokeWidth={1} /> 22 minutes
                    </Typography.Text>
                  </div>
                </Table.Cell>
              </Table.Row>
            </Table.Body>
          </Table.Root>
        </Card.Root>
      </div>
    </RelayStoreHydrator>
  )
}

export default PipelineSlugPage
