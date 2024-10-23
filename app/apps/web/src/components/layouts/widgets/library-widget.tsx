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
      size="sm"
      aria-label="library selector"
      icon={<IconSelector size={20} strokeWidth={1} />}
      items={data.libraries?.edges}
      onSelectionChange={(selected) => setId(selected.toString())}
      selectedKey={id}
    >
      {(item) => (
        <Select.Option id={item.node.id}>
          <Avatar.Root size="xs" className="flex-shrink-0">
            <Avatar.Icon icon={<IconCircleFilled fill="#312e81" className="absolute" />} />
          </Avatar.Root>

          <Typography.Paragraph size="sm" className="flex-shrink-0 font-medium">
            {item.node.name}
          </Typography.Paragraph>

          <Typography.Text variant="subtitle" size="xs" className="overflow-hidden text-ellipsis">
            {item.node.directory.pathInfo.fullName}
          </Typography.Text>
        </Select.Option>
      )}
    </Select.Root>
  )
}

export default LibraryWidget
