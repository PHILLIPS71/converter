import React, { Suspense } from 'react'
import { Button, Navigation } from '@giantnodes/react'

import type { pipelineSidebarCollectionFragment$key } from '~/__generated__/pipelineSidebarCollectionFragment.graphql'
import PipelineEditDialog from '~/domains/pipelines/pipeline-edit-dialog'
import PipelineSidebarCollection from '~/domains/pipelines/pipeline-sidebar-collection'

type PipelineSidebarProps = {
  $key: pipelineSidebarCollectionFragment$key
}

const PipelineSidebar: React.FC<PipelineSidebarProps> = ({ $key }) => (
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

    <Suspense fallback="loading...">
      <PipelineSidebarCollection $key={$key} />
    </Suspense>
  </Navigation.Root>
)

export default PipelineSidebar
