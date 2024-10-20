'use client'

import React from 'react'
import Image from 'next/image'
import { Button, Navigation } from '@giantnodes/react'
import { IconBell, IconMenu } from '@tabler/icons-react'

import { useLayout } from '~/components/layouts/use-layout'
import SearchWidget from '~/components/layouts/widgets/search-widget'

const Root: React.FC = () => {
  const { isSidebarOpen, isMobileDevice, setSidebarOpen } = useLayout()

  return (
    <Navigation.Root orientation="horizontal" isBordered>
      <Navigation.Segment className="flex flex-row flex-grow justify-start">
        {isMobileDevice && (
          <Button color="neutral" onPress={() => setSidebarOpen(!isSidebarOpen)} size="xs">
            <IconMenu size={14} strokeWidth={1} />
          </Button>
        )}

        <Navigation.Brand className="justify-start flex-grow">
          <Image alt="giantnodes logo" height={40} src="/images/giantnodes-logo.png" width={96} priority />
        </Navigation.Brand>
      </Navigation.Segment>

      <Navigation.Segment className="hidden md:block">
        <Navigation.Item variant="none">
          <SearchWidget />
        </Navigation.Item>
      </Navigation.Segment>

      <Navigation.Segment>
        <Navigation.Item>
          <Navigation.Trigger>
            <IconBell size={20} strokeWidth={1} />
          </Navigation.Trigger>
        </Navigation.Item>
      </Navigation.Segment>
    </Navigation.Root>
  )
}

export { Root }
