import { deleteCookie, getCookie, setCookie } from 'cookies-next'

import type { Result } from '~/utilities/result-pattern'
import { env } from '~/env'
import { failure, success } from '~/utilities/result-pattern'

type CookieStrategy = {
  get(name: string): Promise<string | null>
  set(name: string, value: string): Promise<void>
  delete(name: string): Promise<void>
}

const IS_SERVER = typeof window === 'undefined'

const COOKIE_OPTIONS = {
  httpOnly: false,
  secure: env.NODE_ENV === 'production',
  sameSite: 'strict' as const,
  path: '/',
}

class ServerCookieStrategy implements CookieStrategy {
  private static async getStore() {
    const { cookies } = await import('next/headers')
    return await cookies()
  }

  async get(name: string) {
    const store = await ServerCookieStrategy.getStore()
    return store.get(name)?.value ?? null
  }

  async set(name: string, value: string) {
    const store = await ServerCookieStrategy.getStore()
    store.set({ name, value, ...COOKIE_OPTIONS })
  }

  async delete(name: string) {
    const store = await ServerCookieStrategy.getStore()
    store.delete(name)
  }
}

class ClientCookieStrategy implements CookieStrategy {
  async get(name: string) {
    const value = await getCookie(name)
    return value ? String(value) : null
  }

  async set(name: string, value: string) {
    await setCookie(name, value, COOKIE_OPTIONS)
  }

  async delete(name: string) {
    await deleteCookie(name)
  }
}

export class CookieAdapter {
  private static getStrategy(): CookieStrategy {
    return IS_SERVER ? new ServerCookieStrategy() : new ClientCookieStrategy()
  }

  static async get(name: string): Promise<Result<string | null, string>> {
    try {
      const strategy = this.getStrategy()
      const value = await strategy.get(name)
      return success(value)
    } catch (error) {
      return failure(`an unexpected error occurred: ${error instanceof Error ? error.message : String(error)}`)
    }
  }

  static async set(name: string, value: string | null): Promise<Result<string | null, string>> {
    try {
      const strategy = this.getStrategy()

      if (value?.trim().length) {
        await strategy.set(name, value)
      } else {
        await strategy.delete(name)
      }

      return success(value)
    } catch (error) {
      return failure(`an unexpected error occurred: ${error instanceof Error ? error.message : String(error)}`)
    }
  }
}
