'use client'

import React from 'react'
import { Progress, Typography } from '@giantnodes/react'
import { IconPointFilled } from '@tabler/icons-react'
import { useFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import { directoryResolutionWidgetFragment$key } from '~/__generated__/directoryResolutionWidgetFragment.graphql'
import { getHashedColor } from '~/utilities/colors'

type DirectoryResolutionWidgetProps = {
  $key: directoryResolutionWidgetFragment$key
}

const FRAGMENT = graphql`
  fragment directoryResolutionWidgetFragment on FileSystemDirectory {
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

  const getColor = React.useCallback((text?: string | null) => {
    return text ? getHashedColor(text) : 'hsl(var(--twc-shark-400))'
  }, [])

  return (
    <div className="flex flex-col gap-2">
      <Progress.Root>
        {distribution.resolution.map((item) => (
          <Progress.Bar
            key={item.key?.name ?? 'unknown'}
            color={getColor(item.key?.name)}
            width={(item.value / total) * 100}
          />
        ))}
      </Progress.Root>

      <ul className="flex flex-wrap gap-2">
        {distribution.resolution.map((item) => (
          <li key={item.key?.name} className="flex items-center gap-1">
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
