'use client'

import React from 'react'

const SCREEN_SIZE = {
  xs: 0,
  sm: 500,
  md: 750,
  lg: 1000,
  xl: 1200,
  '2xl': 1400,
} as const

type ScreenSize = keyof typeof SCREEN_SIZE

/**
 * A hook for responsive design based on screen size.
 *
 * @param target - Optional target screen size to compare against
 * @returns An object containing the current screen size and a boolean indicating if the screen is at least the target size
 */
export const useScreenSize = (target: ScreenSize | undefined = undefined) => {
  const [size, setSize] = React.useState<ScreenSize>('2xl')

  const sizes = React.useMemo(() => Object.entries(SCREEN_SIZE).sort(([, a], [, b]) => b - a), [])

  const isTargetSize = React.useMemo(() => {
    if (!target) return false

    const keys = sizes.map(([name]) => name)
    return keys.indexOf(size) >= keys.indexOf(target)
  }, [size, sizes, target])

  React.useLayoutEffect(() => {
    const resize = () => {
      const width = window.innerWidth

      for (const [size, breakpoint] of sizes) {
        if (width >= breakpoint) {
          setSize(size as ScreenSize)
          break
        }
      }
    }

    resize()

    window.addEventListener('resize', resize)
    return () => window.removeEventListener('resize', resize)
  }, [sizes])

  return {
    size,
    isTargetSize,
  }
}
