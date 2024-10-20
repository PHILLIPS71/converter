'use client'

import React from 'react'
import { cn, Modal } from '@giantnodes/react'

import { LayoutProvider, useLayout } from '~/components/layouts/use-layout'

type LayoutRootProps = React.PropsWithChildren & {
  navbar?: React.ReactNode
  sidebar?: React.ReactNode
}

type LayoutSectionProps = React.PropsWithChildren & {
  sidebar?: React.ReactNode
}

type LayoutContentProps = React.PropsWithChildren & {
  slim?: boolean
}

const Root: React.FC<LayoutRootProps> = ({ children, navbar }) => (
  <LayoutProvider>
    <div className="h-screen flex flex-col">
      {navbar}
      {children}
    </div>
  </LayoutProvider>
)

const Section: React.FC<LayoutSectionProps> = ({ children, sidebar }) => {
  const { isSidebarOpen, setSidebarOpen, isMobileDevice } = useLayout()

  return (
    <div className="flex flex-row flex-grow overflow-x-hidden">
      {!isMobileDevice && sidebar}

      {isMobileDevice && (
        <Modal.Root
          className="mt-16"
          isOpen={isSidebarOpen}
          onOpenChange={setSidebarOpen}
          placement="left"
          position="none"
          isDismissable
        >
          <Modal.Content>
            <div className="flex flex-row h-full">{sidebar}</div>
          </Modal.Content>
        </Modal.Root>
      )}

      <div className="flex flex-col flex-grow">{children}</div>
    </div>
  )
}

const Content: React.FC<LayoutContentProps> = ({ children, slim }) => (
  <main className={cn('flex-grow py-4 px-5 overflow-y-auto', slim ? 'max-w-6xl mx-auto w-full' : '')}>{children}</main>
)

export { Root, Section, Content }
