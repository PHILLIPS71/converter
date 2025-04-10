'use client'

import React from 'react'
import { Progress, Typography } from '@giantnodes/react'
import { IconPointFilled } from '@tabler/icons-react'
import { useFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { codec_directory$key } from '~/__generated__/codec_directory.graphql'
import { getHashedColor } from '~/utilities/colors'

type DirectoryCodecWidgetProps = {
  $key: codec_directory$key
}

const FRAGMENT = graphql`
  fragment codec_directory on FileSystemDirectory {
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

  const getColor = React.useCallback(
    (text?: string | null) => (text ? getHashedColor(text) : 'var(--color-partition)'),
    []
  )

  return (
    <div className="flex flex-col gap-2">
      <Progress.Root>
        {distribution.codec.map((item) => (
          <Progress.Bar color={getColor(item.key)} key={item.key ?? 'unknown'} width={(item.value / total) * 100} />
        ))}
      </Progress.Root>

      <ul className="flex flex-wrap gap-2">
        {distribution.codec.map((item) => (
          <li className="flex items-center gap-1" key={item.key}>
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
