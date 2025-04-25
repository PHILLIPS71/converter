'use client'

import React from 'react'
import { usePathname } from 'next/navigation'
import { Chip, Input, Link, Spinner, Table, Typography } from '@giantnodes/react'
import { IconFile, IconFolderFilled, IconSearch } from '@tabler/icons-react'
import dayjs from 'dayjs'
import { filesize } from 'filesize'
import { usePaginationFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { ExploreTablePaginationQuery } from '~/__generated__/ExploreTablePaginationQuery.graphql'
import type { table_directory$key } from '~/__generated__/table_directory.graphql'
import { useExplore } from '~/domains/directories/explore'
import { useInfiniteScroll } from '~/hooks/use-infinite-scroll'
import { toPrettyDuration } from '~/libraries/dayjs'

const FRAGMENT = graphql`
  fragment table_directory on FileSystemDirectory
  @refetchable(queryName: "ExploreTablePaginationQuery")
  @argumentDefinitions(first: { type: "Int", defaultValue: 25 }, after: { type: "String" }) {
    scannedAt
    entries(first: $first, after: $after) @connection(key: "ExploreTable_fileSystemDirectory_entries") {
      edges {
        node {
          __typename
          id
          size
          pathInfo {
            name
          }
          ... on FileSystemFile {
            videoStreams {
              index
              codec
              duration
              quality {
                width
                height
                resolution {
                  abbreviation
                }
              }
            }
          }
        }
      }
    }
  }
`

type ExploreTableProps = {
  $key: table_directory$key
}

const ExploreTable: React.FC<ExploreTableProps> = ({ $key }) => {
  const pathname = usePathname()
  const { keys, setKeys } = useExplore()

  const [, setSearch] = React.useState<string>('')

  const { data, loadNext, hasNext, isLoadingNext } = usePaginationFragment<
    ExploreTablePaginationQuery,
    table_directory$key
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
            <Input.Root className="w-full xl:max-w-80" size="xs">
              <Input.Addon>
                <IconSearch size={18} strokeWidth={1} />
              </Input.Addon>

              <Input.Text
                aria-label="search"
                onChange={(e) => setSearch(e.target.value)}
                placeholder="Search for anything..."
                type="text"
              />
            </Input.Root>
          </Table.Column>
          <Table.Column className="font-normal text-right" key="size">
            <Typography.Text size="xs" variant="subtitle">
              {dayjs(data.scannedAt).fromNow()}
            </Typography.Text>
          </Table.Column>
        </Table.Head>

        <Table.Body items={data.entries?.edges ?? []}>
          {(item) => (
            <Table.Row id={item.node.id}>
              <Table.Cell className="w-full max-w-0">
                <div className="flex flex-row items-center gap-2">
                  {item.node.__typename === 'FileSystemDirectory' && (
                    <>
                      <IconFolderFilled size={20} />

                      <Link href={`${pathname}/${item.node.pathInfo.name}`} underline="hover">
                        {item.node.pathInfo.name}
                      </Link>
                    </>
                  )}

                  {item.node.__typename === 'FileSystemFile' && (
                    <>
                      <IconFile className="shrink-0" size={20} />

                      <Typography.Paragraph className="truncate" title={item.node.pathInfo.name}>
                        {item.node.pathInfo.name}
                      </Typography.Paragraph>

                      {item.node.videoStreams?.map((stream) => (
                        <React.Fragment key={stream.index}>
                          <Chip className="whitespace-nowrap" color="cyan" size="sm">
                            {stream.codec}
                          </Chip>
                          <Chip
                            className="whitespace-nowrap"
                            color="emerald"
                            size="sm"
                            title={`${stream.quality.width}x${stream.quality.height}`}
                          >
                            {stream.quality.resolution.abbreviation}
                          </Chip>
                          <Chip className="whitespace-nowrap" color="warning" size="sm">
                            {toPrettyDuration(dayjs.duration(stream.duration))}
                          </Chip>
                        </React.Fragment>
                      ))}
                    </>
                  )}
                </div>
              </Table.Cell>

              <Table.Cell>
                <Typography.Paragraph className="text-sm whitespace-nowrap text-right" variant="subtitle">
                  {filesize(item.node.size, { base: 2 })}
                </Typography.Paragraph>
              </Table.Cell>
            </Table.Row>
          )}
        </Table.Body>
      </Table.Root>

      {hasNext && (
        <div className="flex flex-row grow justify-center py-2 text-brand" ref={loader}>
          {isLoadingNext && <Spinner />}
        </div>
      )}
    </>
  )
}

export default ExploreTable
