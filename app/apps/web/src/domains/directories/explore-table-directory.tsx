'use client'

import React from 'react'
import { usePathname } from 'next/navigation'
import { Link } from '@giantnodes/react'
import { IconFolderFilled } from '@tabler/icons-react'
import { graphql, useFragment } from 'react-relay'

import type { exploreTableDirectoryFragment$key } from '~/__generated__/exploreTableDirectoryFragment.graphql'

const FRAGMENT = graphql`
  fragment exploreTableDirectoryFragment on FileSystemDirectory {
    pathInfo {
      name
    }
  }
`

type ExploreTableDirectoryProps = {
  $key: exploreTableDirectoryFragment$key
}

const ExploreTableDirectory: React.FC<ExploreTableDirectoryProps> = ({ $key }) => {
  const pathname = usePathname()

  const data = useFragment(FRAGMENT, $key)

  return (
    <>
      <IconFolderFilled size={20} />

      <Link href={`${pathname}/${data.pathInfo.name}`}>{data.pathInfo.name}</Link>
    </>
  )
}

export default ExploreTableDirectory
