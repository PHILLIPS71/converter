import type { NextRequest } from 'next/server'
import { NextResponse } from 'next/server'

export type NextFunction = (response: NextResponse) => Promise<NextResponse>

/**
 * Abstract class representing request middleware in the application.
 */
export abstract class Middleware {
  /**
   * Determines if the middleware is applicable to be run for the given pathname.
   *
   * @param pathname - The pathname to check.
   * @returns True if the middleware should be applied, false otherwise.
   */
  abstract applicable(pathname: string): boolean

  /**
   * Handles the middleware logic for the incoming request.
   *
   * @param request - The incoming request object.
   * @param response - The response object.
   * @param next - The function to call the next middleware.
   * @returns A promise that resolves to the next response.
   */
  abstract handle(request: NextRequest, response: NextResponse, next: NextFunction): Promise<NextResponse>

  /**
   * Safely redirects the user to the specified route so to not cause redirect loops. If the current pathname does
   * not match the target route, it performs the redirect. Otherwise, forwards the response.
   *
   * @param request - The incoming request object.
   * @param response - The response object.
   * @param route - The target route to redirect to.
   * @returns The modified response object with the redirect and optional headers appended.
   */
  redirect(request: NextRequest, response: NextResponse, route: URL): NextResponse {
    if (request.nextUrl.pathname === route.pathname) {
      return NextResponse.next(response)
    }

    return NextResponse.redirect(route, { ...response, status: 307 })
  }

  /**
   * Rewrites the request to an internal route while preserving the original URL.
   *
   * @param request - The incoming request.
   * @param response - The response object.
   * @param route - The target route to rewrite to.
   * @returns The rewritten response or forwards if paths match.
   */
  rewrite(request: NextRequest, response: NextResponse, route: URL): NextResponse {
    if (request.nextUrl.pathname === route.pathname) {
      return NextResponse.next(response)
    }

    return NextResponse.rewrite(route, response)
  }
}
