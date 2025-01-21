'use client'

import React from 'react'
import { Button, Navigation } from '@giantnodes/react'
import { IconMenu } from '@tabler/icons-react'

import { useLayout } from '~/components/layouts/use-layout'

const Root: React.FC = () => {
  const { isSidebarOpen, setSidebarOpen } = useLayout()

  return (
    <Navigation.Root orientation="horizontal" isBordered>
      <Navigation.Segment className="flex sm:hidden">
        <Button color="neutral" onPress={() => setSidebarOpen(!isSidebarOpen)} size="xs">
          <IconMenu size={20} strokeWidth={1} />
        </Button>
      </Navigation.Segment>
    </Navigation.Root>
  )
}

export { Root }
