'use client'

import React from 'react'
import { useParams, useRouter } from 'next/navigation'
import { Button, Menu } from '@giantnodes/react'
import { IconDotsVertical } from '@tabler/icons-react'
import { useFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { editMenu_pipeline$key } from '~/__generated__/editMenu_pipeline.graphql'
import type { PipelineEditInput, PipelineEditPayload } from '~/domains/pipelines/construct'
import { PipelineEditDialog } from '~/domains/pipelines/construct'

const FRAGMENT = graphql`
  fragment editMenu_pipeline on Pipeline {
    id
    name
    description
    definition
  }
`

type PipelineEditMenuProps = {
  $key: editMenu_pipeline$key
}

const PipelineEditMenu: React.FC<PipelineEditMenuProps> = ({ $key }) => {
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

export default PipelineEditMenu
