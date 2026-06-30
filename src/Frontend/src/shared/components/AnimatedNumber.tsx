import type { CSSProperties } from 'react'

const DIGITS = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]

type AnimatedNumberProps = {
  value: number
  format?: (value: number) => string
}

// Rolling number (odometer style), implemented without any library.
//
// The value is turned into its final display text (optionally formatted, e.g.
// as currency). Every digit gets its own 0-9 column shown through a one-row
// window; a CSS transform rolls each column to its target digit. Separators
// like ".", "," or "€" are rendered statically.
//
// Working per digit means it stays cheap for any magnitude (10 rows per digit
// instead of one row per counted value), and the key={text} restarts the roll
// whenever the value changes. Reduced motion jumps to the final value (see CSS).
export function AnimatedNumber({ value, format }: AnimatedNumberProps) {
  const text = format ? format(value) : String(Math.max(0, Math.round(value)))

  return (
    <span className="animated-number" key={text}>
      {Array.from(text).map((char, index) => {
        if (char < '0' || char > '9') {
          return (
            <span key={index} className="animated-number__static">
              {char}
            </span>
          )
        }

        return (
          <span key={index} className="animated-number__digit">
            <span
              className="animated-number__strip"
              style={
                { '--animated-number-digit': Number(char) } as CSSProperties
              }
            >
              {DIGITS.map((digit) => (
                <span key={digit} className="animated-number__row">
                  {digit}
                </span>
              ))}
            </span>
          </span>
        )
      })}
    </span>
  )
}
