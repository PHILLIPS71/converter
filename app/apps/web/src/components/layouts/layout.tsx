'use client'

import React from 'react'
import { cn } from '@giantnodes/react'

type LayoutRootProps = React.PropsWithChildren

type LayoutContainerProps = React.PropsWithChildren

type LayoutSectionProps = React.PropsWithChildren & {
  size?: 'lg' | 'sm'
}

const Root: React.FC<LayoutRootProps> = ({ children }) => (
  <main className="min-h-screen flex flex-row w-full">{children}</main>
)

const Container: React.FC<LayoutContainerProps> = ({ children }) => (
  <div className="relative min-h-screen grow">
    <div className="relative flex flex-col h-svh overflow-hidden sm:overflow-auto">{children}</div>
  </div>
)

const Section: React.FC<LayoutSectionProps> = ({ children, size }) => {
  const width = React.useMemo<string | null>(() => {
    switch (size) {
      case 'lg':
        return 'max-w-6xl mx-auto '

      case 'sm':
        return 'max-w-2xl mx-auto'

      default:
        return ''
    }
  }, [size])

  return (
    <div className="flex flex-col grow w-full overflow-auto">
      <div className={cn(width, 'p-4 h-full w-full')}>{children}</div>
    </div>
  )
}

export { Root, Container, Section }
