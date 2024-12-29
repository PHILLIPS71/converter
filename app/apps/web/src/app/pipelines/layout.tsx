import React from 'react'

import PipelineSidebar from '~/domains/pipelines/pipeline-sidebar'

type PipelineLayoutProps = React.PropsWithChildren

const PipelineLayout: React.FC<PipelineLayoutProps> = ({ children }) => (
  <div className="flex flex-row flex-grow">
    <PipelineSidebar />

    <div className="max-w-6xl mx-auto w-full py-4 px-5">{children}</div>
  </div>
)

export default PipelineLayout
