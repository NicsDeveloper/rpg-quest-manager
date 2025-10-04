import './App.css'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import { useState, useEffect } from 'react'
import { AuthProvider, useAuth } from './contexts/AuthContext'
import { CharacterProvider } from './contexts/CharacterContext'
import { Layout } from './components/Layout'
import { Tutorial } from './components/Tutorial'
import Login from './pages/Login'
import Dashboard from './pages/Dashboard'
import Inventory from './pages/Inventory'
import Shop from './pages/Shop'
import Quests from './pages/Quests'
import Combat from './pages/Combat'
import BossesByEnvironment from './pages/BossesByEnvironment'
import Achievements from './pages/Achievements'
import Parties from './pages/Parties'

function AppRoutes() {
  const { isAuthenticated, isLoading } = useAuth();
  const [showTutorial, setShowTutorial] = useState(false);

  useEffect(() => {
    // Verificar se é a primeira vez que o usuário acessa
    const hasSeenTutorial = localStorage.getItem('hasSeenTutorial');
    if (isAuthenticated && !hasSeenTutorial) {
      setShowTutorial(true);
    }
  }, [isAuthenticated]);

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-blue-900 via-purple-900 to-indigo-900 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-white mx-auto mb-4"></div>
          <p className="text-white">Carregando...</p>
        </div>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Login />;
  }

  return (
    <CharacterProvider>
      <Layout>
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/character" element={<Dashboard />} />
          <Route path="/inventory" element={<Inventory />} />
          <Route path="/shop" element={<Shop />} />
          <Route path="/quests" element={<Quests />} />
          <Route path="/combat" element={<Combat />} />
          <Route path="/bosses" element={<BossesByEnvironment />} />
          <Route path="/achievements" element={<Achievements />} />
          <Route path="/parties" element={<Parties />} />
        </Routes>
      </Layout>
      {showTutorial && (
        <Tutorial 
          isOpen={showTutorial}
          onClose={() => {
            setShowTutorial(false);
            localStorage.setItem('hasSeenTutorial', 'true');
          }}
          onComplete={() => {
            setShowTutorial(false);
            localStorage.setItem('hasSeenTutorial', 'true');
          }}
        />
      )}
    </CharacterProvider>
  );
}

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AppRoutes />
      </AuthProvider>
    </BrowserRouter>
  )
}

export default App
