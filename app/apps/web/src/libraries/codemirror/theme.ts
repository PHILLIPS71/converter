import { HighlightStyle, syntaxHighlighting } from '@codemirror/language'
import { EditorView } from '@codemirror/view'
import { tags as t } from '@lezer/highlight'

const theme = EditorView.theme(
  {
    '&': {
      height: '100%',
      backgroundColor: 'var(--color-foreground)',
      borderRadius: '0.375rem',
      borderWidth: '1px',
      borderColor: 'var(--color-partition)',
    },
    '[data-error="true"] &': {
      borderColor: 'var(--color-danger)',
    },

    '.cm-scroller': {
      borderRadius: '0.375rem',
    },
    '.cm-gutters': {
      backgroundColor: 'var(--color-foreground)',
    },
    '.cm-activeLine': {
      backgroundColor: 'var(--color-brand-500) / 0.15',
    },
    '.cm-activeLineGutter': {
      backgroundColor: 'var(--color-brand-500) / 0.15',
    },

    '&.cm-focused': {
      outlineStyle: 'dashed',
      outlineWidth: '1px',
      outlineOffset: '2px',
      outlineColor: 'var(--color-partition)',
    },

    '.cm-line': {
      color: 'var(--color-info-700)',
    },
    '.cm-foldPlaceholder': {
      backgroundColor: 'var(--color-foreground)',
      borderColor: 'var(--color-partition)',
      color: 'var(--color-content)',
      margin: '0 5px',
      padding: '0 5px',
    },
  },
  { dark: true }
)

const highlight = HighlightStyle.define([
  { tag: [t.name], color: 'var(--color-brand)' },
  { tag: [t.propertyName], color: 'var(--color-brand)' },
  { tag: [t.string], color: 'var(--color-info-700)' },
  { tag: [t.separator], color: '#fff' },
  { tag: [t.comment], color: 'var(--color-shark-700)' },
])

export const giantnodes = [theme, syntaxHighlighting(highlight)]
