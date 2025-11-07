import type { NextRequest } from 'next/server'
import { NextResponse } from 'next/server'

import { MiddlewarePipeline } from '~/middleware/middleware-pipeline'
import { SetSlugCookiePipeline } from '~/middleware/pipelines'

export const proxy = async (request: NextRequest) => {
  const pipeline = new MiddlewarePipeline().use(new SetSlugCookiePipeline())

  try {
    return await pipeline.execute(request)
  } catch (error) {
    console.error('Proxy Error:', error)
    return NextResponse.next()
  }
}

export const config = {
  matcher: ['/((?!_next/static|_next/image|images|favicon.ico).*)'],
}
