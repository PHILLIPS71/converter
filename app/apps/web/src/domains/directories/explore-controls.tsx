'use client'

import type { FileSizeReturnObject } from 'filesize'
import React from 'react'
import { Button, Typography } from '@giantnodes/react'
import { IconDeviceFloppy, IconFile, IconFolderFilled, IconFolderSearch } from '@tabler/icons-react'
import { filesize } from 'filesize'
import { useFragment, useMutation } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { exploreControlsFragment$key } from '~/__generated__/exploreControlsFragment.graphql'
import { exploreControlsProbeMutation } from '~/__generated__/exploreControlsProbeMutation.graphql'

const FRAGMENT = graphql`
  fragment exploreControlsFragment on FileSystemDirectory {
    id
    size
  }
`

const MUTATION = graphql`
  mutation exploreControlsProbeMutation($input: EntryProbeInput!) {
    entryProbe(input: $input) {
      fileSystemEntry {
        id
      }
    }
  }
`

type ExploreControlsProps = {
  $key: exploreControlsFragment$key
}

const ExploreControls: React.FC<ExploreControlsProps> = ({ $key }) => {
  const data = useFragment(FRAGMENT, $key)
  const [commit, isLoading] = useMutation<exploreControlsProbeMutation>(MUTATION)

  const size = React.useMemo<FileSizeReturnObject>(
    () => filesize(data.size, { base: 2, output: 'object' }),
    [data.size]
  )

  const onProbeClick = () => {
    commit({
      variables: {
        input: {
          entryId: data.id,
        },
      },
    })
  }

  return (
    <div className="flex flex-col gap-3">
      <div className="flex flex-col sm:flex-row gap-3">
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

        <Button isLoading={isLoading} onClick={onProbeClick} size="xs">
          <IconFolderSearch size={16} /> Refresh
        </Button>
      </div>
    </div>
  )
}

export default ExploreControls
