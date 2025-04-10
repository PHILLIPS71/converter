'use client'

import React from 'react'
import { Button, Navigation } from '@giantnodes/react'
import { IconMenu } from '@tabler/icons-react'

import { useLayout } from '~/components/layouts/use-layout'
import SearchWidget from '~/components/layouts/widgets/search-widget'

const Root: React.FC = () => {
  const { isSidebarOpen, setSidebarOpen } = useLayout()

  return (
    <Navigation.Root className="justify-end" orientation="horizontal" isBordered>
      <Navigation.Segment className="flex md:hidden">
        <Button color="neutral" onPress={() => setSidebarOpen(!isSidebarOpen)} size="xs">
          <IconMenu size={20} strokeWidth={1} />
        </Button>
      </Navigation.Segment>

      <Navigation.Segment className="hidden md:block">
        <Navigation.Item variant="none">
          <SearchWidget />
        </Navigation.Item>
      </Navigation.Segment>
    </Navigation.Root>
  )
}

export { Root }
