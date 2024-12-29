import { HighlightStyle, syntaxHighlighting } from '@codemirror/language'
import { EditorView } from '@codemirror/view'
import { tags as t } from '@lezer/highlight'

const theme = EditorView.theme(
  {
    '&': {
      height: '100%',
      backgroundColor: 'hsl(var(--twc-foreground))',
      borderRadius: '0.375rem',
      borderWidth: '1px',
      borderColor: 'hsl(var(--twc-partition))',
    },
    '[data-error="true"] &': {
      borderColor: 'hsl(var(--twc-danger))',
    },

    '.cm-scroller': {
      borderRadius: '0.375rem',
    },
    '.cm-gutters': {
      backgroundColor: 'hsl(var(--twc-foreground))',
    },
    '.cm-activeLine': {
      backgroundColor: 'hsl(var(--twc-brand-500) / 0.15)',
    },
    '.cm-activeLineGutter': {
      backgroundColor: 'hsl(var(--twc-brand-500) / 0.15)',
    },

    '&.cm-focused': {
      outlineStyle: 'dashed',
      outlineWidth: '1px',
      outlineOffset: '2px',
      outlineColor: 'hsl(var(--twc-partition))',
    },

    '.cm-line': {
      color: 'hsl(var(--twc-info-700))',
    },
    '.cm-foldPlaceholder': {
      backgroundColor: 'hsl(var(--twc-foreground))',
      borderColor: 'hsl(var(--twc-partition))',
      color: 'hsl(var(--twc-content))',
      margin: '0 5px',
      padding: '0 5px',
    },
  },
  { dark: true }
)

const highlight = HighlightStyle.define([
  { tag: [t.name], color: 'hsl(var(--twc-brand))' },
  { tag: [t.propertyName], color: 'hsl(var(--twc-brand))' },
  { tag: [t.string], color: 'hsl(var(--twc-info-700))' },
  { tag: [t.separator], color: '#fff' },
  { tag: [t.comment], color: 'hsl(var(--twc-shark-700))' },
])

export const giantnodes = [theme, syntaxHighlighting(highlight)]
