import react from 'eslint-plugin-react'
import compiler from 'eslint-plugin-react-compiler'
import hooks from 'eslint-plugin-react-hooks'

/** @type {Awaited<import('typescript-eslint').Config>} */
export default [
  {
    files: ['**/*.ts', '**/*.tsx'],
    plugins: {
      react: react,
      'react-hooks': hooks,
    },
    settings: {
      react: {
        version: 'detect',
      },
    },
    rules: {
      ...react.configs['jsx-runtime'].rules,
      ...hooks.configs.recommended.rules,

      'react/self-closing-comp': 'error',
      'react/jsx-sort-props': ['error', { noSortAlphabetically: false, shorthandLast: true }],
      'react/function-component-definition': ['error', { namedComponents: 'arrow-function' }],

      'react-compiler/react-compiler': 'error',
    },
    languageOptions: {
      globals: {
        React: 'writable',
      },
    },
  },
]
