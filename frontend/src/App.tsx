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
import { Crown, Sword, Shield } from 'lucide-react'

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
      <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 flex items-center justify-center relative overflow-hidden">
        <div className="absolute inset-0 overflow-hidden">
          {[...Array(15)].map((_, i) => (
            <div
              key={i}
              className="absolute w-2 h-2 bg-amber-400 rounded-full opacity-30 animate-pulse"
              style={{
                left: `${Math.random() * 100}%`,
                top: `${Math.random() * 100}%`,
                animationDelay: `${Math.random() * 3}s`,
                animationDuration: `${2 + Math.random() * 3}s`
              }}
            />
          ))}
        </div>
        
        <div className="text-center relative z-10">
          <div className="flex justify-center items-center space-x-2 mb-6">
            <Crown className="h-12 w-12 text-amber-400 animate-pulse" />
            <Sword className="h-10 w-10 text-blue-400" />
            <Shield className="h-10 w-10 text-red-400" />
          </div>
          <div className="inline-block p-6 bg-gradient-to-br from-amber-500 to-orange-600 rounded-full shadow-lg shadow-amber-500/50 animate-pulse mb-4">
            <div className="animate-spin rounded-full h-16 w-16 border-4 border-white border-t-transparent"></div>
          </div>
          <h2 className="text-4xl font-black text-transparent bg-clip-text bg-gradient-to-r from-amber-300 via-amber-500 to-orange-600 mb-2">Carregando Aventura...</h2>
          <p className="text-gray-400 text-lg">Preparando seu reino épico</p>
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
