'use client'

import React from 'react'
import { Button, Card, Dialog, Modal, Typography } from '@giantnodes/react'
import { IconX } from '@tabler/icons-react'

import type { PipelineEditInput, PipelineEditPayload, PipelineEditRef } from '~/domains/pipelines/pipeline-edit'
import PipelineEdit from '~/domains/pipelines/pipeline-edit'

type PipelineDialogProps = React.PropsWithChildren & {
  value?: PipelineEditInput
  isOpen?: boolean
  onOpenChange?: (isOpen: boolean) => void
  onComplete?: (payload: PipelineEditPayload) => void
}

const PipelineEditDialog: React.FC<PipelineDialogProps> = ({ children, value, isOpen, onOpenChange, onComplete }) => {
  const ref = React.useRef<PipelineEditRef>(null)
  const [isLoading, setLoading] = React.useState<boolean>(false)

  return (
    <Dialog.Trigger isOpen={isOpen} onOpenChange={onOpenChange}>
      {children}

      <Modal.Root placement="right">
        <Modal.Content className="max-w-3xl w-full">
          <Dialog.Root>
            {({ close }) => (
              <Card.Root className="h-full">
                <Card.Header>
                  <div className="flex flex-row justify-between">
                    <Typography.Paragraph>{value?.name ?? 'Create a Pipeline'}</Typography.Paragraph>

                    <Button color="neutral" onPress={close} size="xs">
                      <IconX size={16} strokeWidth={1} />
                    </Button>
                  </div>
                </Card.Header>

                <Card.Body>
                  <PipelineEdit
                    onComplete={(payload) => {
                      onComplete?.(payload)
                      close()
                    }}
                    onLoadingChange={setLoading}
                    ref={ref}
                    value={value}
                  />
                </Card.Body>

                <Card.Footer className="flex items-center justify-end gap-2">
                  <Button color="neutral" onPress={() => ref.current?.reset()} size="xs">
                    Reset
                  </Button>
                  <Button isDisabled={isLoading} onPress={() => ref.current?.submit()} size="xs">
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
