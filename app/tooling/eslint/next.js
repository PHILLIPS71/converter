import next from '@next/eslint-plugin-next'
import compiler from 'eslint-plugin-react-compiler'

/** @type {Awaited<import('typescript-eslint').Config>} */
export default [
  {
    files: ['**/*.ts', '**/*.tsx'],
    plugins: {
      '@next/next': next,
      'react-compiler': compiler,
    },
    settings: {
      react: {
        version: 'detect',
      },
    },
    rules: {
      ...next.configs.recommended.rules,
      ...next.configs['core-web-vitals'].rules,

      // TypeError: context.getAncestors is not a function
      '@next/next/no-duplicate-head': 'off',

      'react-compiler/react-compiler': 'error',
    },
  },
]
