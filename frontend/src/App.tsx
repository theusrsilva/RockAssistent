import { lazy, Suspense } from 'react'
import { BrowserRouter, Navigate, Route, Routes } from 'react-router'
import { Layout } from './components/Layout'
import { useTranslation } from 'react-i18next'
import HomePage from './pages/HomePage'
import GuidesPage, { GuideSectionRedirect } from './pages/guides/GuidesPage'
import { useTheme } from './theme'
import LoginPage from './pages/LoginPage'
import { AuthProvider } from './lib/auth'
import { ConnectionProvider } from './lib/connection'
import { ToastProvider } from './components/ui'

// Páginas pesadas carregadas sob demanda: cada uma num chunk próprio, baixado ao abrir a rota.
// (HuntPage e NuzlockePage também exportam os cartões do Início, então ficam no chunk principal.)
const BuilderPage = lazy(() => import('./pages/builder/BuilderPage'))
const HistoryPage = lazy(() => import('./pages/HistoryPage'))
const BoxesPage = lazy(() => import('./pages/boxes/BoxesPage'))
const DexPage = lazy(() => import('./pages/dex/DexPage'))
const SpeciesPage = lazy(() => import('./pages/dex/SpeciesPage'))
const MovePage = lazy(() => import('./pages/dex/MovePage'))
const AbilityPage = lazy(() => import('./pages/dex/AbilityPage'))
const ItemPage = lazy(() => import('./pages/dex/ItemPage'))
const TypeChartPage = lazy(() => import('./pages/dex/TypeChartPage'))
const GiftsPage = lazy(() => import('./pages/gifts/GiftsPage'))
const HuntPage = lazy(() => import('./pages/hunt/HuntPage'))
const HuntCounter = lazy(() => import('./pages/hunt/HuntCounter'))
const NewHuntWizard = lazy(() => import('./pages/hunt/NewHuntWizard'))
const NuzlockePage = lazy(() => import('./pages/nuzlocke/NuzlockePage'))
const RunPage = lazy(() => import('./pages/nuzlocke/RunPage'))
const TradesHubPage = lazy(() => import('./pages/trades/TradesHubPage'))
const TradePage = lazy(() => import('./pages/trade/TradePage'))
const SettingsPage = lazy(() => import('./pages/SettingsPage'))
const AboutPage = lazy(() => import('./pages/AboutPage'))

function PageLoading() {
  const { t } = useTranslation()
  return <p className="text-sm text-muted">{t('common.loading')}</p>
}

// A página de um guia (markdown com tabelas e blocos de dados) fica num chunk separado.
const GuidePage = lazy(() => import('./pages/guides/GuidePage'))
// Seção VGC: times sugeridos e "Minha equipe" (também em chunks separados).
const VgcTeamsPage = lazy(() => import('./pages/guides/vgc/TeamsPage'))
const VgcMyTeamPage = lazy(() => import('./pages/guides/vgc/MyTeamPage'))

export default function App() {
  useTheme() // aplica o tema salvo em qualquer página
  return (
    <AuthProvider fallback={(reload) => <LoginPage onLogin={reload} />}>
      <ConnectionProvider>
        <ToastProvider>
          <BrowserRouter>
            <Layout>
              <Suspense fallback={<PageLoading />}>
                <Routes>
                  <Route path="/" element={<HomePage />} />
                  <Route path="/pokedex" element={<DexPage />} />
                  <Route path="/pokedex/:id" element={<SpeciesPage />} />
                  <Route path="/pokedex/golpe/:id" element={<MovePage />} />
                  <Route path="/pokedex/habilidade/:id" element={<AbilityPage />} />
                  <Route path="/pokedex/item/:id" element={<ItemPage />} />
                  <Route path="/pokedex/tipos" element={<TypeChartPage />} />
                  <Route path="/guias" element={<GuidesPage />} />
                  <Route path="/guias/:section" element={<GuideSectionRedirect />} />
                  <Route
                    path="/guias/vgc/times"
                    element={
                      <Suspense fallback={null}>
                        <VgcTeamsPage />
                      </Suspense>
                    }
                  />
                  <Route
                    path="/guias/vgc/minha-equipe"
                    element={
                      <Suspense fallback={null}>
                        <VgcMyTeamPage />
                      </Suspense>
                    }
                  />
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
              </Suspense>
            </Layout>
          </BrowserRouter>
        </ToastProvider>
      </ConnectionProvider>
    </AuthProvider>
  )
}
