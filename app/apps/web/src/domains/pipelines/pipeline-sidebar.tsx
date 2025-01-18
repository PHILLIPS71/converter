'use client'

import React, { Suspense } from 'react'
import { usePathname } from 'next/navigation'
import { Button, Navigation, Typography } from '@giantnodes/react'
import { useFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { pipelineSidebarFragment_query$key } from '~/__generated__/pipelineSidebarFragment_query.graphql'
import PipelineEditDialog from '~/domains/pipelines/pipeline-edit-dialog'
import PipelineSidebarCollection from '~/domains/pipelines/pipeline-sidebar-collection'

const FRAGMENT = graphql`
  fragment pipelineSidebarFragment_query on Query {
    ...pipelineSidebarCollectionFragment_query
  }
`

type PipelineSidebarProps = {
  $key: pipelineSidebarFragment_query$key
}

const PipelineSidebar: React.FC<PipelineSidebarProps> = ({ $key }) => {
  const data = useFragment(FRAGMENT, $key)

  const router = usePathname()
  const route = router.split('/')[1]

  return (
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

        <Navigation.Item isSelected={route === 'pipelines' && router.split('/').length === 2}>
          <Navigation.Link className="p-1 min-w-0" href="/pipelines">
            <Typography.Text>All Pipelines</Typography.Text>
          </Navigation.Link>
        </Navigation.Item>
      </Navigation.Segment>

      <Navigation.Divider className="my-0" />

      <Suspense fallback="loading...">
        <PipelineSidebarCollection $key={data} />
      </Suspense>
    </Navigation.Root>
  )
}

export default PipelineSidebar
