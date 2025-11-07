import next from '@next/eslint-plugin-next'
import config from 'eslint/config'

export default config.defineConfig({
  files: ['**/*.ts', '**/*.tsx'],
  plugins: {
    '@next/next': next,
  },
  rules: {
    ...next.configs.recommended.rules,
    ...next.configs['core-web-vitals'].rules,

    // TypeError: context.getAncestors is not a function
    '@next/next/no-duplicate-head': 'off',
  },
})
