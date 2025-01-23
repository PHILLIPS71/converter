'use client'

import React from 'react'
import Link from 'next/link'
import { Avatar, Button, cn, Divider, Input, Menu, Spinner, Typography } from '@giantnodes/react'
import { IconCheck, IconSearch, IconSelector } from '@tabler/icons-react'
import { usePaginationFragment } from 'react-relay'
import { graphql } from 'relay-runtime'
import { useDebounce } from 'use-debounce'

import type { libraryWidgetFragment_query$key } from '~/__generated__/libraryWidgetFragment_query.graphql'
import type {
  LibraryFilterInput,
  LibraryWidgetRefetchableQuery,
} from '~/__generated__/LibraryWidgetRefetchableQuery.graphql'
import { useLibrary } from '~/domains/libraries/use-library.hook'
import { useInfiniteScroll } from '~/hooks/use-infinite-scroll'

type LibraryWidgetProps = {
  $key: libraryWidgetFragment_query$key
}

const FRAGMENT = graphql`
  fragment libraryWidgetFragment_query on Query
  @refetchable(queryName: "LibraryWidgetRefetchableQuery")
  @argumentDefinitions(
    first: { type: "Int", defaultValue: 8 }
    after: { type: "String" }
    where: { type: "LibraryFilterInput" }
    order: { type: "[LibrarySortInput!]", defaultValue: [{ name: ASC }] }
  ) {
    libraries(first: $first, after: $after, where: $where, order: $order)
      @connection(key: "LibraryWidget_query_libraries") {
      edges {
        node {
          id
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

  const [isPending, startTransition] = React.useTransition()
  const [search, setSearch] = React.useState<string>('')
  const [debounced] = useDebounce(search, 300)

  const { data, refetch, hasNext, loadNext, isLoadingNext } = usePaginationFragment<
    LibraryWidgetRefetchableQuery,
    libraryWidgetFragment_query$key
  >(FRAGMENT, $key)

  const [loader] = useInfiniteScroll({
    onScroll: () => {
      if (hasNext) loadNext(8)
    },
  })

  const reset = () => {
    setSearch('')
  }

  React.useEffect(() => {
    startTransition(() => {
      const where: LibraryFilterInput = debounced.trim().length > 0 ? { name: { eq: debounced } } : {}
      refetch({ where })
    })
  }, [debounced, refetch])

  return (
    <Menu.Root size="sm" onOpenChange={reset}>
      <Button color="neutral" size="lg" className="justify-start p-1.5 w-full">
        {library && (
          <Avatar.Root size="xs" className="shrink-0">
            <Avatar.Image src={`https://avatar.vercel.sh/${library?.slug}`} />
          </Avatar.Root>
        )}

        <div className="flex flex-grow flex-col items-start text-left min-w-0">
          <Typography.Paragraph size="sm" truncate>
            {library?.name ?? 'Select a library'}
          </Typography.Paragraph>

          <Typography.Paragraph size="xs" variant="subtitle" className="font-mono" truncate>
            {library?.directory.pathInfo.fullName ?? 'No library selected'}
          </Typography.Paragraph>
        </div>

        <IconSelector size={16} strokeWidth={1} className="shrink-0" />
      </Button>

      <Menu.Popover placement="bottom right" className="p-0">
        <div className="flex flex-col">
          <Input.Root size="md" className="border-none">
            <Input.Addon>
              <IconSearch size={16} strokeWidth={1} />
            </Input.Addon>

            <Input.Text
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              aria-label="library search"
              placeholder="Search library..."
              className="placeholder:text-sm"
              type="text"
            />
          </Input.Root>

          <Divider />

          <div className="max-h-[196px] overflow-y-auto">
            <Menu.List className={cn('p-1.5', isPending ? 'blur-sm' : '')}>
              {data.libraries?.edges?.map((edge) => (
                <Menu.Item key={edge.node.id} onAction={() => setLibrary(edge.node.slug)}>
                  <div className="flex flex-row items-center gap-x-2 w-full">
                    <Avatar.Root size="xs" className="shrink-0">
                      <Avatar.Image src={`https://avatar.vercel.sh/${edge.node.slug}`} />
                    </Avatar.Root>

                    <div className="flex flex-grow flex-col items-start min-w-0">
                      <Typography.Paragraph size="sm" truncate>
                        {edge.node.name}
                      </Typography.Paragraph>
                      <Typography.Paragraph size="xs" variant="subtitle" className="font-mono" truncate>
                        {edge.node.directory.pathInfo.fullName}
                      </Typography.Paragraph>
                    </div>

                    {edge.node.id == library?.id && <IconCheck size={16} strokeWidth={1} className="shrink-0" />}
                  </div>
                </Menu.Item>
              ))}
            </Menu.List>

            {hasNext && (
              <div className="flex flex-row flex-grow justify-center text-brand" ref={loader}>
                {isLoadingNext && <Spinner />}
              </div>
            )}

            {data.libraries?.edges?.length == 0 && search.trim().length > 0 && (
              <div className="flex flex-col items-center justify-center py-6 text-center">
                <Typography.Paragraph size="sm">No library found</Typography.Paragraph>
              </div>
            )}
          </div>

          <Divider />

          <div className="p-1.5">
            <Button size="xs" className="w-full" as={Link} href="/create">
              Create Library
            </Button>
          </div>
        </div>
      </Menu.Popover>
    </Menu.Root>
  )
}

export default LibraryWidget
