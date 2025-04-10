import React from 'react'

import { Layout } from '~/components/layouts'

type CreateLayoutProps = React.PropsWithChildren

const CreateLayout: React.FC<CreateLayoutProps> = ({ children }) => (
  <Layout.Section size="sm">{children}</Layout.Section>
)

export default CreateLayout
