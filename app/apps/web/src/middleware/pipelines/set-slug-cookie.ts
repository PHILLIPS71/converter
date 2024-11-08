import type { NextRequest, NextResponse } from 'next/server'

import type { NextFunction } from '~/middleware/middleware'
import * as LibraryStore from '~/domains/libraries/library-store'
import { Middleware } from '~/middleware/middleware'
import { isSuccess } from '~/utilities/result-pattern'

export class SetSlugCookiePipeline extends Middleware {
  applicable(pathname: string): boolean {
    const pattern = /^\/library\/[^/]+(?:\/.*)?$/
    return pattern.test(pathname)
  }

  async handle(request: NextRequest, response: NextResponse, next: NextFunction): Promise<NextResponse> {
    const pattern = /^\/library\/([^/]+)/

    const slug = RegExp(pattern).exec(request.nextUrl.pathname)?.at(1)
    if (!slug) {
      return next(response)
    }

    const result = await LibraryStore.set(slug)
    if (isSuccess(result) && result.value != null) {
      response.cookies.set(result.value)
    }

    return next(response)
  }
}
