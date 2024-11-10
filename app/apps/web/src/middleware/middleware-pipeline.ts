import type { NextRequest } from 'next/server'
import { NextResponse } from 'next/server'

import type { Middleware } from '~/middleware/middleware'

export class MiddlewarePipeline {
  private readonly middlewares: Middleware[] = []
  private readonly response: NextResponse = NextResponse.next()

  /**
   * Adds a middleware to the pipeline.
   *
   * @param middleware - The middleware to add.
   * @returns The current instance for method chaining.
   */
  use(middleware: Middleware): this {
    this.middlewares.push(middleware)
    return this
  }

  /**
   * Executes the middleware pipeline for a given request.
   *
   * @param request - The incoming request.
   * @returns A promise resolving to the final response after processing all middleware.
   */
  async execute(request: NextRequest): Promise<NextResponse> {
    let index = 0

    // filter middleware based on their applicability to the current pathname
    const middlewares = this.middlewares.filter((middleware) => middleware.applicable(request.nextUrl.pathname))

    /**
     * Recursive function to process the next middleware in the pipeline.
     *
     * @param response - The current response object.
     * @returns A promise resolving to the response after processing the middleware.
     */
    const next = async (response: NextResponse): Promise<NextResponse> => {
      if (index < middlewares.length) {
        const middleware = middlewares[index++]
        if (middleware == undefined) {
          return response
        }

        // execute the current middleware and pass the 'next' function for potential chaining
        response = await middleware.handle(request, response, next)
      }

      return response
    }

    return next(this.response)
  }
}
