import React from 'react'
import { notFound } from 'next/navigation'
import { Card, Typography } from '@giantnodes/react'
import { graphql } from 'relay-runtime'

import type { page_ExplorePageQuery } from '~/__generated__/page_ExplorePageQuery.graphql'
import DirectoryBreadcrumb from '~/domains/directories/directory-breadcrumb'
import DirectoryCodecWidget from '~/domains/directories/directory-codec-widget'
import DirectoryContainerWidget from '~/domains/directories/directory-container-widget'
import DirectoryResolutionWidget from '~/domains/directories/directory-resolution-widget'
import ExploreControls from '~/domains/directories/explore-controls'
import ExploreTable from '~/domains/directories/explore-table'
import * as LibraryStore from '~/domains/libraries/library-store'
import RelayStoreHydrator from '~/libraries/relay/RelayStoreHydrator'
import { query } from '~/libraries/relay/server'

const QUERY = graphql`
  query page_ExplorePageQuery($pathname: String!) {
    directory(where: { pathInfo: { fullName: { eq: $pathname } } }) {
      ...directoryBreadcrumbFragment
      ...exploreControlsFragment
      ...exploreTableFragment
      ...directoryCodecWidgetFragment
      ...directoryResolutionWidgetFragment
      ...directoryContainerWidgetFragment
    }
  }
`

const getDirectoryPath = (base: string, slug: string[], separator: string) => {
  if (!slug.length) return base

  const decoded = slug.map(decodeURIComponent)
  return `${base}${separator}${decoded.join(separator)}`
}

type ExplorePageProps = {
  params: Promise<{
    slug: string
    path?: string[]
  }>
}

const ExplorePage = async ({ params }: ExplorePageProps): Promise<React.ReactNode> => {
  const [library, { path }] = await Promise.all([LibraryStore.get(), params])

  if (library == null) {
    console.warn(`the library was not found`)
    return notFound()
  }

  const pathname = getDirectoryPath(
    library.directory.pathInfo.fullName,
    path ?? [],
    library.directory.pathInfo.directorySeparatorChar
  )

  const { data, ...operation } = await query<page_ExplorePageQuery>(QUERY, {
    pathname,
  })

  if (data.directory == null) {
    console.warn(`the directory at ${pathname} was not found`)
    return notFound()
  }

  return (
    <RelayStoreHydrator operation={operation}>
      <div className="flex flex-row flex-wrap gap-2">
        <div className="flex flex-col flex-grow gap-2">
          <Card.Root>
            <Card.Header>
              <DirectoryBreadcrumb $key={data.directory} library={library} />
            </Card.Header>
          </Card.Root>

          <Card.Root>
            <Card.Header>
              <ExploreControls $key={data.directory} />
            </Card.Header>
          </Card.Root>

          <Card.Root>
            <ExploreTable $key={data.directory} />
          </Card.Root>
        </div>

        <div className="flex flex-col gap-2 w-80">
          <Card.Root size="sm">
            <Card.Header>
              <Typography.Text size="sm">Container</Typography.Text>
            </Card.Header>

            <Card.Body>
              <DirectoryContainerWidget $key={data.directory} />
            </Card.Body>
          </Card.Root>

          <Card.Root size="sm">
            <Card.Header>
              <Typography.Text size="sm">Codec</Typography.Text>
            </Card.Header>

            <Card.Body>
              <DirectoryCodecWidget $key={data.directory} />
            </Card.Body>
          </Card.Root>

          <Card.Root size="sm">
            <Card.Header>
              <Typography.Text size="sm">Resolution</Typography.Text>
            </Card.Header>

            <Card.Body>
              <DirectoryResolutionWidget $key={data.directory} />
            </Card.Body>
          </Card.Root>
        </div>
      </div>
    </RelayStoreHydrator>
  )
}

export default ExplorePage
