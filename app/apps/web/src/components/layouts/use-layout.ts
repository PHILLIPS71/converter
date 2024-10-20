'use client'

import React from 'react'

import { useScreenSize } from '~/hooks/use-screen-size'
import { create } from '~/utilities/create-context'

type UseLayoutProps = {
  isSidebarOpen?: boolean
}

type LayoutContextType = ReturnType<typeof useLayoutValue>

const useLayoutValue = (props: UseLayoutProps) => {
  const { isTargetSize: isMobileDevice } = useScreenSize('sm')
  const [isSidebarOpen, setSidebarOpen] = React.useState<boolean>(props.isSidebarOpen ?? true)

  React.useEffect(() => {
    setSidebarOpen(!isMobileDevice)
  }, [isMobileDevice])

  return {
    isMobileDevice,
    isSidebarOpen,
    setSidebarOpen,
  }
}

export const [LayoutProvider, useLayout] = create<LayoutContextType, UseLayoutProps>(useLayoutValue, {
  name: 'LayoutContext',
  strict: true,
  errorMessage: 'useLayout: `context` is undefined. Seems you forgot to wrap component within the <LayoutProvider />',
})
