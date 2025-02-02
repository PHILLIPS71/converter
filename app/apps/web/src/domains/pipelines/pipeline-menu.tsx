'use client'

import React from 'react'
import { useParams, useRouter } from 'next/navigation'
import { Button, Menu } from '@giantnodes/react'
import { IconDotsVertical } from '@tabler/icons-react'
import { useFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { pipelineMenuFragment_pipeline$key } from '~/__generated__/pipelineMenuFragment_pipeline.graphql'
import type { PipelineEditInput, PipelineEditPayload } from '~/domains/pipelines/pipeline-edit'
import PipelineEditDialog from '~/domains/pipelines/pipeline-edit-dialog'

const FRAGMENT = graphql`
  fragment pipelineMenuFragment_pipeline on Pipeline {
    id
    name
    description
    definition
  }
`

type PipelineSlugHeadingProps = {
  $key: pipelineMenuFragment_pipeline$key
}

const PipelineMenu: React.FC<PipelineSlugHeadingProps> = ({ $key }) => {
  const router = useRouter()
  const params = useParams()
  const data = useFragment(FRAGMENT, $key)

  const [isOpen, setOpen] = React.useState<boolean>(false)

  const value = React.useMemo<PipelineEditInput>(
    () => ({
      id: data.id,
      name: data.name,
      description: data.description,
      definition: data.definition,
    }),
    [data.definition, data.description, data.id, data.name]
  )

  const onEdit = (payload: PipelineEditPayload) => {
    if (payload.slug !== params.slug) {
      router.replace(`/pipelines/${payload.slug}`)
    }
  }

  return (
    <>
      <Menu.Root size="xs">
        <Button color="neutral" size="xs">
          <IconDotsVertical size={16} />
        </Button>

        <Menu.Popover className="w-fit" placement="bottom right">
          <Menu.List>
            <Menu.Item onAction={() => setOpen(true)}>Edit</Menu.Item>
          </Menu.List>
        </Menu.Popover>
      </Menu.Root>

      <PipelineEditDialog isOpen={isOpen} onComplete={onEdit} onOpenChange={(value) => setOpen(value)} value={value} />
    </>
  )
}

export default PipelineMenu
