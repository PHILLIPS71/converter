'use client'

import type { SubmitHandler } from 'react-hook-form'
import React from 'react'
import { Alert, Form, Input } from '@giantnodes/react'
import { zodResolver } from '@hookform/resolvers/zod'
import { IconAlertCircleFilled } from '@tabler/icons-react'
import { useForm } from 'react-hook-form'
import { ConnectionHandler, graphql, useMutation } from 'react-relay'
import { ROOT_ID } from 'relay-runtime'
import * as z from 'zod'

import type { libraryCreateMutation, libraryCreateMutation$data } from '~/__generated__/libraryCreateMutation.graphql'

export type LibraryCreateRef = {
  submit: () => void
  reset: () => void
}

export type LibraryCreatePayload = NonNullable<libraryCreateMutation$data['libraryCreate']['library']>

type LibraryCreateInput = z.infer<typeof LibraryCreateSchema>

type LibraryCreateProps = {
  onComplete?: (payload: LibraryCreatePayload) => void
  onLoadingChange?: (isLoading: boolean) => void
}

const MUTATION = graphql`
  mutation libraryCreateMutation($input: LibraryCreateInput!) {
    libraryCreate(input: $input) {
      library {
        slug
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

const LibraryCreateSchema = z.object({
  name: z.string().trim().min(1, { message: 'the name cannot be left empty' }).max(128, { message: 'too many chars' }),
  path: z.string().trim().min(1, { message: 'the path cannot be left empty' }),
})

const LibraryCreate = React.forwardRef<LibraryCreateRef, LibraryCreateProps>((props, ref) => {
  const { onComplete, onLoadingChange } = props

  const [errors, setErrors] = React.useState<string[]>([])
  const [commit, isLoading] = useMutation<libraryCreateMutation>(MUTATION)

  const form = useForm<LibraryCreateInput>({ resolver: zodResolver(LibraryCreateSchema) })

  const onSubmit: SubmitHandler<LibraryCreateInput> = React.useCallback(
    (data) => {
      commit({
        variables: {
          input: {
            name: data.name,
            path: data.path,
          },
        },
        onCompleted: (payload) => {
          if (payload.libraryCreate.errors != null) {
            const faults = payload.libraryCreate.errors
              .filter((error): error is { message: string } => error.message !== undefined)
              .map((error) => error.message)

            setErrors(faults)

            return
          }

          if (payload.libraryCreate.library) onComplete?.(payload.libraryCreate.library)

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
      },
    }),
    [form, onSubmit]
  )

  return (
    <Form.Root onSubmit={form.handleSubmit(onSubmit)}>
      <div className="flex flex-col gap-y-3">
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
        <div className="flex flex-row gap-3 flex-wrap md:flex-nowrap">
          <Form.Group {...form.register('name')} error={!!form.formState.errors.name}>
            <Form.Label>Name</Form.Label>
            <Input.Root>
              <Input.Text type="text" />
            </Input.Root>
            <Form.Feedback type="error">{form.formState.errors.name?.message}</Form.Feedback>
          </Form.Group>
        </div>
        <Form.Group {...form.register('path')} error={!!form.formState.errors.path}>
          <Form.Label>Folder</Form.Label>
          <Input.Root>
            <Input.Text type="text" />
          </Input.Root>
          <Form.Feedback type="error">{form.formState.errors.path?.message}</Form.Feedback>
        </Form.Group>
      </div>
    </Form.Root>
  )
})

export default LibraryCreate
