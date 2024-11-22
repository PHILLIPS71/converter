import dayjs from 'dayjs'
import duration, { Duration } from 'dayjs/plugin/duration'

dayjs.extend(duration)

export const toPrettyDuration = (duration: Duration) => {
  const hours = Math.floor(duration.asHours())
  const minutes = Math.floor(duration.minutes())
  const seconds = Math.floor(duration.seconds())

  if (minutes === 0) {
    return `${seconds}s`
  }

  if (hours === 0) {
    return `${minutes}m ${seconds}s`
  }

  return `${hours}h ${minutes}m ${seconds}s`
}
