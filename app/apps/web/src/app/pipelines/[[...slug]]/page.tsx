import React from 'react'
import { notFound, redirect } from 'next/navigation'
import { Card, Typography } from '@giantnodes/react'
import { graphql } from 'relay-runtime'

import type { page_PipelineSlugQuery } from '~/__generated__/page_PipelineSlugQuery.graphql'
import type { PipelineExecutionFilterInput } from '~/__generated__/PipelineExecutionTablePaginationQuery.graphql'
import PipelineExecutionSearch from '~/domains/pipelines/pipeline-execution-search'
import PipelineExecutionTable from '~/domains/pipelines/pipeline-execution-table'
import PipelineMenu from '~/domains/pipelines/pipeline-menu'
import { PipelineProvider } from '~/domains/pipelines/use-pipeline-execution.hook'
import RelayStoreHydrator from '~/libraries/relay/RelayStoreHydrator'
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
  query page_PipelineSlugQuery($isAll: Boolean!, $slug: String, $where: PipelineExecutionFilterInput) {
    pipeline(where: { slug: { eq: $slug } }) @skip(if: $isAll) {
      id
      name
      description
      ...pipelineMenuFragment_pipeline
    }
    ...pipelineExecutionTableFragment_query @arguments(where: $where)
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

  const { data, ...operation } = await query<page_PipelineSlugQuery>(QUERY, {
    isAll: slug?.trim().length == 0,
    slug: slug,
    where: where(slug, q),
  })

  if (slug && data.pipeline == null) {
    console.warn(`pipeline with slug '${slug}' not found`)
    return notFound()
  }

  return (
    <RelayStoreHydrator operation={operation}>
      <PipelineProvider search={q} slug={slug}>
        <div className="flex flex-col gap-6">
          <div className="flex flex-row justify-between items-start gap-6">
            <div className="flex flex-col flex-1 min-w-0 w-0">
              <Typography.HeadingLevel>
                <Typography.Heading className="truncate" level={6}>
                  {data.pipeline?.name ?? 'All Pipelines'}
                </Typography.Heading>
                <Typography.Paragraph className="truncate" size="sm" variant="subtitle">
                  {data.pipeline?.description ?? 'Showing runs from all pipelines'}
                </Typography.Paragraph>
              </Typography.HeadingLevel>
            </div>

            <div className="flex flex-row gap-2">
              <PipelineExecutionSearch />

              {data.pipeline != null && <PipelineMenu $key={data.pipeline} />}
            </div>
          </div>

          <Card.Root>
            <PipelineExecutionTable $key={data} />
          </Card.Root>
        </div>
      </PipelineProvider>
    </RelayStoreHydrator>
  )
}

export default PipelineSlugPage
