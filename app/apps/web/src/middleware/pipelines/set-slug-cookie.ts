import type { NextRequest, NextResponse } from 'next/server'
import { cookies } from 'next/headers'

import type { NextFunction } from '~/middleware/middleware'
import { LibraryService } from '~/domains/libraries/service'
import { Middleware } from '~/middleware/middleware'
import { isSuccess } from '~/utilities/result-pattern'

export class SetSlugCookiePipeline extends Middleware {
  applicable(pathname: string): boolean {
    const pattern = /^\/explore\/[^/]+(?:\/.*)?$/
    return pattern.test(pathname)
  }

  async handle(request: NextRequest, response: NextResponse, next: NextFunction): Promise<NextResponse> {
    const pattern = /^\/explore\/([^/]+)/

    const slug = RegExp(pattern).exec(request.nextUrl.pathname)?.at(1)
    if (!slug) {
      return next(response)
    }

    const result = await LibraryService.set(slug)
    if (isSuccess(result) && result.value != null) {
      const store = await cookies()

      for (const cookie of store.getAll()) {
        response.cookies.set({ ...cookie })
      }
    }

    return next(response)
  }
}
