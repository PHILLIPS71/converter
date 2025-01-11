'use client'

import { Button } from '@giantnodes/react'
import { IconFolderSearch } from '@tabler/icons-react'
import { useFragment, useMutation } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { exploreControlRefreshFragment$key } from '~/__generated__/exploreControlRefreshFragment.graphql'
import type { exploreControlRefreshMutation } from '~/__generated__/exploreControlRefreshMutation.graphql'

const FRAGMENT = graphql`
  fragment exploreControlRefreshFragment on FileSystemDirectory {
    id
    size
  }
`

const MUTATION = graphql`
  mutation exploreControlRefreshMutation($input: EntryProbeInput!) {
    entryProbe(input: $input) {
      fileSystemEntry {
        id
      }
    }
  }
`

type ExploreControlRefreshProps = {
  $key: exploreControlRefreshFragment$key
}

const ExploreControlRefresh: React.FC<ExploreControlRefreshProps> = ({ $key }) => {
  const data = useFragment(FRAGMENT, $key)

  const [commit, isLoading] = useMutation<exploreControlRefreshMutation>(MUTATION)

  const onClick = () => {
    commit({
      variables: {
        input: {
          entryId: data.id,
        },
      },
    })
  }

  return (
    <Button isLoading={isLoading} onClick={onClick} size="xs">
      <IconFolderSearch size={16} /> Refresh
    </Button>
  )
}

export default ExploreControlRefresh
