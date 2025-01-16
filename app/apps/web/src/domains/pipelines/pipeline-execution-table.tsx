'use client'

import React from 'react'
import { Chip, Spinner, Table, Typography } from '@giantnodes/react'
import {
  IconCalendar,
  IconCircleCheckFilled,
  IconCircleXFilled,
  IconHelpOctagonFilled,
  IconStopwatch,
} from '@tabler/icons-react'
import dayjs from 'dayjs'
import { usePaginationFragment } from 'react-relay'
import { graphql } from 'relay-runtime'
import { useDebounce } from 'use-debounce'

import type {
  pipelineExecutionTableFragment_query$key,
  PipelineStatus,
} from '~/__generated__/pipelineExecutionTableFragment_query.graphql'
import type {
  PipelineExecutionFilterInput,
  PipelineExecutionTablePaginationQuery,
} from '~/__generated__/PipelineExecutionTablePaginationQuery.graphql'
import { usePipeline } from '~/domains/pipelines/use-pipeline-execution.hook'
import { toPrettyDuration } from '~/libraries/dayjs'

const FRAGMENT = graphql`
  fragment pipelineExecutionTableFragment_query on Query
  @refetchable(queryName: "PipelineExecutionTablePaginationQuery")
  @argumentDefinitions(
    first: { type: "Int", defaultValue: 20 }
    after: { type: "String" }
    where: { type: "PipelineExecutionFilterInput" }
    order: { type: "[PipelineExecutionSortInput!]", defaultValue: { createdAt: DESC } }
  ) {
    pipelineExecutions(first: $first, after: $after, where: $where, order: $order)
      @connection(key: "PipelineExecutionTable_pipelineExecutions") {
      edges {
        node {
          id
          status
          startedAt
          completedAt
          duration
          failure {
            failedAt
            reason
          }
          pipeline {
            name
          }
          file {
            pathInfo {
              name
              fullName
            }
          }
        }
      }
      totalCount
    }
  }
`

const PipelineStatusIcon: React.FC<{ status: PipelineStatus }> = ({ status }) => {
  switch (status) {
    case 'PENDING':
    case 'RUNNING':
      return <Spinner color="brand" />

    case 'FAILED':
      return <IconCircleXFilled className="text-danger" />

    case 'COMPLETED':
      return <IconCircleCheckFilled className="text-brand" />

    case '%future added value':
    default:
      return <IconHelpOctagonFilled className="text-warning" />
  }
}

type PipelineExecutionTableProps = {
  $key: pipelineExecutionTableFragment_query$key
}

const PipelineExecutionTable: React.FC<PipelineExecutionTableProps> = ({ $key }) => {
  const context = usePipeline()
  const [search] = useDebounce(context.search, 300)

  const { data, refetch } = usePaginationFragment<
    PipelineExecutionTablePaginationQuery,
    pipelineExecutionTableFragment_query$key
  >(FRAGMENT, $key)

  const variables = React.useMemo<PipelineExecutionFilterInput | undefined>(() => {
    const parts = []

    if (context.search) {
      parts.push({ file: { pathInfo: { name: { contains: context.search } } } })
    }

    if (context.slug) {
      parts.push({ pipeline: { slug: { eq: context.slug } } })
    }

    let input: PipelineExecutionFilterInput | undefined = undefined
    if (parts.length > 0) {
      input = { and: parts }
    }

    return input
  }, [context.search, context.slug])

  React.useEffect(() => {
    refetch({ where: variables })
  }, [search])

  return (
    <Table.Root aria-label="explore table" size="sm">
      <Table.Head>
        <Table.Column key="name" isRowHeader>
          {data.pipelineExecutions?.totalCount ?? 0} pipeline runs
        </Table.Column>
        <Table.Column />
        <Table.Column />
      </Table.Head>

      <Table.Body>
        {data.pipelineExecutions?.edges?.map((edge) => (
          <Table.Row key={edge.node.id}>
            <Table.Cell>
              <div className="flex items-center gap-2">
                <PipelineStatusIcon status={edge.node.status} />

                <div className="flex flex-col">
                  <Typography.Paragraph className="font-semibold">{edge.node.file.pathInfo.name}</Typography.Paragraph>
                  <Typography.Text size="xs" variant="subtitle">
                    {edge.node.file.pathInfo.fullName}
                  </Typography.Text>
                </div>
              </div>
            </Table.Cell>
            <Table.Cell>
              <Chip color="info" size="sm">
                {edge.node.pipeline.name}
              </Chip>
            </Table.Cell>
            <Table.Cell>
              <div className="flex flex-col gap-1">
                <Typography.Text className="flex gap-2" size="xs">
                  <IconCalendar size={18} strokeWidth={1} /> {dayjs(edge.node.startedAt).fromNow()}
                </Typography.Text>

                {edge.node.duration != null && (
                  <Typography.Text className="flex gap-2" size="xs">
                    <IconStopwatch size={18} strokeWidth={1} />
                    {toPrettyDuration(dayjs.duration(edge.node.duration))}
                  </Typography.Text>
                )}
              </div>
            </Table.Cell>
          </Table.Row>
        ))}
      </Table.Body>
    </Table.Root>
  )
}

export default PipelineExecutionTable
