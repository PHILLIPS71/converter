import { fileURLToPath } from 'url'
import type { NextConfig } from 'next'
import createJiti from 'jiti'

// import env files to validate at build time. Use jiti so we can load .ts files in here.
await createJiti(fileURLToPath(import.meta.url))('./src/env')

/** @type {import("next").NextConfig} */
const config: NextConfig = {
  reactStrictMode: true,

  /** Enables hot reloading for local packages without a build step */
  transpilePackages: ['@t3-oss/env-nextjs', '@t3-oss/env-core'],

  compiler: {
    relay: {
      src: './src',
      language: 'typescript',
      artifactDirectory: 'src/__generated__',
    },
  },

  images: {
    remotePatterns: [
      {
        protocol: 'http',
        hostname: 'localhost',
      },
    ],
  },
}

export default config
