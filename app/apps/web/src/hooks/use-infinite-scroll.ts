import React from 'react'
import { useDebounce } from 'use-debounce'

type UseInfiniteScrollProps = {
  distance?: number
  onScroll: () => void
}

export const useInfiniteScroll = ({ distance = 250, onScroll }: UseInfiniteScrollProps) => {
  const targetRef = React.useRef<HTMLDivElement>(null)

  const [callback] = useDebounce(() => onScroll(), 300, {
    leading: false,
    trailing: true,
  })

  React.useLayoutEffect(() => {
    const target = targetRef.current

    if (!target) return

    const options = {
      root: null,
      rootMargin: `0px 0px ${distance}px 0px`,
      threshold: 0,
    }

    const observer = new IntersectionObserver((entries) => {
      const [entry] = entries

      if (entry?.isIntersecting) {
        callback()
      }
    }, options)

    observer.observe(target)

    return () => observer.disconnect()
  }, [distance, callback])

  return [targetRef]
}
