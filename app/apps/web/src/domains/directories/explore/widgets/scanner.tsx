'use client'

import React from 'react'
import { Button, Menu, Typography } from '@giantnodes/react'
import { IconFolderSearch } from '@tabler/icons-react'
import { useFragment, useMutation } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { scanner_DeepScanMutation } from '~/__generated__/scanner_DeepScanMutation.graphql'
import type { scanner_directory$key } from '~/__generated__/scanner_directory.graphql'
import type { scanner_QuickScanMutation } from '~/__generated__/scanner_QuickScanMutation.graphql'

type ExploreScannerWidgetProps = {
  $key: scanner_directory$key
}

const FRAGMENT = graphql`
  fragment scanner_directory on FileSystemDirectory {
    id
    size
  }
`

const QUICK_SCAN_MUTATION = graphql`
  mutation scanner_QuickScanMutation($input: DirectoryScanInput!) {
    directoryScan(input: $input) {
      fileSystemDirectory {
        ...scanner_directory
      }
    }
  }
`

const DEEP_SCAN_MUTATION = graphql`
  mutation scanner_DeepScanMutation($input: EntryProbeInput!) {
    entryProbe(input: $input) {
      fileSystemEntry {
        id
      }
    }
  }
`

const Scanner: React.FC<ExploreScannerWidgetProps> = ({ $key }) => {
  const data = useFragment(FRAGMENT, $key)

  const [commitQuickScan, isQuickScanning] = useMutation<scanner_QuickScanMutation>(QUICK_SCAN_MUTATION)
  const [commitDeepScan, isDeepScanning] = useMutation<scanner_DeepScanMutation>(DEEP_SCAN_MUTATION)

  const isLoading = React.useMemo<boolean>(() => isQuickScanning || isDeepScanning, [isQuickScanning, isDeepScanning])

  const onQuickScanClick = () => {
    commitQuickScan({
      variables: {
        input: {
          directoryId: data.id,
        },
      },
    })
  }

  const onDeepScanClick = () => {
    commitDeepScan({
      variables: {
        input: {
          entryId: data.id,
        },
      },
    })
  }

  return (
    <Menu.Root size="sm">
      <Button isLoading={isLoading} size="xs">
        <IconFolderSearch size={16} /> <Typography.Text>Refresh</Typography.Text>
      </Button>

      <Menu.Popover className="w-fit" placement="bottom right">
        <Menu.List>
          <Menu.Item className="flex flex-col items-start gap-0" onAction={onQuickScanClick}>
            <Typography.Text className="w-full truncate" size="sm">
              Quick Scan
            </Typography.Text>
            <Typography.Text className="w-full truncate" size="xs" variant="subtitle">
              Updates file and folder structure only
            </Typography.Text>
          </Menu.Item>
          <Menu.Item className="flex flex-col items-start gap-0" onAction={onDeepScanClick}>
            <Typography.Text className="w-full truncate" size="sm">
              Deep Scan
            </Typography.Text>
            <Typography.Text className="w-full truncate" size="xs" variant="subtitle">
              Updates structure and analyzes media content
            </Typography.Text>
          </Menu.Item>
        </Menu.List>
      </Menu.Popover>
    </Menu.Root>
  )
}

export default Scanner
