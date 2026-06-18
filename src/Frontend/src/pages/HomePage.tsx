import heroImg from '../assets/hero.png'

export function HomePage() {
  return (
    <main className="app-shell">
      <section className="hero-section">
        <img
          src={heroImg}
          className="hero-image"
          width="170"
          height="179"
          alt="Gartenzwerg illustration"
        />

        <div className="hero-content">
          <h1>Gartenzwerge Management</h1>
          <p>Angebots- und Auftragsmanagement für Gartenzwerge Außenservice</p>
          <p>Backend verbunden: noch nicht</p>
          <p>Frontend Foundation: aktiv</p>
        </div>
      </section>
    </main>
  )
}