'use client'

import type { FileSizeReturnObject } from 'filesize'
import React from 'react'
import { Typography } from '@giantnodes/react'
import { IconDeviceFloppy, IconFile, IconFolderFilled } from '@tabler/icons-react'
import { filesize } from 'filesize'
import { useFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { exploreControlsFragment$key } from '~/__generated__/exploreControlsFragment.graphql'

const FRAGMENT = graphql`
  fragment exploreControlsFragment on FileSystemDirectory {
    size
  }
`

type ExploreControlsProps = {
  $key: exploreControlsFragment$key
}

const ExploreControls: React.FC<ExploreControlsProps> = ({ $key }) => {
  const data = useFragment(FRAGMENT, $key)

  const size = React.useMemo<FileSizeReturnObject>(
    () => filesize(data.size, { base: 2, output: 'object' }),
    [data.size]
  )

  return (
    <div className="flex flex-col gap-3">
      <div className="flex gap-3 flex-col sm:flex-row">
        <div className="flex flex-grow gap-3 justify-around flex-wrap sm:justify-normal">
          <div className="flex items-center gap-1">
            <IconFolderFilled size={16} />
            <Typography.Text as="strong" size="xs">
              0
            </Typography.Text>
            <Typography.Text size="xs">folders</Typography.Text>
          </div>

          <div className="flex items-center gap-1">
            <IconFile size={16} />
            <Typography.Text as="strong" size="xs">
              0
            </Typography.Text>
            <Typography.Text size="xs">files</Typography.Text>
          </div>

          <div className="flex items-center gap-1">
            <IconDeviceFloppy size={16} />
            <Typography.Text as="strong" size="xs">
              {size.value}
            </Typography.Text>
            <Typography.Text size="xs">{size.symbol}</Typography.Text>
          </div>
        </div>
      </div>
    </div>
  )
}

export default ExploreControls
