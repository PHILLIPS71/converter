'use client'

import React from 'react'
import { usePathname } from 'next/navigation'
import { Navigation } from '@giantnodes/react'
import { IconGauge } from '@tabler/icons-react'

const Root: React.FC = () => {
  const router = usePathname()

  const route = router.split('/')[1]

  return (
    <Navigation.Root orientation="vertical" size="lg" isBordered>
      <Navigation.Segment>
        <Navigation.Item isSelected={route === ''}>
          <Navigation.Link className="p-2" href="/">
            <IconGauge size={20} strokeWidth={1} /> Dashboard
          </Navigation.Link>
        </Navigation.Item>
      </Navigation.Segment>
    </Navigation.Root>
  )
}

export { Root }
