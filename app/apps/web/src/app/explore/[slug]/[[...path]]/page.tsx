import React from 'react'
import { notFound } from 'next/navigation'
import { Card } from '@giantnodes/react'
import { graphql } from 'relay-runtime'

import type { page_ExplorePageQuery } from '~/__generated__/page_ExplorePageQuery.graphql'
import Breadcrumb from '~/domains/directories/directory-breadcrumb'
import ExploreTable from '~/domains/directories/explore-table'
import * as LibraryStore from '~/domains/libraries/library-store'
import RelayStoreHydrator from '~/libraries/relay/RelayStoreHydrator'
import { query } from '~/libraries/relay/server'

const QUERY = graphql`
  query page_ExplorePageQuery($pathname: String!) {
    directory(where: { pathInfo: { fullName: { eq: $pathname } } }) {
      ...directoryBreadcrumbFragment
      ...exploreTableFragment
    }
  }
`

const getDirectoryPath = (base: string, slug: string[], separator: string) => {
  if (!slug.length) return base

  const decoded = slug.map(decodeURI)
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
    library.directory.pathInfo.directorySeparatorChar as string
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
      <div className="flex flex-col gap-2">
        <Card.Root>
          <Card.Header>
            <Breadcrumb $key={data.directory} library={library} />
          </Card.Header>
        </Card.Root>

        <Card.Root>
          <ExploreTable $key={data.directory} />
        </Card.Root>
      </div>
    </RelayStoreHydrator>
  )
}

export default ExplorePage