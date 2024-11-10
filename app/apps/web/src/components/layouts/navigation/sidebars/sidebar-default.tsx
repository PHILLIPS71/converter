'use client'

import React from 'react'
import { usePathname } from 'next/navigation'
import { Navigation } from '@giantnodes/react'
import { IconFolders, IconGauge } from '@tabler/icons-react'
import { useFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { sidebarDefaultFragment$key } from '~/__generated__/sidebarDefaultFragment.graphql'
import LibraryWidget from '~/components/layouts/widgets/library-widget'
import { useLibrary } from '~/domains/libraries/use-library.hook'

const FRAGMENT = graphql`
  fragment sidebarDefaultFragment on Query {
    ...libraryWidgetFragment
  }
`

type SidebarDefaultProps = {
  $key: sidebarDefaultFragment$key
}

const Root: React.FC<SidebarDefaultProps> = ({ $key }) => {
  const router = usePathname()
  const { library } = useLibrary()

  const data = useFragment<sidebarDefaultFragment$key>(FRAGMENT, $key)

  const route = router.split('/')[1]

  return (
    <Navigation.Root orientation="vertical" size="lg" isBordered>
      <Navigation.Segment>
        <Navigation.Item isSelected={route === ''}>
          <Navigation.Link className="p-2" href="/">
            <IconGauge size={20} strokeWidth={1} /> Dashboard
          </Navigation.Link>
        </Navigation.Item>

        <Navigation.Item isSelected={route === 'explore'}>
          <Navigation.Link className="p-2" href={`/explore/${library?.slug}`}>
            <IconFolders size={20} strokeWidth={1} /> File Explorer
          </Navigation.Link>
        </Navigation.Item>
      </Navigation.Segment>

      <Navigation.Segment className="mt-auto">
        <Navigation.Item>
          <LibraryWidget $key={data} />
        </Navigation.Item>
      </Navigation.Segment>
    </Navigation.Root>
  )
}

export { Root }
