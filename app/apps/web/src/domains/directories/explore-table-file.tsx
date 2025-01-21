'use client'

import React from 'react'
import { Chip, Typography } from '@giantnodes/react'
import { IconFile } from '@tabler/icons-react'
import dayjs from 'dayjs'
import { graphql, useFragment } from 'react-relay'

import type { exploreTableFileFragment$key } from '~/__generated__/exploreTableFileFragment.graphql'
import { toPrettyDuration } from '~/libraries/dayjs'

const FRAGMENT = graphql`
  fragment exploreTableFileFragment on FileSystemFile {
    pathInfo {
      name
    }
    videoStreams {
      index
      codec
      duration
      quality {
        width
        height
        resolution {
          abbreviation
        }
      }
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
      <IconFile className="shrink-0" size={20} />

      <Typography.Paragraph className="truncate" title={data.pathInfo.name}>
        {data.pathInfo.name}
      </Typography.Paragraph>

      {data.videoStreams.map((stream) => (
        <React.Fragment key={stream.index}>
          <Chip className="whitespace-nowrap" color="cyan" size="sm">
            {stream.codec}
          </Chip>
          <Chip
            className="whitespace-nowrap"
            color="emerald"
            size="sm"
            title={`${stream.quality.width}x${stream.quality.height}`}
          >
            {stream.quality.resolution.abbreviation}
          </Chip>
          <Chip className="whitespace-nowrap" color="warning" size="sm">
            {toPrettyDuration(dayjs.duration(stream.duration))}
          </Chip>
        </React.Fragment>
      ))}
    </>
  )
}

export default ExploreTableFile
