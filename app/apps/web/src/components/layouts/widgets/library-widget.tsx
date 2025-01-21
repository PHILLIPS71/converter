'use client'

import React from 'react'
import { Avatar, Select, Typography } from '@giantnodes/react'
import { IconCircleFilled, IconSelector } from '@tabler/icons-react'
import { usePaginationFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { libraryWidgetFragment_query$key } from '~/__generated__/libraryWidgetFragment_query.graphql'
import type { LibraryWidgetRefetchableQuery } from '~/__generated__/LibraryWidgetRefetchableQuery.graphql'
import { useLibrary } from '~/domains/libraries/use-library.hook'

type LibraryWidgetProps = {
  $key: libraryWidgetFragment_query$key
}

const FRAGMENT = graphql`
  fragment libraryWidgetFragment_query on Query
  @refetchable(queryName: "LibraryWidgetRefetchableQuery")
  @argumentDefinitions(
    first: { type: "Int", defaultValue: 10 }
    after: { type: "String" }
    order: { type: "[LibrarySortInput!]", defaultValue: [{ name: ASC }] }
  ) {
    libraries(first: $first, after: $after, order: $order) @connection(key: "LibraryWidget_query_libraries") {
      edges {
        node {
          name
          slug
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

const LibraryWidget: React.FC<LibraryWidgetProps> = ({ $key }) => {
  const { library, setLibrary } = useLibrary()
  const { data } = usePaginationFragment<LibraryWidgetRefetchableQuery, libraryWidgetFragment_query$key>(FRAGMENT, $key)

  const onSelect = (item: string | number | Set<string | number>) => {
    if (typeof item === 'string' || typeof item === 'number') {
      setLibrary(item.toString())
      return
    }

    setLibrary(Array.from(item).at(0)?.toString() ?? null)
  }

  return (
    <Select.Root
      aria-label="library selector"
      icon={<IconSelector size={20} strokeWidth={1} />}
      items={data.libraries?.edges}
      onSelectionChange={onSelect}
      selectedKey={library?.slug}
      selectionMode="single"
      size="sm"
    >
      {(item) => (
        <Select.Option id={item.node.slug}>
          <Avatar.Root className="flex-shrink-0" size="xs">
            <Avatar.Icon icon={<IconCircleFilled className="absolute" fill="#312e81" />} />
          </Avatar.Root>

          <Typography.Paragraph className="flex-shrink-0 font-medium" size="sm">
            {item.node.name}
          </Typography.Paragraph>

          <Typography.Text className="line-clamp-1 text-ellipsis overflow-hidden" size="xs" variant="subtitle">
            {item.node.directory.pathInfo.fullName}
          </Typography.Text>
        </Select.Option>
      )}
    </Select.Root>
  )
}

export default LibraryWidget
