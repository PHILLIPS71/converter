import React from 'react'

import { Layout } from '~/components/layouts'

type DashboardLayoutProps = React.PropsWithChildren

const DashboardLayout: React.FC<DashboardLayoutProps> = ({ children }) => <Layout.Section>{children}</Layout.Section>

export default DashboardLayout
