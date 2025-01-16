'use client'

import React from 'react'
import { usePathname, useRouter, useSearchParams } from 'next/navigation'
import { Input } from '@giantnodes/react'
import { IconSearch } from '@tabler/icons-react'
import { useDebounce } from 'use-debounce'

import { usePipeline } from '~/domains/pipelines/use-pipeline-execution.hook'

const PipelineExecutionSearch: React.FC = () => {
  const router = useRouter()
  const pathname = usePathname()
  const searchParams = useSearchParams()

  const { search, setSearch } = usePipeline()
  const [debounced] = useDebounce(search, 300)

  React.useEffect(() => {
    const params = new URLSearchParams(searchParams)
    if (debounced) {
      params.set('q', debounced)
    } else {
      params.delete('q')
    }

    router.push(`${pathname}?${params.toString()}`)
  }, [debounced, pathname, router, searchParams])

  return (
    <Input.Root className="w-80" size="xs">
      <Input.Addon>
        <IconSearch size={18} strokeWidth={1} />
      </Input.Addon>

      <Input.Text
        aria-label="search"
        placeholder="Search pipeline runs"
        type="text"
        value={search}
        onChange={(e) => setSearch(e.target.value)}
      />
    </Input.Root>
  )
}

export default PipelineExecutionSearch
