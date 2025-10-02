import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { PrivateRoute } from './components/PrivateRoute';
import { Login } from './pages/Login';
import { Register } from './pages/Register';
import { Dashboard } from './pages/Dashboard';
import { Profile } from './pages/Profile';
import { Heroes } from './pages/Heroes';
import { HeroRecovery } from './pages/HeroRecovery';
import { Quests } from './pages/Quests';
import { QuestCatalog } from './pages/QuestCatalog';
import { QuestCategories } from './pages/QuestCategories';
import { Items } from './pages/Items';
import { Enemies } from './pages/Enemies';
import Combat from './pages/Combat';
import { DiceShop } from './pages/DiceShop';
import { Bestiary } from './pages/Bestiary';
import { FreeDice } from './pages/FreeDice';
import { Shop } from './pages/Shop';
import { Attributes } from './pages/Attributes';

const App: React.FC = () => {
  return (
    <Router>
      <AuthProvider>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />

          <Route
            path="/"
            element={
              <PrivateRoute>
                <Dashboard />
              </PrivateRoute>
            }
          />

          <Route
            path="/profile"
            element={
              <PrivateRoute>
                <Profile />
              </PrivateRoute>
            }
          />

          <Route
            path="/heroes"
            element={
              <PrivateRoute>
                <Heroes />
              </PrivateRoute>
            }
          />

          <Route
            path="/hero-recovery"
            element={
              <PrivateRoute>
                <HeroRecovery />
              </PrivateRoute>
            }
          />

          <Route
            path="/quests"
            element={
              <PrivateRoute>
                <Quests />
              </PrivateRoute>
            }
          />

          <Route
            path="/quest-catalog"
            element={
              <PrivateRoute>
                <QuestCatalog />
              </PrivateRoute>
            }
          />

          <Route
            path="/quest-categories"
            element={
              <PrivateRoute>
                <QuestCategories />
              </PrivateRoute>
            }
          />

          <Route
            path="/items"
            element={
              <PrivateRoute>
                <Items />
              </PrivateRoute>
            }
          />

          <Route
            path="/shop"
            element={
              <PrivateRoute>
                <Shop />
              </PrivateRoute>
            }
          />

          <Route
            path="/attributes"
            element={
              <PrivateRoute>
                <Attributes />
              </PrivateRoute>
            }
          />

          <Route
            path="/enemies"
            element={
              <PrivateRoute adminOnly>
                <Enemies />
              </PrivateRoute>
            }
          />

          <Route
            path="/combat"
            element={
              <PrivateRoute>
                <Combat />
              </PrivateRoute>
            }
          />

          <Route
            path="/dice-shop"
            element={
              <PrivateRoute>
                <DiceShop />
              </PrivateRoute>
            }
          />

          <Route
            path="/bestiary"
            element={
              <PrivateRoute>
                <Bestiary />
              </PrivateRoute>
            }
          />

          <Route
            path="/free-dice"
            element={
              <PrivateRoute>
                <FreeDice />
              </PrivateRoute>
            }
          />

          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </AuthProvider>
    </Router>
  );
};

export default App;

