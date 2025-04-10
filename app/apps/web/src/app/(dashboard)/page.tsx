'use client'

import { Card, Typography } from '@giantnodes/react'

import { useLibrary } from '~/domains/libraries/use-library.hook'

const DashboardPage = () => {
  const { library } = useLibrary()

  return (
    <Card.Root>
      <Typography.Paragraph>Library: {library?.name}</Typography.Paragraph>
    </Card.Root>
  )
}

export default DashboardPage
