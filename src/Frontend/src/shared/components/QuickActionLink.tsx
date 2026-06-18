import { Link } from 'react-router-dom'

type QuickActionLinkProps = {
  to: string
  title: string
  description: string
}

export function QuickActionLink({ to, title, description }: QuickActionLinkProps) {
  return (
    <Link to={to} className="quick-action">
      <span className="quick-action__title">{title}</span>
      <span className="quick-action__description">{description}</span>
    </Link>
  )
}