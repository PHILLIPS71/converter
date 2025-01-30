'use client'

// todo: https://github.com/react-hook-form/react-hook-form/issues/12298
'use no memo'

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
import type {
  pipelineEditUpdateMutation,
  pipelineEditUpdateMutation$data,
} from '~/__generated__/pipelineEditUpdateMutation.graphql'
import { giantnodes } from '~/libraries/codemirror/theme'

export type PipelineEditRef = {
  submit: () => void
  reset: () => void
}

export type PipelineEditInput = z.infer<typeof PipelineEditSchema>

export type PipelineEditPayload = NonNullable<pipelineEditCreateMutation$data['pipelineCreate']['pipeline']>

type PipelineEditProps = {
  value?: PipelineEditInput
  onComplete?: (payload: PipelineEditPayload) => void
  onLoadingChange?: (isLoading: boolean) => void
}

const MUTATION_CREATE = graphql`
  mutation pipelineEditCreateMutation($input: PipelineCreateInput!, $connections: [ID!]!) {
    pipelineCreate(input: $input) {
      pipeline @appendNode(connections: $connections, edgeTypeName: "PipelinesEdge") {
        id
        slug
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

const MUTATION_UPDATE = graphql`
  mutation pipelineEditUpdateMutation($input: PipelineUpdateInput!) {
    pipelineUpdate(input: $input) {
      pipeline {
        id
        slug
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
  id: z.string().nullish(),
  name: z.string().trim().min(1, 'pipeline name is required').max(128, 'pipeline name must be 128 characters or less'),
  description: z.string().trim().nullish(),
  definition: z.string().trim().min(1, 'pipeline definition is required').default(''),
})

const PipelineEdit = React.forwardRef<PipelineEditRef, PipelineEditProps>((props, ref) => {
  const { value, onComplete, onLoadingChange } = props

  const [errors, setErrors] = React.useState<string[]>([])
  const [create, isCreateLoading] = useMutation<pipelineEditCreateMutation>(MUTATION_CREATE)
  const [update, isUpdateLoading] = useMutation<pipelineEditUpdateMutation>(MUTATION_UPDATE)

  const isLoading = React.useMemo(() => isCreateLoading || isUpdateLoading, [isCreateLoading, isUpdateLoading])

  const form = useForm<PipelineEditInput>({
    resolver: zodResolver(PipelineEditSchema),
    defaultValues: value,
  })

  const upsert = React.useCallback(
    async (input: PipelineEditInput) => {
      const id = value?.id

      const common = {
        name: input.name,
        description: input.description,
        definition: input.definition,
      }

      let response: pipelineEditCreateMutation$data | pipelineEditUpdateMutation$data

      if (id == null) {
        const connection = ConnectionHandler.getConnectionID(ROOT_ID, 'PipelineSidebarCollection_pipelines', {
          order: [{ name: 'ASC' }],
        })

        response = await new Promise<pipelineEditCreateMutation$data>((resolve, reject) => {
          create({
            variables: {
              input: {
                ...common,
              },
              connections: [connection],
            },
            onCompleted: resolve,
            onError: reject,
          })
        })
      } else {
        response = await new Promise<pipelineEditUpdateMutation$data>((resolve, reject) => {
          update({
            variables: {
              input: {
                ...common,
                id,
              },
            },
            onCompleted: resolve,
            onError: reject,
          })
        })
      }

      const result = 'pipelineCreate' in response ? response.pipelineCreate : response.pipelineUpdate

      if (result.errors?.length) {
        const faults = result.errors
          .filter((error): error is { message: string } => error.message !== undefined)
          .map((error) => error.message)

        setErrors(faults)
        return
      }

      if (result.pipeline) {
        onComplete?.(result.pipeline)
        setErrors([])
      }
    },
    [create, update, value?.id, onComplete]
  )

  const onSubmit: SubmitHandler<PipelineEditInput> = React.useCallback(
    async (data) => {
      await upsert(data)
    },
    [upsert]
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
        form.reset(value)
        setErrors([])
      },
    }),
    [form, onSubmit, value]
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
            <Input.TextArea rows={4} />
          </Input.Root>
          <Form.Feedback type="error">{form.formState.errors.description?.message}</Form.Feedback>
        </Form.Group>

        <Form.Group {...form.register('definition')} className="grow" error={!!form.formState.errors.definition}>
          <Form.Label>Definition</Form.Label>

          <div className="grow">
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
