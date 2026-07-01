import type { ReactNode } from 'react'

type PageStateProps = {
  isLoading: boolean
  error?: string | null
  loadingText?: string
  children: ReactNode
}

/**
 * Renders the shared loading and error states for an async data area and
 * shows its children only once loading has finished without an error.
 */
export function PageState({
  isLoading,
  error,
  loadingText = 'Wird geladen...',
  children,
}: PageStateProps) {
  if (isLoading) {
    return <p className="muted-text">{loadingText}</p>
  }

  if (error) {
    return <p className="form-message form-message--error">{error}</p>
  }

  return <>{children}</>
}
