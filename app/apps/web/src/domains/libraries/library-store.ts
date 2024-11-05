'server only'

import { cookies } from 'next/headers'

const STORAGE_KEY = 'library:id'

/**
 * Retrieves the current library ID from cookies
 *
 * @returns Promise that resolves to the library ID if found, null otherwise
 */
export const get = async (): Promise<string | null> => {
  const store = await cookies()
  return store.get(STORAGE_KEY)?.value ?? null
}

/**
 * Stores a library ID in cookies or removes it if null is provided
 *
 * @param id - The library ID to store, or null to clear the selection
 */
export const set = async (id: string | null): Promise<void> => {
  const store = await cookies()

  if (id != null) {
    store.set(STORAGE_KEY, id)
  } else {
    store.delete(STORAGE_KEY)
  }
}
