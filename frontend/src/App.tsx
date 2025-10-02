import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { PrivateRoute } from './components/PrivateRoute';
import { Login } from './pages/Login';
import { Register } from './pages/Register';
import { Dashboard } from './pages/Dashboard';
import { Profile } from './pages/Profile';
import { Heroes } from './pages/Heroes';
import { Quests } from './pages/Quests';
import { QuestCatalog } from './pages/QuestCatalog';
import { Items } from './pages/Items';
import { Enemies } from './pages/Enemies';

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
            path="/items"
            element={
              <PrivateRoute>
                <Items />
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

          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </AuthProvider>
    </Router>
  );
};

export default App;

