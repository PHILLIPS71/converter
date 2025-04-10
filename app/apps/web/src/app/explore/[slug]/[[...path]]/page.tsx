import React from 'react'
import { notFound } from 'next/navigation'
import { Card, Typography } from '@giantnodes/react'
import { graphql } from 'relay-runtime'

import type { page_directory_Query } from '~/__generated__/page_directory_Query.graphql'
import { ExploreBreadcrumb, ExploreControls, ExploreTable } from '~/domains/directories/explore'
import {
  DirectoryCodecWidget,
  DirectoryContainerWidget,
  DirectoryResolutionWidget,
} from '~/domains/directories/widgets'
import { LibraryService } from '~/domains/libraries/service'
import RelayStoreHydrator from '~/libraries/relay/RelayStoreHydrator'
import { query } from '~/libraries/relay/server'
import { isFailure } from '~/utilities/result-pattern'

type ExplorePageProps = {
  params: Promise<{
    slug: string
    path?: string[]
  }>
}

const QUERY = graphql`
  query page_directory_Query($pathname: String!) {
    directory(where: { pathInfo: { fullName: { eq: $pathname } } }) {
      ...breadcrumb_directory
      ...controls_directory
      ...table_directory
      ...codec_directory
      ...container_directory
      ...resolution_directory
    }
  }
`

const getDirectoryPath = (base: string, slug: string[], separator: string) => {
  if (!slug.length) return base

  const decoded = slug.map(decodeURIComponent)
  return `${base}${separator}${decoded.join(separator)}`
}

const ExplorePage: React.FC<ExplorePageProps> = async ({ params }) => {
  const [library, { path }] = await Promise.all([LibraryService.get(), params])

  if (isFailure(library)) {
    return notFound()
  }

  if (library.value == null) {
    return notFound()
  }

  const pathname = getDirectoryPath(
    library.value.directory.pathInfo.fullName,
    path ?? [],
    library.value.directory.pathInfo.directorySeparatorChar
  )

  const {
    data: { directory },
    ...operation
  } = await query<page_directory_Query>(QUERY, {
    pathname,
  })

  if (directory == null) {
    console.warn(`the directory at ${pathname} was not found`)
    return notFound()
  }

  return (
    <RelayStoreHydrator operation={operation}>
      <div className="flex flex-row gap-2 flex-wrap xl:flex-nowrap">
        <div className="flex flex-col grow gap-2">
          <Card.Root>
            <Card.Body>
              <ExploreBreadcrumb $key={directory} library={library.value} />
            </Card.Body>
          </Card.Root>

          <Card.Root>
            <Card.Body>
              <ExploreControls $key={directory} />
            </Card.Body>
          </Card.Root>

          <ExploreTable $key={directory} />
        </div>

        <div className="flex flex-col gap-2 w-80">
          <Card.Root size="sm">
            <Card.Header>
              <Typography.Text size="sm">Container</Typography.Text>
            </Card.Header>

            <Card.Body>
              <DirectoryContainerWidget $key={directory} />
            </Card.Body>
          </Card.Root>

          <Card.Root size="sm">
            <Card.Header>
              <Typography.Text size="sm">Codec</Typography.Text>
            </Card.Header>

            <Card.Body>
              <DirectoryCodecWidget $key={directory} />
            </Card.Body>
          </Card.Root>

          <Card.Root size="sm">
            <Card.Header>
              <Typography.Text size="sm">Resolution</Typography.Text>
            </Card.Header>

            <Card.Body>
              <DirectoryResolutionWidget $key={directory} />
            </Card.Body>
          </Card.Root>
        </div>
      </div>
    </RelayStoreHydrator>
  )
}

export default ExplorePage
