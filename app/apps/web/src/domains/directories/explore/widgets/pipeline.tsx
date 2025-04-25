'use client'

import React from 'react'
import { Button, Menu, Typography } from '@giantnodes/react'
import { IconPlug } from '@tabler/icons-react'
import { useFragment, useMutation } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { pipeline_ExecuteMutation } from '~/__generated__/pipeline_ExecuteMutation.graphql'
import type { pipeline_explore_widget_query$key } from '~/__generated__/pipeline_explore_widget_query.graphql'
import { useExplore } from '~/domains/directories/explore/use-explore.hook'

const FRAGMENT = graphql`
  fragment pipeline_explore_widget_query on Query
  @refetchable(queryName: "ExploreControlPipelineRefetchableQuery")
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
  mutation pipeline_ExecuteMutation($input: PipelineExecuteInput!) {
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

type ExplorePipelineWidgetProps = {
  $key: pipeline_explore_widget_query$key
}

const Pipeline: React.FC<ExplorePipelineWidgetProps> = ({ $key }) => {
  const { keys, setKeys } = useExplore()

  const data = useFragment(FRAGMENT, $key)
  const [commit, isLoading] = useMutation<pipeline_ExecuteMutation>(MUTATION)

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
      onCompleted: () => {
        setKeys(new Set<string>())
      },
    })
  }

  return (
    <Menu.Root size="sm">
      <Button isDisabled={isDisabled} isLoading={isLoading} size="xs">
        <IconPlug size={16} /> <Typography.Text>Run Pipeline</Typography.Text>
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
              <Typography.Text className="w-full truncate" size="sm">
                {edge.node.name}
              </Typography.Text>
              <Typography.Text className="w-full truncate" size="xs" variant="subtitle">
                {edge.node.description}
              </Typography.Text>
            </Menu.Item>
          ))}
        </Menu.List>
      </Menu.Popover>
    </Menu.Root>
  )
}

export default Pipeline
