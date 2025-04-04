import next from '@next/eslint-plugin-next'
import relay from 'eslint-plugin-relay'

/** @type {Awaited<import('typescript-eslint').Config>} */
export default [
  {
    files: ['**/*.ts', '**/*.tsx'],
    plugins: {
      '@next/next': next,
      relay: relay,
    },
    rules: {
      ...next.configs.recommended.rules,
      ...next.configs['core-web-vitals'].rules,

      ...relay.configs.recommended.rules,

      // TypeError: context.getAncestors is not a function
      '@next/next/no-duplicate-head': 'off',
    },
  },
]
