'use client'

import React from 'react'
import type { Selection} from '@giantnodes/react';
import { Input, Table, Typography } from '@giantnodes/react'
import { IconSearch } from '@tabler/icons-react'
import { usePaginationFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { exploreTableFragment$key } from '~/__generated__/exploreTableFragment.graphql'
import type { ExploreTablePaginationQuery } from '~/__generated__/ExploreTablePaginationQuery.graphql'
import ExploreTableDirectory from '~/domains/directories/explore-table-directory'
import ExploreTableFile from '~/domains/directories/explore-table-file'

const FRAGMENT = graphql`
  fragment exploreTableFragment on FileSystemDirectory
  @refetchable(queryName: "ExploreTablePaginationQuery")
  @argumentDefinitions(first: { type: "Int", defaultValue: 10 }, after: { type: "String" }) {
    entries(first: $first, after: $after) @connection(key: "ExploreTable_fileSystemDirectory_entries") {
      edges {
        node {
          __typename
          id
          ... on FileSystemDirectory {
            ...exploreTableDirectoryFragment
          }
          ... on FileSystemFile {
            ...exploreTableFileFragment
          }
        }
      }
    }
  }
`

type ExploreTableProps = {
  $key: exploreTableFragment$key
}

const ExploreTable: React.FC<ExploreTableProps> = ({ $key }) => {
  const { data } = usePaginationFragment<ExploreTablePaginationQuery, exploreTableFragment$key>(FRAGMENT, $key)
  const [keys, setKeys] = React.useState<Selection>(new Set<string>())

  React.useEffect(() => {
    console.log('keys', keys)
  }, [keys])

  return (
    <Table.Root
      aria-label="explore table"
      behavior="toggle"
      mode="multiple"
      onSelectionChange={(selection) => setKeys(selection)}
      selectedKeys={keys}
      size="sm"
    >
      <Table.Head>
        <Table.Column key="name" isRowHeader>
          <Input.Root shape="pill" size="xs">
            <Input.Addon>
              <IconSearch size={20} strokeWidth={1} />
            </Input.Addon>

            <Input.Text aria-label="search" placeholder="Search for anything..." type="text" />
          </Input.Root>
        </Table.Column>
        <Table.Column className="text-right" key="size">
          <Typography.Text variant="subtitle">scanned</Typography.Text>
        </Table.Column>
      </Table.Head>

      <Table.Body items={data.entries?.edges ?? []}>
        {(item) => (
          <Table.Row id={item.node.id}>
            <Table.Cell>
              <div className="flex flex-row items-center gap-2">
                {item.node.__typename === 'FileSystemDirectory' && <ExploreTableDirectory $key={item.node} />}

                {item.node.__typename === 'FileSystemFile' && <ExploreTableFile $key={item.node} />}
              </div>
            </Table.Cell>

            <Table.Cell>
              <Typography.Paragraph className="text-sm text-right" variant="subtitle">
                100mb
              </Typography.Paragraph>
            </Table.Cell>
          </Table.Row>
        )}
      </Table.Body>
    </Table.Root>
  )
}

export default ExploreTable
