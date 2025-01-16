'use client'

import React from 'react'
import { Button, Menu, Typography } from '@giantnodes/react'
import { IconPlug } from '@tabler/icons-react'
import { useFragment, useMutation } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { exploreControlPipelineFragment$key } from '~/__generated__/exploreControlPipelineFragment.graphql'
import { exploreControlPipelineMutation } from '~/__generated__/exploreControlPipelineMutation.graphql'
import { useExplore } from '~/domains/directories/use-explore.hook'

const FRAGMENT = graphql`
  fragment exploreControlPipelineFragment on Query
  @refetchable(queryName: "ExploreControlPipelinePaginationQuery")
  @argumentDefinitions(
    first: { type: "Int", defaultValue: 10 }
    after: { type: "String" }
    order: { type: "[PipelineSortInput!]", defaultValue: [{ name: ASC }] }
  ) {
    pipelines(first: $first, after: $after, order: $order) @connection(key: "ExploreControlPipeline_pipelines") {
      edges {
        node {
          id
          name
          description
        }
      }
      pageInfo {
        hasNextPage
      }
    }
  }
`

const MUTATION = graphql`
  mutation exploreControlPipelineMutation($input: PipelineExecuteInput!) {
    pipelineExecute(input: $input) {
      results {
        file {
          id
        }
        faults {
          description
        }
      }
      errors {
        ... on DomainError {
          message
        }
        ... on ValidationError {
          message
        }
      }
    }
  }
`

type ExploreControlPipelineProps = {
  $key: exploreControlPipelineFragment$key
}

const ExploreControlPipeline: React.FC<ExploreControlPipelineProps> = ({ $key }) => {
  const { keys } = useExplore()

  const data = useFragment(FRAGMENT, $key)
  const [commit, isLoading] = useMutation<exploreControlPipelineMutation>(MUTATION)

  const isDisabled = React.useMemo<boolean>(() => {
    if (typeof keys === 'string') return false

    if (typeof keys === 'object') return keys.size === 0

    return true
  }, [keys])

  const files = React.useMemo<string[]>(() => {
    if (keys === 'all') return ['all']

    if (keys instanceof Set) {
      return Array.from(keys.values()).map(String)
    }

    return []
  }, [keys])

  const onClick = (pipeline: string) => {
    commit({
      variables: {
        input: {
          pipelineId: pipeline,
          entries: files,
        },
      },
    })
  }

  return (
    <Menu.Root size="sm">
      <Button isDisabled={isDisabled} size="xs" isLoading={isLoading}>
        <IconPlug size={16} />
        Run Pipeline
      </Button>

      <Menu.Popover className="w-96" placement="bottom right">
        <Menu.List>
          {data.pipelines?.edges?.map((edge) => (
            <Menu.Item
              className="flex flex-col items-start gap-0"
              id={edge.node.id}
              key={edge.node.id}
              onAction={() => onClick(edge.node.id)}
            >
              <Typography.Text className="w-full truncate">{edge.node.name}</Typography.Text>
              <Typography.Text className="w-full truncate" variant="subtitle">
                {edge.node.description}
              </Typography.Text>
            </Menu.Item>
          ))}
        </Menu.List>
      </Menu.Popover>
    </Menu.Root>
  )
}

export default ExploreControlPipeline
