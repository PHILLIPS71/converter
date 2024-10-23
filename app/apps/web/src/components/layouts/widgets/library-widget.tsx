'use client'

import React from 'react'
import { Avatar, Select, Typography } from '@giantnodes/react'
import { IconCircleFilled, IconSelector } from '@tabler/icons-react'
import { usePaginationFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { libraryWidgetFragment$key } from '~/__generated__/libraryWidgetFragment.graphql'
import type { LibraryWidgetPaginationQuery } from '~/__generated__/LibraryWidgetPaginationQuery.graphql'
import { useLibrary } from '~/domains/libraries/use-library.hook'

const FRAGMENT = graphql`
  fragment libraryWidgetFragment on Query
  @refetchable(queryName: "LibraryWidgetPaginationQuery")
  @argumentDefinitions(
    first: { type: "Int", defaultValue: 10 }
    after: { type: "String" }
    order: { type: "[LibrarySortInput!]", defaultValue: [{ name: ASC }] }
  ) {
    libraries(first: $first, after: $after, order: $order) @connection(key: "LibraryWidget_query_libraries") {
      edges {
        node {
          id
          name
          directory {
            pathInfo {
              fullName
            }
          }
        }
      }
    }
  }
`

type LibraryWidgetProps = {
  $key: libraryWidgetFragment$key
}

const LibraryWidget: React.FC<LibraryWidgetProps> = ({ $key }) => {
  const { id, setId } = useLibrary()
  const { data } = usePaginationFragment<LibraryWidgetPaginationQuery, libraryWidgetFragment$key>(FRAGMENT, $key)

  return (
    <Select.Root
      aria-label="library selector"
      icon={<IconSelector size={20} strokeWidth={1} />}
      items={data.libraries?.edges}
      onSelectionChange={(selected) => setId(selected.toString())}
      selectedKey={id}
      size="sm"
    >
      {(item) => (
        <Select.Option id={item.node.id}>
          <Avatar.Root className="flex-shrink-0" size="xs">
            <Avatar.Icon icon={<IconCircleFilled className="absolute" fill="#312e81" />} />
          </Avatar.Root>

          <Typography.Paragraph className="flex-shrink-0 font-medium" size="sm">
            {item.node.name}
          </Typography.Paragraph>

          <Typography.Text className="overflow-hidden text-ellipsis" size="xs" variant="subtitle">
            {item.node.directory.pathInfo.fullName}
          </Typography.Text>
        </Select.Option>
      )}
    </Select.Root>
  )
}

export default LibraryWidget
