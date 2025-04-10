import { Input } from '@giantnodes/react'
import { IconSearch } from '@tabler/icons-react'

const SearchWidget: React.FC = () => (
  <Input.Root shape="pill">
    <Input.Addon>
      <IconSearch size={20} strokeWidth={1} />
    </Input.Addon>

    <Input.Text aria-label="search" className="w-64" placeholder="Search for anything..." size="sm" type="text" />
  </Input.Root>
)

export default SearchWidget
