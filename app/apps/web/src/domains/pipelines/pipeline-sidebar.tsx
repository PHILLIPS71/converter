import React from 'react'
import { Button, Navigation, Typography } from '@giantnodes/react'

import PipelineEditDialog from '~/domains/pipelines/pipeline-edit-dialog'

const PipelineSidebar = () => (
  <Navigation.Root orientation="vertical" size="md" isBordered>
    <Navigation.Segment>
      <Navigation.Title className="flex justify-between items-center">
        Pipelines
        <PipelineEditDialog>
          <Button color="brand" size="xs">
            Create
          </Button>
        </PipelineEditDialog>
      </Navigation.Title>
    </Navigation.Segment>

    <Navigation.Divider className="my-0" />

    <Navigation.Segment>
      <Navigation.Item isSelected>
        <Navigation.Link className="p-1" href={`/`}>
          <Typography.Text>Pipeline #1</Typography.Text>
        </Navigation.Link>
      </Navigation.Item>

      <Navigation.Item>
        <Navigation.Link className="p-1" href={`/`}>
          <Typography.Text>Pipeline #1</Typography.Text>
        </Navigation.Link>
      </Navigation.Item>

      <Navigation.Item>
        <Navigation.Link className="p-1" href={`/`}>
          <Typography.Text>Pipeline #1</Typography.Text>
        </Navigation.Link>
      </Navigation.Item>
    </Navigation.Segment>
  </Navigation.Root>
)

export default PipelineSidebar
