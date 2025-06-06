'use client'

import React from 'react'
import { Progress, Typography } from '@giantnodes/react'
import { IconPointFilled } from '@tabler/icons-react'
import { useFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { resolution_directory$key } from '~/__generated__/resolution_directory.graphql'
import { getHashedColor } from '~/utilities/colors'

type DirectoryResolutionWidgetProps = {
  $key: resolution_directory$key
}

const FRAGMENT = graphql`
  fragment resolution_directory on FileSystemDirectory {
    pathInfo {
      fullNameNormalized
    }
    distribution {
      resolution {
        key {
          name
          abbreviation
        }
        value
      }
    }
  }
`

const DirectoryResolutionWidget: React.FC<DirectoryResolutionWidgetProps> = ({ $key }) => {
  const { distribution } = useFragment(FRAGMENT, $key)

  const total = React.useMemo<number>(
    () => distribution.resolution.reduce<number>((accu, item) => accu + item.value, 0),
    [distribution]
  )

  const getColor = React.useCallback(
    (text?: string | null) => (text ? getHashedColor(text) : 'var(--color-shark-400)'),
    []
  )

  return (
    <div className="flex flex-col gap-2">
      <Progress.Root>
        {distribution.resolution.map((item) => (
          <Progress.Bar
            color={getColor(item.key?.name)}
            key={item.key?.name ?? 'unknown'}
            width={(item.value / total) * 100}
          />
        ))}
      </Progress.Root>

      <ul className="flex flex-wrap gap-2">
        {distribution.resolution.map((item) => (
          <li className="flex items-center gap-1" key={item.key?.name ?? 'unknown'}>
            <IconPointFilled color={getColor(item.key?.name)} size={16} />
            <Typography.Text className="font-bold text-xs" title={item.key?.name}>
              {item.key?.abbreviation ?? 'unknown'}
            </Typography.Text>
            <Typography.Text className="text-xs" variant="subtitle">
              {(item.value / total).toLocaleString(undefined, { style: 'percent', maximumFractionDigits: 2 })}
            </Typography.Text>
          </li>
        ))}
      </ul>
    </div>
  )
}

export default DirectoryResolutionWidget
