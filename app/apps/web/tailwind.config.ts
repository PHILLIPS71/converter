import type { Config } from 'tailwindcss'
import { giantnodes } from '@giantnodes/theme'

const config: Config = {
  content: ['./src/**/*.{ts,tsx}', './node_modules/@giantnodes/theme/dist/**/*.{js,ts,jsx,tsx}'],
  plugins: [giantnodes()],
  darkMode: 'class',
}

export default config
