'use client'

import { Chip, Table, Typography } from '@giantnodes/react'
import { IconCalendar, IconCircleCheckFilled, IconStopwatch } from '@tabler/icons-react'
import dayjs from 'dayjs'
import { usePaginationFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { pipelineExecutionTableFragment$key } from '~/__generated__/pipelineExecutionTableFragment.graphql'
import type { PipelineExecutionTablePaginationQuery } from '~/__generated__/PipelineExecutionTablePaginationQuery.graphql'
import { toPrettyDuration } from '~/libraries/dayjs'

const FRAGMENT = graphql`
  fragment pipelineExecutionTableFragment on Query
  @refetchable(queryName: "PipelineExecutionTablePaginationQuery")
  @argumentDefinitions(
    first: { type: "Int", defaultValue: 20 }
    after: { type: "String" }
    where: { type: "PipelineExecutionFilterInput" }
  ) {
    pipelineExecutions(first: $first, after: $after, where: $where)
      @connection(key: "PipelineExecutionTable_pipelineExecutions") {
      edges {
        node {
          id
          startedAt
          completedAt
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

type PipelineExecutionTableProps = {
  $key: pipelineExecutionTableFragment$key
}

const PipelineExecutionTable: React.FC<PipelineExecutionTableProps> = ({ $key }) => {
  const { data } = usePaginationFragment<PipelineExecutionTablePaginationQuery, pipelineExecutionTableFragment$key>(
    FRAGMENT,
    $key
  )

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
                <IconCircleCheckFilled className="text-brand" size={20} strokeWidth={1} />

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

                <Typography.Text className="flex gap-2" size="xs">
                  <IconStopwatch size={18} strokeWidth={1} />
                  {toPrettyDuration(dayjs.duration(dayjs(edge.node.completedAt).diff(edge.node.startedAt)))}
                </Typography.Text>
              </div>
            </Table.Cell>
          </Table.Row>
        ))}
      </Table.Body>
    </Table.Root>
  )
}

export default PipelineExecutionTable
