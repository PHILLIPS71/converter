'use client'

import React from 'react'
import { Typography } from '@giantnodes/react'
import { IconFile } from '@tabler/icons-react'
import { graphql, useFragment } from 'react-relay'

import type { exploreTableFileFragment$key } from '~/__generated__/exploreTableFileFragment.graphql'

const FRAGMENT = graphql`
  fragment exploreTableFileFragment on FileSystemFile {
    pathInfo {
      name
    }
  }
`

type ExploreTableFileProps = {
  $key: exploreTableFileFragment$key
}

const ExploreTableFile: React.FC<ExploreTableFileProps> = ({ $key }) => {
  const data = useFragment(FRAGMENT, $key)

  return (
    <>
      <IconFile size={20} />
      <Typography.Paragraph>{data.pathInfo.name}</Typography.Paragraph>
    </>
  )
}

export default ExploreTableFile
