import { useEffect, useState } from 'react'

export type ChartPoint = {
  label: string
  value: number
}

type TrendBarChartProps = {
  data: ChartPoint[]
  formatValue?: (value: number) => string
  formatAxis?: (value: number) => string
}

const WIDTH = 480
const HEIGHT = 250
const PADDING_TOP = 26
const PADDING_BOTTOM = 34
const PADDING_X = 12
const AXIS_LEFT = 56

// Least-squares linear regression over (index, value) pairs.
function computeTrend(values: number[]) {
  const n = values.length

  if (n === 0) {
    return { slope: 0, intercept: 0 }
  }

  let sumX = 0
  let sumY = 0
  let sumXY = 0
  let sumXX = 0

  for (let i = 0; i < n; i += 1) {
    sumX += i
    sumY += values[i]
    sumXY += i * values[i]
    sumXX += i * i
  }

  const denominator = n * sumXX - sumX * sumX

  if (denominator === 0) {
    return { slope: 0, intercept: sumY / n }
  }

  const slope = (n * sumXY - sumX * sumY) / denominator
  const intercept = (sumY - slope * sumX) / n

  return { slope, intercept }
}

// Rounds the axis maximum up to a "nice" value and returns a matching step,
// so the y-axis shows clean numbers (0, 2.000, 4.000, ...) instead of raw maxima.
function niceScale(max: number, tickCount: number) {
  const rawStep = max / tickCount
  const magnitude = Math.pow(10, Math.floor(Math.log10(rawStep)))
  const normalized = rawStep / magnitude

  let niceStep
  if (normalized <= 1) {
    niceStep = 1
  } else if (normalized <= 2) {
    niceStep = 2
  } else if (normalized <= 5) {
    niceStep = 5
  } else {
    niceStep = 10
  }

  niceStep *= magnitude

  return { niceStep, niceMax: Math.ceil(max / niceStep) * niceStep }
}

// DIY SVG bar chart with a linear trend line. Per-month values are revealed on
// hover (desktop) or on tap (touch devices, which cannot hover). On desktop a
// euro y-axis scale is shown; mobile stays axis-free to save space.
export function TrendBarChart({
  data,
  formatValue,
  formatAxis,
}: TrendBarChartProps) {
  const [active, setActive] = useState<number | null>(null)
  const [supportsHover] = useState(
    () =>
      typeof window !== 'undefined' &&
      window.matchMedia('(hover: hover)').matches
  )
  const [isDesktop, setIsDesktop] = useState(
    () =>
      typeof window !== 'undefined' &&
      window.matchMedia('(min-width: 768px)').matches
  )

  useEffect(() => {
    const query = window.matchMedia('(min-width: 768px)')
    const handleChange = (event: MediaQueryListEvent) =>
      setIsDesktop(event.matches)

    query.addEventListener('change', handleChange)
    return () => query.removeEventListener('change', handleChange)
  }, [])

  const format = formatValue ?? ((value: number) => String(value))
  const formatTick = formatAxis ?? format
  const values = data.map((point) => point.value)
  const maxValue = Math.max(1, ...values)
  const n = data.length

  const { niceStep, niceMax } = niceScale(maxValue, 4)
  const scaleMax = niceMax

  const leftPadding = isDesktop ? AXIS_LEFT : PADDING_X
  const chartWidth = WIDTH - leftPadding - PADDING_X
  const chartHeight = HEIGHT - PADDING_TOP - PADDING_BOTTOM
  const baselineY = PADDING_TOP + chartHeight
  const slot = chartWidth / n
  const barWidth = slot * 0.6

  const centerX = (index: number) => leftPadding + index * slot + slot / 2
  const barX = (index: number) =>
    leftPadding + index * slot + (slot - barWidth) / 2
  const valueToY = (value: number) =>
    baselineY - Math.max(0, Math.min(1, value / scaleMax)) * chartHeight

  const { slope, intercept } = computeTrend(values)
  const trendStartY = valueToY(intercept)
  const trendEndY = valueToY(slope * (n - 1) + intercept)

  const ticks: number[] = []
  for (let tick = 0; tick <= scaleMax + niceStep / 2; tick += niceStep) {
    ticks.push(tick)
  }

  let tooltip = null

  if (active !== null) {
    const label = format(data[active].value)
    const tooltipWidth = Math.max(44, label.length * 7 + 16)
    const clampedX = Math.min(
      WIDTH - PADDING_X - tooltipWidth / 2,
      Math.max(leftPadding + tooltipWidth / 2, centerX(active))
    )
    const tooltipY = Math.max(PADDING_TOP, valueToY(data[active].value) - 28)

    tooltip = (
      <g pointerEvents="none">
        <rect
          x={clampedX - tooltipWidth / 2}
          y={tooltipY}
          width={tooltipWidth}
          height={22}
          rx={5}
          fill="var(--color-text)"
        />
        <text
          x={clampedX}
          y={tooltipY + 15}
          textAnchor="middle"
          fontSize={13}
          fontWeight={700}
          fill="#ffffff"
        >
          {label}
        </text>
      </g>
    )
  }

  return (
    <div className="trend-chart">
      <svg
        viewBox={`0 0 ${WIDTH} ${HEIGHT}`}
        role="img"
        aria-label="Umsatzentwicklung der letzten 12 Monate"
      >
        {isDesktop &&
          ticks.map((tick) => (
            <g key={`tick-${tick}`}>
              <line
                x1={leftPadding}
                y1={valueToY(tick)}
                x2={WIDTH - PADDING_X}
                y2={valueToY(tick)}
                stroke="var(--color-border)"
              />
              <text
                x={leftPadding - 8}
                y={valueToY(tick) + 4}
                textAnchor="end"
                fontSize={11}
                fill="var(--color-text-soft)"
              >
                {formatTick(tick)}
              </text>
            </g>
          ))}

        {!isDesktop && (
          <line
            x1={leftPadding}
            y1={baselineY}
            x2={WIDTH - PADDING_X}
            y2={baselineY}
            stroke="var(--color-border)"
          />
        )}

        {data.map((point, index) => {
          const height = Math.min(1, point.value / scaleMax) * chartHeight
          const isActive = active === index

          return (
            <g key={index}>
              <rect
                className="trend-chart__bar"
                x={barX(index)}
                y={baselineY - height}
                width={barWidth}
                height={height}
                rx={3}
                fill={isActive ? 'var(--color-primary)' : 'var(--color-accent)'}
              />
              <text
                x={centerX(index)}
                y={HEIGHT - 12}
                textAnchor="middle"
                fontSize={15}
                fill="var(--color-text-soft)"
              >
                {point.label}
              </text>
            </g>
          )
        })}

        <line
          x1={centerX(0)}
          y1={trendStartY}
          x2={centerX(n - 1)}
          y2={trendEndY}
          stroke="var(--color-text)"
          strokeWidth={2}
          strokeDasharray="5 4"
          strokeLinecap="round"
          pointerEvents="none"
        />

        {data.map((_, index) => (
          <rect
            key={index}
            x={leftPadding + index * slot}
            y={PADDING_TOP}
            width={slot}
            height={chartHeight}
            fill="transparent"
            style={{ cursor: supportsHover ? 'default' : 'pointer' }}
            onMouseEnter={supportsHover ? () => setActive(index) : undefined}
            onMouseLeave={supportsHover ? () => setActive(null) : undefined}
            onClick={
              supportsHover
                ? undefined
                : () =>
                    setActive((current) => (current === index ? null : index))
            }
          />
        ))}

        {tooltip}
      </svg>
    </div>
  )
}
