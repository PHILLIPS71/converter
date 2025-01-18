'use client'

import React from 'react'

import { create } from '~/utilities/create-context'

type PipelineContextType = ReturnType<typeof usePipelineValue>

type UsePipelineProps = {
  slug?: string
  search?: string
}

const usePipelineValue = (props: UsePipelineProps) => {
  const [slug, setSlug] = React.useState<string>(props.slug ?? '')
  const [search, setSearch] = React.useState<string>(props.search ?? '')

  return {
    slug,
    search,
    setSlug,
    setSearch,
  }
}

export const [PipelineProvider, usePipeline] = create<PipelineContextType, UsePipelineProps>(usePipelineValue, {
  name: 'PipelineContext',
  strict: true,
  errorMessage: 'usePipeline: `context` is undefined. Seems you forgot to wrap component within <PipelineProvider />',
})
