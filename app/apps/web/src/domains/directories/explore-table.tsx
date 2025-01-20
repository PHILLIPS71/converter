'use client'

import React from 'react'
import { Card, Input, Spinner, Table, Typography } from '@giantnodes/react'
import { IconSearch } from '@tabler/icons-react'
import { filesize } from 'filesize'
import { usePaginationFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { exploreTableFragment$key } from '~/__generated__/exploreTableFragment.graphql'
import type { ExploreTablePaginationQuery } from '~/__generated__/ExploreTablePaginationQuery.graphql'
import ExploreTableDirectory from '~/domains/directories/explore-table-directory'
import ExploreTableFile from '~/domains/directories/explore-table-file'
import { useExplore } from '~/domains/directories/use-explore.hook'
import { useInfiniteScroll } from '~/hooks/use-infinite-scroll'

const FRAGMENT = graphql`
  fragment exploreTableFragment on FileSystemDirectory
  @refetchable(queryName: "ExploreTablePaginationQuery")
  @argumentDefinitions(first: { type: "Int", defaultValue: 25 }, after: { type: "String" }) {
    entries(first: $first, after: $after) @connection(key: "ExploreTable_fileSystemDirectory_entries") {
      edges {
        node {
          __typename
          id
          size
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
  const { keys, setKeys } = useExplore()

  const [, setSearch] = React.useState<string>('')

  const { data, loadNext, hasNext, isLoadingNext } = usePaginationFragment<
    ExploreTablePaginationQuery,
    exploreTableFragment$key
  >(FRAGMENT, $key)

  // const fetch = useDebouncedCallback((value: string) => {
  //   refetch({ search: value })
  // }, 300)
  //
  // const onSearchChange = (value: string) => {
  //   setSearch(value)
  //   fetch(value)
  // }

  const [loader] = useInfiniteScroll({
    onScroll: () => {
      if (hasNext) loadNext(25)
    },
  })

  return (
    <>
      <Card.Root>
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

                <Input.Text
                  aria-label="search"
                  onChange={(e) => setSearch(e.target.value)}
                  placeholder="Search for anything..."
                  type="text"
                />
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
                    {filesize(item.node.size, { base: 2 })}
                  </Typography.Paragraph>
                </Table.Cell>
              </Table.Row>
            )}
          </Table.Body>
        </Table.Root>
      </Card.Root>

      {hasNext && (
        <div className="flex flex-row flex-grow justify-center py-2 text-brand" ref={loader}>
          {isLoadingNext && <Spinner />}
        </div>
      )}
    </>
  )
}

export default ExploreTable
