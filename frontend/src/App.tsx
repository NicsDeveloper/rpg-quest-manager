import './App.css'
import { BrowserRouter, Link, Route, Routes } from 'react-router-dom'
import Dashboard from './pages/Dashboard'
import Quests from './pages/Quests'
import Combat from './pages/Combat'
import BossesByEnvironment from './pages/BossesByEnvironment'

function App() {
  return (
    <BrowserRouter>
      <nav style={{ display: 'flex', gap: 12, padding: 12 }}>
        <Link to="/">Dashboard</Link>
        <Link to="/quests">Quests</Link>
        <Link to="/combat">Combat</Link>
        <Link to="/bosses">Bosses</Link>
      </nav>
      <Routes>
        <Route path="/" element={<Dashboard />} />
        <Route path="/quests" element={<Quests />} />
        <Route path="/combat" element={<Combat />} />
        <Route path="/bosses" element={<BossesByEnvironment />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
