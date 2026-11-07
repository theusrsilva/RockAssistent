import { useEffect } from 'react'
import { useTranslation } from 'react-i18next'
import { Link, Navigate, useLocation, useParams } from 'react-router'
import { Icon, PageTitle } from '../../components/ui'
import { GAME_VERSIONS } from '../../lib/gameVersions'
import { GUIDE_SECTIONS, guideIndex, isGuideSection, type GuideEntry, type GuideSection } from '../../lib/guides'

const icons: Record<GuideSection, string> = { games: 'sports_esports', mechanics: 'settings_suggest', vgc: 'trophy', mythology: 'auto_stories' }

/** Ordem dos jogos: a de GAME_VERSIONS (lançamento). */
const gameOrder = (key: string) => {
  const i = GAME_VERSIONS.findIndex((g) => g.key === key)
  return i < 0 ? Number.MAX_SAFE_INTEGER : i
}

function GuideCard({ section, guide }: { section: GuideSection; guide: GuideEntry }) {
  const { t } = useTranslation()
  const game = section === 'games' ? GAME_VERSIONS.find((g) => g.key === guide.key) : undefined
  return (
    <Link
      to={`/guias/${section}/${guide.key}`}
      className="flex flex-col gap-0.5 rounded-xl border border-line bg-surface px-4 py-3 transition-colors hover:border-accent hover:bg-surface2"
    >
      <span className="font-bold">{game?.name ?? guide.title}</span>
      <span className="text-[12.5px] text-muted">
        {game ? t('guides.generation', { n: game.generation }) : guide.description}
      </span>
      <span className="mt-1 text-[11.5px] font-semibold text-muted">{t('guides.pages', { count: guide.sections.length })}</span>
    </Link>
  )
}

/** Atalho para uma ferramenta da seção (times sugeridos, Minha equipe). */
function ToolCard({ to, icon, title, sub }: { to: string; icon: string; title: string; sub: string }) {
  return (
    <Link
      to={to}
      className="flex items-start gap-2.5 rounded-xl border border-line bg-surface px-4 py-3 transition-colors hover:border-accent hover:bg-surface2"
    >
      <Icon name={icon} className="mt-0.5 text-[22px] text-accent" />
      <span className="flex flex-col gap-0.5">
        <span className="font-bold">{title}</span>
        <span className="text-[12.5px] text-muted">{sub}</span>
      </span>
    </Link>
  )
}

/** /guias/<seção> sem guia: volta ao índice, na seção (ou no topo, se a seção não existe). */
export function GuideSectionRedirect() {
  const { section } = useParams()
  return <Navigate to={isGuideSection(section) ? `/guias#${section}` : '/guias'} replace />
}

/** Índice dos guias: Jogos, Mecânicas, VGC e Mitologia, cada um com os seus cartões. */
export default function GuidesPage() {
  const { t, i18n } = useTranslation()
  const index = guideIndex(i18n.language)
  const { hash } = useLocation()
  useEffect(() => {
    if (hash) document.getElementById(hash.slice(1))?.scrollIntoView?.()
  }, [hash])
  return (
    <div className="flex flex-col gap-6">
      <div>
        <PageTitle>{t('guides.title')}</PageTitle>
        <p className="mt-1 text-sm text-muted">{t('guides.sub')}</p>
      </div>
      {GUIDE_SECTIONS.map((section) => {
        const guides = section === 'games' ? [...index.games].sort((a, b) => gameOrder(a.key) - gameOrder(b.key)) : index[section]
        return (
          <section key={section} id={section} className="flex flex-col gap-2.5">
            <div>
              <h2 className="flex items-center gap-1.5 font-display text-lg font-bold">
                <Icon name={icons[section]} className="text-[20px] text-muted" />
                {t(`guides.sections.${section}`)}
              </h2>
              <p className="text-[12.5px] text-muted">{t(`guides.sectionsSub.${section}`)}</p>
            </div>
            {section === 'vgc' && (
              <div className="grid gap-2.5 sm:grid-cols-2 lg:grid-cols-3">
                <ToolCard to="/guias/vgc/times" icon="groups" title={t('vgc.teamsTitle')} sub={t('vgc.teamsCard')} />
                <ToolCard to="/guias/vgc/minha-equipe" icon="edit_note" title={t('vgc.myTeam.title')} sub={t('vgc.myTeam.card')} />
              </div>
            )}
            {guides.length === 0 ? (
              <p className="text-sm text-muted">{t('guides.empty')}</p>
            ) : (
              <div className="grid gap-2.5 sm:grid-cols-2 lg:grid-cols-3">
                {guides.map((g) => (
                  <GuideCard key={g.key} section={section} guide={g} />
                ))}
              </div>
            )}
          </section>
        )
      })}
    </div>
  )
}
