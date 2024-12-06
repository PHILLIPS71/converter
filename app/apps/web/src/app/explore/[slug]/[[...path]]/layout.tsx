import React from 'react'

import { Layout } from '~/components/layouts'

import '~/app/globals.css'

type AppLayoutProps = React.PropsWithChildren

const AppLayout: React.FC<AppLayoutProps> = ({ children }) => <Layout.Content slim>{children}</Layout.Content>

export default AppLayout
