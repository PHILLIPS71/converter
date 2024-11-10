import React from 'react'

type UseInfiniteScrollProps = {
  distance?: number
  onScroll: () => void
}

export const useInfiniteScroll = ({ distance = 250, onScroll }: UseInfiniteScrollProps) => {
  const targetRef = React.useRef<HTMLDivElement>(null)

  React.useLayoutEffect(() => {
    const target = targetRef.current

    if (!target) return

    const options = {
      root: null,
      rootMargin: `0px 0px ${distance}px 0px`,
      threshold: 0.1,
    }

    const observer = new IntersectionObserver((entries) => {
      const [entry] = entries

      if (entry?.isIntersecting) {
        onScroll()
      }
    }, options)

    observer.observe(target)

    return () => observer.disconnect()
  }, [distance, onScroll])

  return [targetRef]
}
