'use client'

import React from 'react'
import { Button, Card, Dialog, Modal, Typography } from '@giantnodes/react'
import { IconX } from '@tabler/icons-react'

import type { PipelineEditInput, PipelineEditRef } from '~/domains/pipelines/pipeline-edit'
import PipelineEdit from '~/domains/pipelines/pipeline-edit'

type PipelineDialogProps = React.PropsWithChildren

const PipelineEditDialog: React.FC<PipelineDialogProps> = ({ children }) => {
  const ref = React.useRef<PipelineEditRef>(null)

  const onPipelineEdit = (input: PipelineEditInput) => {
    console.log(input)
  }

  return (
    <Dialog.Trigger>
      {children}

      <Modal.Root placement="right">
        <Modal.Content className="max-w-3xl w-full">
          <Dialog.Root>
            {({ close }) => (
              <Card.Root className="h-full">
                <Card.Header>
                  <div className="flex flex-row justify-between">
                    <Typography.Paragraph>path_info</Typography.Paragraph>

                    <Button color="transparent" onPress={close} size="xs">
                      <IconX size={16} strokeWidth={1} />
                    </Button>
                  </div>
                </Card.Header>

                <Card.Body>
                  <PipelineEdit onComplete={onPipelineEdit} ref={ref} />
                </Card.Body>

                <Card.Footer className="flex items-center justify-end gap-2">
                  <Button color="neutral" onPress={() => ref.current?.reset()} size="xs">
                    Reset
                  </Button>
                  <Button onPress={() => ref.current?.submit()} size="xs">
                    Save
                  </Button>
                </Card.Footer>
              </Card.Root>
            )}
          </Dialog.Root>
        </Modal.Content>
      </Modal.Root>
    </Dialog.Trigger>
  )
}

export default PipelineEditDialog
