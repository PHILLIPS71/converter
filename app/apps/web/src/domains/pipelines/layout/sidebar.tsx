'use client'

import React from 'react'
import { usePathname } from 'next/navigation'
import { Button, Navigation, Typography } from '@giantnodes/react'
import { useFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { sidebar_pipeline_query$key } from '~/__generated__/sidebar_pipeline_query.graphql'
import { PipelineEditDialog } from '~/domains/pipelines/construct'

const FRAGMENT = graphql`
  fragment sidebar_pipeline_query on Query
  @refetchable(queryName: "PipelineSidebarCollectionRefetchableQuery")
  @argumentDefinitions(
    first: { type: "Int", defaultValue: 10 }
    after: { type: "String" }
    order: { type: "[PipelineSortInput!]", defaultValue: [{ name: ASC }] }
  ) {
    pipelines(first: $first, after: $after, order: $order) @connection(key: "PipelineSidebarCollection_pipelines") {
      edges {
        node {
          id
          name
          slug
        }
      }
    }
  }
`

type PipelineSidebarProps = {
  $key: sidebar_pipeline_query$key
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

      <Navigation.Segment>
        {data.pipelines?.edges?.map((item) => (
          <Navigation.Item isSelected={route === item.node.slug} key={item.node.id} title={item.node.name}>
            <Navigation.Link className="p-1 min-w-0" href={`/pipelines/${item.node.slug}`}>
              <Typography.Text className="truncate">{item.node.name}</Typography.Text>
            </Navigation.Link>
          </Navigation.Item>
        ))}
      </Navigation.Segment>
    </Navigation.Root>
  )
}

export default PipelineSidebar
