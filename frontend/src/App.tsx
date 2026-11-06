import { lazy, Suspense } from 'react'
import { BrowserRouter, Navigate, Route, Routes } from 'react-router'
import { Layout } from './components/Layout'
import BuilderPage from './pages/builder/BuilderPage'
import HistoryPage from './pages/HistoryPage'
import HomePage from './pages/HomePage'
import BoxesPage from './pages/boxes/BoxesPage'
import DexPage from './pages/dex/DexPage'
import SpeciesPage from './pages/dex/SpeciesPage'
import MovePage from './pages/dex/MovePage'
import AbilityPage from './pages/dex/AbilityPage'
import ItemPage from './pages/dex/ItemPage'
import TypeChartPage from './pages/dex/TypeChartPage'
import GiftsPage from './pages/gifts/GiftsPage'
import GuidesPage from './pages/guides/GuidesPage'
import HuntPage from './pages/hunt/HuntPage'
import HuntCounter from './pages/hunt/HuntCounter'
import NewHuntWizard from './pages/hunt/NewHuntWizard'
import NuzlockePage from './pages/nuzlocke/NuzlockePage'
import RunPage from './pages/nuzlocke/RunPage'
import TradesHubPage from './pages/trades/TradesHubPage'
import SettingsPage from './pages/SettingsPage'
import TradePage from './pages/trade/TradePage'
import { useTheme } from './theme'
import AboutPage from './pages/AboutPage'
import LoginPage from './pages/LoginPage'
import { AuthProvider } from './lib/auth'
import { ConnectionProvider } from './lib/connection'
import { ToastProvider } from './components/ui'

// A página de um guia (markdown com tabelas e blocos de dados) fica num chunk separado.
const GuidePage = lazy(() => import('./pages/guides/GuidePage'))

export default function App() {
  useTheme() // aplica o tema salvo em qualquer página
  return (
    <AuthProvider fallback={(reload) => <LoginPage onLogin={reload} />}>
      <ConnectionProvider>
        <ToastProvider>
          <BrowserRouter>
            <Layout>
              <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/pokedex" element={<DexPage />} />
                <Route path="/pokedex/:id" element={<SpeciesPage />} />
                <Route path="/pokedex/golpe/:id" element={<MovePage />} />
                <Route path="/pokedex/habilidade/:id" element={<AbilityPage />} />
                <Route path="/pokedex/item/:id" element={<ItemPage />} />
                <Route path="/pokedex/tipos" element={<TypeChartPage />} />
                <Route path="/guias" element={<GuidesPage />} />
                <Route
                  path="/guias/:section/:slug/:page?"
                  element={
                    <Suspense fallback={null}>
                      <GuidePage />
                    </Suspense>
                  }
                />
                <Route path="/boxes" element={<BoxesPage />} />
                <Route path="/trocas" element={<TradesHubPage />} />
                <Route path="/trocas/trocar" element={<TradePage />} />
                <Route path="/mystery-gift" element={<GiftsPage />} />
                <Route path="/nuzlocke" element={<NuzlockePage />} />
                <Route path="/nuzlocke/:id" element={<RunPage />} />
                <Route path="/hunt" element={<HuntPage />} />
                <Route path="/hunt/nova" element={<NewHuntWizard />} />
                <Route path="/hunt/:id" element={<HuntCounter />} />
                <Route path="/historico" element={<HistoryPage />} />
                {/* O Boxes abre o construtor em "Editar" e "+ Novo". */}
                <Route path="/criar" element={<BuilderPage />} />
                <Route path="/criar/:id" element={<BuilderPage />} />
                {/* Rotas antigas do TradeRock. */}
                <Route path="/trocar" element={<Navigate to="/trocas/trocar" replace />} />
                <Route path="/pokemon" element={<Navigate to="/boxes" replace />} />
                <Route path="/atividades" element={<Navigate to="/trocas" replace />} />
                <Route path="/configuracoes" element={<SettingsPage />} />
                {/* O antigo assistente de 7 etapas virou a lista de Configurações → Conexão. */}
                <Route path="/configurar" element={<Navigate to="/configuracoes?aba=conexao" replace />} />
                <Route path="/sobre" element={<AboutPage />} />
                <Route path="*" element={<HomePage />} />
              </Routes>
            </Layout>
          </BrowserRouter>
        </ToastProvider>
      </ConnectionProvider>
    </AuthProvider>
  )
}
