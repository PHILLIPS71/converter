'use client'

import React from 'react'
import { useRouter } from 'next/navigation'
import { Button, Card, Typography } from '@giantnodes/react'

import type { LibraryCreatePayload, LibraryCreateRef } from '~/domains/libraries/library-create'
import LibraryCreate from '~/domains/libraries/library-create'

const LibraryCreatePage = () => {
  const router = useRouter()

  const ref = React.useRef<LibraryCreateRef>(null)
  const [isLoading, setLoading] = React.useState<boolean>(false)

  const onComplete = (payload: LibraryCreatePayload) => {
    router.push(`/explore/${payload.slug}`)
  }

  return (
    <Card.Root>
      <Card.Header>
        <Typography.HeadingLevel>
          <Typography.Heading level={6}>Create a new library</Typography.Heading>
          <Typography.Text size="sm" variant="subtitle">
            Your library will have its own dedicated metrics and control panel. A dashboard will be set up so you can
            easily interact with your new library.
          </Typography.Text>
        </Typography.HeadingLevel>
      </Card.Header>

      <Card.Body>
        <LibraryCreate onComplete={onComplete} onLoadingChange={setLoading} ref={ref} />
      </Card.Body>

      <Card.Footer className="flex items-center justify-end gap-3">
        <Button color="neutral" onPress={() => ref.current?.reset()} size="xs">
          Reset
        </Button>
        <Button isDisabled={isLoading} onPress={() => ref.current?.submit()} size="xs">
          Save
        </Button>
      </Card.Footer>
    </Card.Root>
  )
}

export default LibraryCreatePage
