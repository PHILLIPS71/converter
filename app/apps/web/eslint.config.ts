import base, { environments } from '@giantnodes/eslint-config/base'
import nextjs from '@giantnodes/eslint-config/next'
import react from '@giantnodes/eslint-config/react'
import config from 'eslint/config'

export default config.defineConfig(
  {
    ignores: ['.next/**', 'src/__generated__/**'],
  },
  ...base,
  ...react,
  ...nextjs,
  ...environments
)
