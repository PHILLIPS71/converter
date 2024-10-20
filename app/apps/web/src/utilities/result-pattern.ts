/**
 * Represents the result of an operation that can either succeed or fail.
 *
 * @template TValue The type of the success value
 * @template TError The type of the error value
 */
export type Result<TValue, TError> =
  | { readonly kind: 'success'; readonly value: TValue }
  | { readonly kind: 'failure'; readonly error: TError }

/**
 * Creates a success result.
 *
 * @returns A Result representing a successful operation with no value
 */
export function success(): Result<void, never>

/**
 * Creates a success result with a value.
 *
 * @template T The type of the success value
 * @param value The success value
 * @returns A Result representing a successful operation with a value
 */
export function success<T>(value: T): Result<T, never>
export function success<T>(value?: T): Result<T | void, never> {
  return {
    kind: 'success',
    value: value as T | void,
  }
}

/**
 * Creates a failure result.
 *
 * @returns A Result representing a failed operation with no error details
 */
export function failure(): Result<never, void>

/**
 * Creates a failure result with an error.
 * @template E The type of the error value
 * @param error The error value
 * @returns A Result representing a failed operation with an error
 */
export function failure<E>(error: E): Result<never, E>
export function failure<E>(error?: E): Result<never, E | void> {
  return {
    kind: 'failure',
    error: error as E | void,
  }
}

/**
 * Type guard to check if a Result is a success.
 *
 * @template TValue The type of the success value
 * @template TError The type of the error value
 * @param result The Result to check
 * @returns True if the Result is a success, false otherwise
 */
export const isSuccess = <TValue, TError>(
  result: Result<TValue, TError>
): result is Extract<Result<TValue, TError>, { kind: 'success' }> => result.kind === 'success'

/**
 * Type guard to check if a Result is a failure.
 *
 * @template TValue The type of the success value
 * @template TError The type of the error value
 * @param result The Result to check
 * @returns True if the Result is a failure, false otherwise
 */
export const isFailure = <TValue, TError>(
  result: Result<TValue, TError>
): result is Extract<Result<TValue, TError>, { kind: 'failure' }> => result.kind === 'failure'
