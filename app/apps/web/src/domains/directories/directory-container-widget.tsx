'use client'

import React from 'react'
import { Progress, Typography } from '@giantnodes/react'
import { IconPointFilled } from '@tabler/icons-react'
import { useFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { directoryContainerWidgetFragment$key } from '~/__generated__/directoryContainerWidgetFragment.graphql'

type DirectoryContainerWidgetProps = {
  $key: directoryContainerWidgetFragment$key
}

const FRAGMENT = graphql`
  fragment directoryContainerWidgetFragment on FileSystemDirectory {
    pathInfo {
      fullNameNormalized
    }
    distribution {
      container {
        key {
          name
          extension
          color
        }
        value
      }
    }
  }
`

const DirectoryContainerWidget: React.FC<DirectoryContainerWidgetProps> = ({ $key }) => {
  const { distribution } = useFragment(FRAGMENT, $key)
  const partition = 'var(--color-shark-400)'

  const total = React.useMemo<number>(
    () => distribution.container.reduce<number>((accu, item) => accu + item.value, 0),
    [distribution]
  )

  return (
    <div className="flex flex-col gap-2">
      <Progress.Root>
        {distribution.container.map((item) => (
          <Progress.Bar
            color={item.key?.color ?? partition}
            key={item.key?.name ?? 'unknown'}
            width={(item.value / total) * 100}
          />
        ))}
      </Progress.Root>

      <ul className="flex flex-wrap gap-2">
        {distribution.container.map((item) => (
          <li className="flex items-center gap-1" key={item.key?.name}>
            <IconPointFilled color={item.key?.color ?? partition} size={16} />
            <Typography.Text className="font-bold text-xs" title={item.key?.name}>
              {item.key?.extension ?? 'unknown'}
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

export default DirectoryContainerWidget
