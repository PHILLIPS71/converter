import react from 'eslint-plugin-react'
import hooks from 'eslint-plugin-react-hooks'
import config from 'eslint/config'

export default config.defineConfig({
  files: ['**/*.ts', '**/*.tsx'],
  plugins: {
    react: react,
    // https://github.com/facebook/react/pull/34994
    'react-hooks': hooks as any,
  },
  settings: {
    react: {
      version: 'detect',
    },
  },
  rules: {
    ...react.configs.recommended.rules,
    ...hooks.configs['recommended-latest'].rules,

    'react/self-closing-comp': 'error',
    'react/jsx-sort-props': ['error', { noSortAlphabetically: false, shorthandLast: true }],
    'react/function-component-definition': ['error', { namedComponents: 'arrow-function' }],
  },
  languageOptions: {
    globals: {
      React: 'writable',
    },
  },
})
