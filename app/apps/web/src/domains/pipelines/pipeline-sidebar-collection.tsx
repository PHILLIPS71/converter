'use client'

import { usePathname } from 'next/navigation'
import { Navigation, Typography } from '@giantnodes/react'
import { usePaginationFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { pipelineSidebarCollectionFragment_query$key } from '~/__generated__/pipelineSidebarCollectionFragment_query.graphql'
import type { PipelineSidebarCollectionRefetchableQuery } from '~/__generated__/PipelineSidebarCollectionRefetchableQuery.graphql'

const FRAGMENT = graphql`
  fragment pipelineSidebarCollectionFragment_query on Query
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

type SidebarPipelineListProps = {
  $key: pipelineSidebarCollectionFragment_query$key
}

const PipelineSidebarCollection: React.FC<SidebarPipelineListProps> = ({ $key }) => {
  const router = usePathname()
  const route = router.split('/')[2]

  const { data } = usePaginationFragment<
    PipelineSidebarCollectionRefetchableQuery,
    pipelineSidebarCollectionFragment_query$key
  >(FRAGMENT, $key)

  return (
    <Navigation.Segment>
      {data.pipelines?.edges?.map((item) => (
        <Navigation.Item isSelected={route === item.node.slug} key={item.node.id} title={item.node.name}>
          <Navigation.Link className="p-1 min-w-0" href={`/pipelines/${item.node.slug}`}>
            <Typography.Text className="truncate">{item.node.name}</Typography.Text>
          </Navigation.Link>
        </Navigation.Item>
      ))}
    </Navigation.Segment>
  )
}

export default PipelineSidebarCollection
