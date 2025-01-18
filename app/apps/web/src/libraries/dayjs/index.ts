import type { Duration } from 'dayjs/plugin/duration'
import dayjs from 'dayjs'
import duration from 'dayjs/plugin/duration'
import relative from 'dayjs/plugin/relativeTime'

dayjs.extend(duration)
dayjs.extend(relative)

export const toPrettyDuration = (duration: Duration) => {
  const hours = Math.floor(duration.asHours())
  const minutes = Math.floor(duration.minutes())
  const seconds = Math.floor(duration.seconds())

  const parts: string[] = []

  if (hours > 0) {
    parts.push(`${hours}h`)
  }

  if (minutes > 0) {
    parts.push(`${minutes}m`)
  }

  if (seconds > 0) {
    parts.push(`${seconds}s`)
  }

  return parts.length > 0 ? parts.join(' ') : '0s'
}
