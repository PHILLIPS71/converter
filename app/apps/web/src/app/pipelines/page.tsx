import React from 'react'
import { Card, Chip, Table, Typography } from '@giantnodes/react'
import { IconCalendar, IconCircleCheckFilled, IconCircleXFilled, IconStopwatch } from '@tabler/icons-react'

const PipelinePage = () => (
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
)

export default PipelinePage
