import React from 'react'
import { notFound, redirect } from 'next/navigation'
import { Typography } from '@giantnodes/react'
import { graphql } from 'relay-runtime'

import type { page_pipeline_Query } from '~/__generated__/page_pipeline_Query.graphql'
import type { PipelineExecutionFilterInput } from '~/__generated__/PipelineExecutionTablePaginationQuery.graphql'
import { PipelineEditMenu } from '~/domains/pipelines/construct'
import { PipelineExecutionSearch, PipelineExecutionTable } from '~/domains/pipelines/executions'
import { PipelineProvider } from '~/domains/pipelines/use-pipeline.hook'
import HydrationBoundary from '~/libraries/relay/HydrationBoundary'
import { query } from '~/libraries/relay/server'

type PipelineSlugPageProps = {
  params: Promise<{
    slug?: string[]
  }>
  searchParams: Promise<{
    q?: string
  }>
}

const QUERY = graphql`
  query page_pipeline_Query($isAll: Boolean!, $slug: String, $where: PipelineExecutionFilterInput) {
    pipeline(where: { slug: { eq: $slug } }) @skip(if: $isAll) {
      id
      name
      description
      ...editMenu_pipeline
    }
    ...table_pipelineExecutions_query @arguments(where: $where)
  }
`

const where = (slug?: string, search?: string): PipelineExecutionFilterInput | undefined => {
  const parts = []

  if (search?.trim()) {
    parts.push({ file: { pathInfo: { name: { contains: search } } } })
  }

  if (slug?.trim()) {
    parts.push({ pipeline: { slug: { eq: slug } } })
  }

  let where: PipelineExecutionFilterInput | undefined = undefined
  if (parts.length > 0) {
    where = { and: parts }
  }

  return where
}

const PipelineSlugPage: React.FC<PipelineSlugPageProps> = async ({ params, searchParams }) => {
  const [parameters, { q }] = await Promise.all([params, searchParams])
  const slug = parameters.slug?.[0]?.trim()

  if (parameters.slug && parameters.slug.length > 1) {
    return redirect('/pipelines')
  }

  const { data, ...operation } = await query<page_pipeline_Query>(QUERY, {
    isAll: slug?.trim().length == 0,
    slug: slug,
    where: where(slug, q),
  })

  if (slug && data.pipeline == null) {
    console.warn(`pipeline with slug '${slug}' not found`)
    return notFound()
  }

  return (
    <HydrationBoundary operation={operation}>
      <PipelineProvider search={q} slug={slug}>
        <div className="flex flex-col gap-6">
          <div className="flex flex-col xl:flex-row xl:space-between items-start gap-6">
            <div className="flex flex-col grow w-full min-w-0">
              <Typography.HeadingLevel>
                <Typography.Heading className="truncate" level={6}>
                  {data.pipeline?.name ?? 'All Pipelines'}
                </Typography.Heading>
                <Typography.Paragraph className="truncate" size="sm" variant="subtitle">
                  {data.pipeline?.description ?? 'Showing runs from all pipelines'}
                </Typography.Paragraph>
              </Typography.HeadingLevel>
            </div>

            <div className="flex flex-row justify-end gap-2 w-full">
              <PipelineExecutionSearch />

              {data.pipeline != null && <PipelineEditMenu $key={data.pipeline} />}
            </div>
          </div>

          <PipelineExecutionTable $key={data} />
        </div>
      </PipelineProvider>
    </HydrationBoundary>
  )
}

export default PipelineSlugPage
