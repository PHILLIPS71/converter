'use client'

import React from 'react'
import { Avatar, Button, cn, Divider, Input, Link, Menu, Spinner, Typography } from '@giantnodes/react'
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
          slug
          name
          directory {
            pathInfo {
              fullName
              directorySeparatorChar
            }
          }
        }
      }
    }
  }
`

const LibraryWidget: React.FC<LibraryWidgetProps> = ({ $key }) => {
  const { library, set } = useLibrary()

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
    <Menu.Root onOpenChange={reset} size="sm">
      <Button className="justify-start p-1.5 space-x-2.5" color="neutral" size="xl" block>
        {library && (
          <Avatar.Root className="shrink-0" color="none" radius="sm" size="sm">
            <Avatar.Image src={`https://avatar.vercel.sh/${library.slug}`} />
          </Avatar.Root>
        )}

        <div className="flex grow flex-col items-start text-left min-w-0">
          <Typography.Paragraph size="sm" truncate>
            {library?.slug ?? 'Select a library'}
          </Typography.Paragraph>

          <Typography.Paragraph className="font-mono" size="xs" variant="subtitle" truncate>
            {library?.directory.pathInfo.fullName ?? 'No library selected'}
          </Typography.Paragraph>
        </div>

        <IconSelector className="shrink-0" size={24} strokeWidth={1} />
      </Button>

      <Menu.Popover placement="bottom right">
        <div className="flex flex-col">
          <Input.Root className="border-none" size="md">
            <Input.Addon>
              <IconSearch size={16} strokeWidth={1} />
            </Input.Addon>

            <Input.Text
              aria-label="library selector search"
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Search library..."
              type="text"
              value={search}
            />
          </Input.Root>

          <Divider />

          <div className="max-h-[164px] overflow-y-auto">
            <Menu.List className={cn(isPending ? 'blur-xs' : '')}>
              {data.libraries?.edges?.map((edge) => (
                <Menu.Item key={edge.node.id} onAction={() => set(edge.node)}>
                  <div className="flex flex-row items-center gap-x-2 w-full">
                    <Avatar.Root className="rounded-sm shrink-0" color="none" radius="sm" size="sm">
                      <Avatar.Image src={`https://avatar.vercel.sh/${edge.node.id}`} />
                    </Avatar.Root>

                    <div className="flex grow flex-col items-start min-w-0">
                      <Typography.Paragraph size="sm" truncate>
                        {edge.node.name}
                      </Typography.Paragraph>
                      <Typography.Paragraph className="font-mono" size="xs" variant="subtitle" truncate>
                        {edge.node.directory.pathInfo.fullName}
                      </Typography.Paragraph>
                    </div>

                    {edge.node.id == library?.id && <IconCheck className="shrink-0" size={16} strokeWidth={1} />}
                  </div>
                </Menu.Item>
              ))}
            </Menu.List>

            {hasNext && (
              <div className="flex flex-row grow justify-center text-brand" ref={loader}>
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

          <div className="m-1.5">
            <Button as={Link} href="/create" size="xs" block>
              Create Library
            </Button>
          </div>
        </div>
      </Menu.Popover>
    </Menu.Root>
  )
}

export default LibraryWidget
