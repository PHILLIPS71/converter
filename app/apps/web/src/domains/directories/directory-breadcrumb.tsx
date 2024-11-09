'use client'

import React from 'react'
import { Breadcrumb, Link, Typography } from '@giantnodes/react'
import { useFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { directoryBreadcrumbFragment$key } from '~/__generated__/directoryBreadcrumbFragment.graphql'
import type { Library } from '~/domains/libraries/library-store'

const FRAGMENT = graphql`
  fragment directoryBreadcrumbFragment on FileSystemDirectory {
    pathInfo {
      fullName
      directorySeparatorChar
    }
  }
`

type DirectoryBreadcrumbProps = {
  $key: directoryBreadcrumbFragment$key
  library: Library
}

type DirectoryBreadcrumbItem = {
  name: string
  path: string
  isLink: boolean
}

const DirectoryBreadcrumb: React.FC<DirectoryBreadcrumbProps> = ({ $key, library }) => {
  const data = useFragment(FRAGMENT, $key)

  const directories = React.useMemo<DirectoryBreadcrumbItem[]>(() => {
    const separator = data.pathInfo.directorySeparatorChar as string

    const parts = data.pathInfo.fullName.split(separator)
    const max = library.directory.pathInfo.fullName.split(separator).length - 1

    return parts.reduce<DirectoryBreadcrumbItem[]>((accumulator, current, index) => {
      const previous = accumulator[index - 1]?.path ?? ''
      const path = previous ? `${previous}${separator}${current}` : current
      const relative = path.replace(library.directory.pathInfo.fullName, '').replace(/^\/+/, '')

      accumulator.push({
        name: current,
        path: relative,
        isLink: index < parts.length - 1 && index >= max,
      })

      return accumulator
    }, [])
  }, [data.pathInfo.directorySeparatorChar, data.pathInfo.fullName, library.directory.pathInfo.fullName])

  return (
    <Breadcrumb.Root size="sm">
      {directories.map(({ name, path, isLink }, index) => (
        <Breadcrumb.Item key={name}>
          {isLink ? (
            <Link href={`/library/${library.slug}/explore/${path}`} underline="hover">
              {name}
            </Link>
          ) : (
            <Typography.Text className={index == directories.length - 1 ? 'font-semibold' : ''}>{name}</Typography.Text>
          )}
        </Breadcrumb.Item>
      ))}
    </Breadcrumb.Root>
  )
}

export default DirectoryBreadcrumb
