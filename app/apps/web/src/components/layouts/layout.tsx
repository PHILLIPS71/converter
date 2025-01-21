'use client'

import React from 'react'

type LayoutRootProps = React.PropsWithChildren

type LayoutContainerProps = React.PropsWithChildren

type LayoutSectionProps = React.PropsWithChildren & {
  slim?: boolean
}

const Root: React.FC<LayoutRootProps> = ({ children }) => (
  <main className="relative min-h-screen flex-grow">{children}</main>
)

const Container: React.FC<LayoutContainerProps> = ({ children }) => (
  <div className="relative flex flex-col h-svh overflow-hidden sm:overflow-auto">{children}</div>
)

const Section: React.FC<LayoutSectionProps> = ({ children, slim }) => (
  <div className="flex flex-col flex-grow w-full overflow-auto p-4">
    <div className={slim ? 'max-w-6xl mx-auto w-full' : 'w-full'}>{children}</div>
  </div>
)

export { Root, Container, Section }
