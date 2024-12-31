'use client'

import type { SubmitHandler } from 'react-hook-form'
import React from 'react'
import { yaml } from '@codemirror/lang-yaml'
import { Alert, Form, Input } from '@giantnodes/react'
import { zodResolver } from '@hookform/resolvers/zod'
import { IconAlertCircleFilled } from '@tabler/icons-react'
import CodeMirror from '@uiw/react-codemirror'
import { Controller, useForm } from 'react-hook-form'
import { useMutation } from 'react-relay'
import { ConnectionHandler, graphql, ROOT_ID } from 'relay-runtime'
import * as z from 'zod'

import type {
  pipelineEditCreateMutation,
  pipelineEditCreateMutation$data,
} from '~/__generated__/pipelineEditCreateMutation.graphql'
import { giantnodes } from '~/libraries/codemirror/theme'

export type PipelineEditRef = {
  submit: () => void
  reset: () => void
}

type PipelineEditInput = z.infer<typeof PipelineEditSchema>

export type PipelineEditPayload = NonNullable<pipelineEditCreateMutation$data['pipelineCreate']['pipeline']>

type PipelineEditProps = {
  value?: PipelineEditInput
  onComplete?: (payload: PipelineEditPayload) => void
  onLoadingChange?: (isLoading: boolean) => void
}

const MUTATION = graphql`
  mutation pipelineEditCreateMutation($input: PipelineCreateInput!, $connections: [ID!]!) {
    pipelineCreate(input: $input) {
      pipeline @appendNode(connections: $connections, edgeTypeName: "PipelinesEdge") {
        id
        name
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

const PipelineEditSchema = z.object({
  name: z.string().trim().min(1, 'pipeline name is required').max(128, 'pipeline name must be 128 characters or less'),
  description: z.string().trim(),
  definition: z.string().trim().min(1, 'pipeline definition is required').default(''),
})

const PipelineEdit = React.forwardRef<PipelineEditRef, PipelineEditProps>((props, ref) => {
  const { value, onComplete, onLoadingChange } = props

  const [errors, setErrors] = React.useState<string[]>([])
  const [commit, isLoading] = useMutation<pipelineEditCreateMutation>(MUTATION)

  const form = useForm<PipelineEditInput>({
    resolver: zodResolver(PipelineEditSchema),
    defaultValues: value,
  })

  const onSubmit: SubmitHandler<PipelineEditInput> = React.useCallback(
    (data) => {
      const connection = ConnectionHandler.getConnectionID(ROOT_ID, 'PipelineSidebarCollection_pipelines', {
        order: [{ name: 'ASC' }],
      })

      commit({
        variables: {
          connections: [connection],
          input: {
            name: data.name,
            description: data.description,
            definition: data.definition,
          },
        },
        onCompleted: (payload) => {
          if (payload.pipelineCreate.errors != null) {
            const faults = payload.pipelineCreate.errors
              .filter((error): error is { message: string } => error.message !== undefined)
              .map((error) => error.message)

            setErrors(faults)

            return
          }

          if (payload.pipelineCreate.pipeline) onComplete?.(payload.pipelineCreate.pipeline)

          setErrors([])
        },
        onError: (error) => {
          setErrors([error.message])
        },
      })
    },
    [commit, onComplete]
  )

  React.useEffect(() => {
    onLoadingChange?.(isLoading)
  }, [isLoading, onLoadingChange])

  React.useImperativeHandle(
    ref,
    () => ({
      submit: async () => {
        await form.handleSubmit(onSubmit)()
      },
      reset: () => {
        form.reset()
        setErrors([])
      },
    }),
    [form, onSubmit]
  )

  return (
    <Form.Root className="h-full" onSubmit={form.handleSubmit(onSubmit)}>
      <div className="flex flex-col gap-y-3 h-full">
        {errors.length > 0 && (
          <Alert.Root color="danger">
            <IconAlertCircleFilled size={20} />
            <Alert.Body>
              <Alert.Heading>There were {errors.length} error with your submission</Alert.Heading>
              <Alert.List>
                {errors.map((error) => (
                  <Alert.Item key={error}>{error}</Alert.Item>
                ))}
              </Alert.List>
            </Alert.Body>
          </Alert.Root>
        )}

        <Form.Group {...form.register('name')} error={!!form.formState.errors.name}>
          <Form.Label>Name</Form.Label>
          <Input.Root size="sm">
            <Input.Text type="text" />
          </Input.Root>
          <Form.Feedback type="error">{form.formState.errors.name?.message}</Form.Feedback>
        </Form.Group>

        <Form.Group {...form.register('description')} error={!!form.formState.errors.description}>
          <Form.Label>Description</Form.Label>
          <Input.Root size="sm">
            <Input.TextArea />
          </Input.Root>
          <Form.Feedback type="error">{form.formState.errors.description?.message}</Form.Feedback>
        </Form.Group>

        <Form.Group {...form.register('definition')} className="flex-grow" error={!!form.formState.errors.definition}>
          <Form.Label>Definition</Form.Label>

          <div className="flex-grow">
            <Controller
              control={form.control}
              name="definition"
              render={({ field }) => (
                <CodeMirror
                  className="h-full"
                  extensions={[yaml()]}
                  onBlur={field.onBlur}
                  onChange={(value) => field.onChange(value)}
                  theme={giantnodes}
                  value={field.value}
                />
              )}
            />
          </div>

          <Form.Feedback type="error">{form.formState.errors.definition?.message}</Form.Feedback>
        </Form.Group>
      </div>
    </Form.Root>
  )
})

export default PipelineEdit
