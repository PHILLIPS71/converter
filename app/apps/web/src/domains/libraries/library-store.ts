'server only'

import { ResponseCookie } from 'next/dist/compiled/@edge-runtime/cookies'
import { cookies } from 'next/headers'

import { Result, success } from '~/utilities/result-pattern'

const STORAGE_KEY = 'library:slug'

/**
 * Retrieves the current library slug from cookies
 *
 * @returns Promise that resolves to the library slug if found, null otherwise
 */
export const get = async (): Promise<string | null> => {
  const store = await cookies()
  return store.get(STORAGE_KEY)?.value ?? null
}

/**
 * Stores a library slug in cookies or removes it if null is provided
 *
 * @param slug - The library slug to store, or null to clear the selection
 */
export const set = async (slug: string | null): Promise<Result<ResponseCookie | null, never>> => {
  const store = await cookies()

  if (slug != null) {
    store.set(STORAGE_KEY, slug)
  } else {
    store.delete(STORAGE_KEY)
  }

  return success(store.get(STORAGE_KEY) ?? null)
}
