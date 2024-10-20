import base, { environments } from '@giantnodes/eslint-config/base'
import nextjs from '@giantnodes/eslint-config/next'
import react from '@giantnodes/eslint-config/react'

/** @type {import('typescript-eslint').Config} */
export default [
  {
    ignores: ['.next/**', 'src/__generated__/**'],
  },
  ...base,
  ...react,
  ...nextjs,
  ...environments,
]
