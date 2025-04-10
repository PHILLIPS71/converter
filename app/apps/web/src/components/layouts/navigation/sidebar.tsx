'use client'

import type { NavigationProps } from '@giantnodes/react'
import React from 'react'
import Image from 'next/image'
import { usePathname } from 'next/navigation'
import { Button, Modal, Navigation } from '@giantnodes/react'
import { IconFolders, IconGauge, IconLayoutSidebar, IconPlug } from '@tabler/icons-react'
import { useFragment } from 'react-relay'
import { graphql } from 'relay-runtime'

import type { sidebarFragment_query$key } from '~/__generated__/sidebarFragment_query.graphql'
import { useLayout } from '~/components/layouts/use-layout'
import LibraryWidget from '~/components/layouts/widgets/library-widget'
import { useLibrary } from '~/domains/libraries/use-library.hook'

type SidebarProps = NavigationProps & {
  $key: sidebarFragment_query$key
}

const FRAGMENT = graphql`
  fragment sidebarFragment_query on Query {
    ...libraryWidgetFragment_query
  }
`

const Desktop: React.FC<SidebarProps> = ({ $key, ...rest }) => {
  const pathname = usePathname()

  const { library } = useLibrary()
  const { isSidebarOpen, setSidebarOpen } = useLayout()
  const data = useFragment(FRAGMENT, $key)

  const route = pathname.split('/')[1]

  return (
    <Navigation.Root as="aside" orientation="vertical" size="lg" isBordered {...rest}>
      <Navigation.Segment>
        <div className="flex flex-row items-center gap-x-2">
          <Navigation.Brand className="justify-start grow">
            <Image alt="giantnodes logo" height={40} src="/images/giantnodes-logo.png" width={96} priority />
          </Navigation.Brand>

          <Button className="flex sm:hidden" color="neutral" onPress={() => setSidebarOpen(!isSidebarOpen)} size="xs">
            <IconLayoutSidebar size={20} strokeWidth={1} />
          </Button>
        </div>
      </Navigation.Segment>

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

        <Navigation.Item isSelected={route === 'pipelines'}>
          <Navigation.Link className="p-2" href="/pipelines">
            <IconPlug size={20} strokeWidth={1} /> Pipelines
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

const Popover: React.FC<SidebarProps> = ({ $key }) => {
  const { isSidebarOpen, setSidebarOpen } = useLayout()

  return (
    <Modal.Root isOpen={isSidebarOpen} onOpenChange={setSidebarOpen} placement="left" position="none" isDismissable>
      <Modal.Content>
        <div className="flex flex-row h-full">
          <Desktop $key={$key} />
        </div>
      </Modal.Content>
    </Modal.Root>
  )
}

const Root: React.FC<SidebarProps> = ({ $key }) => (
  <>
    <Desktop $key={$key} className="hidden md:flex" />
    <Popover $key={$key} />
  </>
)

export { Root, Desktop }
