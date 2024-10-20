'use client'

import React from 'react'

type CreateContextOptions = {
  /**
   * If true, the useContext hook will throw an error when the context is undefined.
   * Set to false if you want to support nested contexts.
   *
   * @default true
   */
  strict?: boolean

  /**
   * Custom error message to display when the context is undefined and strict mode is enabled.
   *
   * @default 'useContext: `context` is undefined. Seems you forgot to wrap component within the Provider'
   */
  errorMessage?: string

  /**
   * The display name for the context, useful for debugging.
   */
  name?: string
}

type CreateContextReturn<ContextType, ContextProps> = readonly [
  React.FC<React.PropsWithChildren<ContextProps>>,
  () => ContextType,
]

/**
 * A utility function to create a React context with improved type safety, error handling, and state management.
 *
 * @template ContextType The type of the context value.
 * @template ContextProps The type of the props passed to the Provider component.
 * @param useValue A hook that returns the value to be provided by the context.
 * @param options Configuration options for creating the context.
 * @returns A readonly tuple containing the Provider component and a custom useContext hook.
 *
 * @example
 * type SessionContextType {
 *   id: string
 * }
 *
 * type UseSessionProps {
 *   id: User
 * }
 *
 * const useSessionValue = ({ id }: UseSessionProps) => {
 *   const [id, setId] = React.useState<string>(id);
 *   return { id };
 * };
 *
 * const [SessionProvider, useSession] = create<SessionContextType, UseSessionProps>(useSessionValue, {
 *   name: 'SessionContext',
 * });
 *
 * // In the root component:
 * <SessionProvider id="cbf3d503-4af5-4502-8a36-9b6b99a9364d">
 *   <App />
 * </SessionProvider>
 *
 * // In a consumer component:
 * const { id } = useSession();
 */
export const create = <ContextType, ContextProps extends object>(
  useValue: (props: ContextProps) => ContextType,
  options: CreateContextOptions
): CreateContextReturn<ContextType, ContextProps> => {
  const {
    strict = true,
    errorMessage = 'useContext: `context` is undefined. Seems you forgot to wrap component within the Provider',
    name,
  } = options

  const Context = React.createContext<ContextType | undefined>(undefined)

  Context.displayName = name

  const useContext = (): ContextType => {
    const context = React.useContext(Context)

    if (!context && strict) {
      const error = new Error(errorMessage)
      error.name = 'ContextError'
      throw error
    }

    return context as ContextType
  }

  const Provider: React.FC<React.PropsWithChildren<ContextProps>> = ({ children, ...props }) => {
    const value = useValue(props as ContextProps)

    return <Context.Provider value={value}>{children}</Context.Provider>
  }

  return [Provider, useContext] as const
}
