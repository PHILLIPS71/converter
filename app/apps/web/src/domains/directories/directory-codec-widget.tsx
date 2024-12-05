'use client'

import React from 'react'
import { Progress, Typography } from '@giantnodes/react'
import { IconPointFilled } from '@tabler/icons-react'
import { useFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import { directoryCodecWidgetFragment$key } from '~/__generated__/directoryCodecWidgetFragment.graphql'
import { getHashedColor } from '~/utilities/colors'

type DirectoryCodecWidgetProps = {
  $key: directoryCodecWidgetFragment$key
}

const FRAGMENT = graphql`
  fragment directoryCodecWidgetFragment on FileSystemDirectory {
    pathInfo {
      fullNameNormalized
    }
    distribution {
      codec {
        key
        value
      }
    }
  }
`

const DirectoryCodecWidget: React.FC<DirectoryCodecWidgetProps> = ({ $key }) => {
  const { distribution } = useFragment(FRAGMENT, $key)

  const total = React.useMemo<number>(
    () => distribution.codec.reduce<number>((accu, item) => accu + item.value, 0),
    [distribution]
  )

  const getColor = React.useCallback((text?: string | null) => {
    return text ? getHashedColor(text) : 'hsl(var(--twc-partition))'
  }, [])

  return (
    <div className="flex flex-col gap-2">
      <Progress.Root>
        {distribution.codec.map((item) => (
          <Progress.Bar key={item.key ?? 'unknown'} color={getColor(item.key)} width={(item.value / total) * 100} />
        ))}
      </Progress.Root>

      <ul className="flex flex-wrap gap-2">
        {distribution.codec.map((item) => (
          <li key={item.key} className="flex items-center gap-1">
            <IconPointFilled color={getColor(item.key)} size={16} />
            <Typography.Text className="font-bold text-xs">{item.key ?? 'unknown'}</Typography.Text>
            <Typography.Text className="text-xs" variant="subtitle">
              {(item.value / total).toLocaleString(undefined, { style: 'percent', maximumFractionDigits: 2 })}
            </Typography.Text>
          </li>
        ))}
      </ul>
    </div>
  )
}

export default DirectoryCodecWidget
