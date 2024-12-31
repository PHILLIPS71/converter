import React from 'react'
import { Input } from '@giantnodes/react'
import { IconSearch } from '@tabler/icons-react'

const SearchWidget: React.FC = () => (
  <Input.Root className="w-80" shape="pill" size="sm">
    <Input.Addon>
      <IconSearch size={18} strokeWidth={1} />
    </Input.Addon>

    <Input.Text aria-label="search" placeholder="Search for anything..." type="text" />
  </Input.Root>
)

export default SearchWidget
