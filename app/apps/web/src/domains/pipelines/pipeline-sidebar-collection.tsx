'use client'

import { Navigation, Typography } from '@giantnodes/react'
import { usePaginationFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { pipelineSidebarCollectionFragment$key } from '~/__generated__/pipelineSidebarCollectionFragment.graphql'
import type { PipelineSidebarCollectionPaginationQuery } from '~/__generated__/PipelineSidebarCollectionPaginationQuery.graphql'

const FRAGMENT = graphql`
  fragment pipelineSidebarCollectionFragment on Query
  @refetchable(queryName: "PipelineSidebarCollectionPaginationQuery")
  @argumentDefinitions(
    first: { type: "Int", defaultValue: 10 }
    after: { type: "String" }
    order: { type: "[PipelineSortInput!]", defaultValue: [{ name: ASC }] }
  ) {
    pipelines(first: $first, after: $after, order: $order)
      @connection(key: "PipelineSidebarCollectionSegment_query_pipelines") {
      edges {
        node {
          id
          name
        }
      }
    }
  }
`

type SidebarPipelineListProps = {
  $key: pipelineSidebarCollectionFragment$key
}

const PipelineSidebarCollection: React.FC<SidebarPipelineListProps> = ({ $key }) => {
  const { data } = usePaginationFragment<
    PipelineSidebarCollectionPaginationQuery,
    pipelineSidebarCollectionFragment$key
  >(FRAGMENT, $key)

  return (
    <Navigation.Segment>
      {data.pipelines?.edges?.map((item) => (
        <Navigation.Item key={item.node.id}>
          <Navigation.Link className="p-1" href={`/`}>
            <Typography.Text>{item.node.name}</Typography.Text>
          </Navigation.Link>
        </Navigation.Item>
      ))}
    </Navigation.Segment>
  )
}

export default PipelineSidebarCollection
