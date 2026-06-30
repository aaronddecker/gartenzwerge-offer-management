import type { CSSProperties } from 'react'

type AnimatedNumberProps = {
  value: number
}

// Rolling number (odometer style), implemented without any library.
//
// We stack the values 0..target in a vertical column and show only one row
// through a 1-line tall window (overflow: hidden). A single CSS transform
// then slides the column up until the target row is in view, so the number
// visibly rolls through the in-between values. Because it is one continuous
// transform animation (not integer stepping), the motion stays perfectly
// smooth, and a mid-animation blur gives it the soft "trend" feel.
//
// The key={target} restarts the roll whenever the value changes, and users
// who prefer reduced motion jump straight to the final value (see CSS).
export function AnimatedNumber({ value }: AnimatedNumberProps) {
  const target = Math.max(0, Math.round(value))
  const rows = Array.from({ length: target + 1 }, (_, index) => index)

  return (
    <span className="animated-number">
      <span
        key={target}
        className="animated-number__roll"
        style={{ '--animated-number-target': target } as CSSProperties}
      >
        {rows.map((row) => (
          <span key={row} className="animated-number__row">
            {row}
          </span>
        ))}
      </span>
    </span>
  )
}
