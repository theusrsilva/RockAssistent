import { useEffect, useMemo, useRef, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link, useNavigate, useParams } from 'react-router'
import { Button, Callout, Icon, selectClass } from '../../components/ui'
import { gameVersionByKey } from '../../lib/gameVersions'
import { findGuide, guideIndex, loadGuide, type GuideEntry, type GuideSection } from '../../lib/guides'
import { countNormalized, normalizeForSearch, normalizeQuery } from '../../lib/textSearch'
import { GuideMarkdown } from './GuideMarkdown'
import { GuideSearchBar, useGuideSearch } from './GuideSearch'

type PageText = { file: string; text: string | null; failed: boolean }

/** Texto da página, carregado sob demanda; muda de arquivo = volta a "carregando". */
function usePageText(lang: string, file: string | null): PageText | null {
  const [state, setState] = useState<PageText | null>(null)
  useEffect(() => {
    if (!file) return
    let alive = true
    loadGuide(lang, file)
      .then((text) => alive && setState({ file, text, failed: false }))
      .catch(() => alive && setState({ file, text: null, failed: true }))
    return () => {
      alive = false
    }
  }, [lang, file])
  return state && state.file === file ? state : null
}

/**
 * Resultados da busca em cada página do guia (no texto do markdown, para o sumário). Os textos das páginas só carregam
 * quando há busca.
 */
function usePageCounts(lang: string, guide: GuideEntry | null, query: string): Map<string, number> {
  const [texts, setTexts] = useState<{ key: string; byId: Map<string, string> } | null>(null)
  const active = normalizeQuery(query) !== ''
  const key = guide ? `${lang}:${guide.key}` : ''
  useEffect(() => {
    if (!active || !guide || texts?.key === key) return
    let alive = true
    void Promise.all(guide.sections.map((p) => loadGuide(lang, p.file).catch(() => null))).then((list) => {
      // Normaliza uma vez por carga; cada tecla só conta.
      if (alive) setTexts({ key, byId: new Map(guide.sections.map((p, i) => [p.id, normalizeForSearch(list[i] ?? '').text])) })
    })
    return () => {
      alive = false
    }
  }, [active, guide, key, lang, texts?.key])
  return useMemo(() => {
    const counts = new Map<string, number>()
    if (active && texts?.key === key) for (const [id, text] of texts.byId) counts.set(id, countNormalized(text, query))
    return counts
  }, [active, texts, key, query])
}

/** Um guia: breadcrumb, título, sumário (lateral; seletor no celular), a página e anterior/próxima. */
export default function GuidePage() {
  const { t, i18n } = useTranslation()
  const navigate = useNavigate()
  const { section, slug, page: pageId } = useParams()
  const guide = findGuide(guideIndex(i18n.language), section, slug)
  const index = guide ? (pageId ? guide.sections.findIndex((p) => p.id === pageId) : 0) : -1
  const page = guide && index >= 0 ? guide.sections[index] : null
  const text = usePageText(i18n.language, page?.file ?? null)
  const [query, setQuery] = useState('')
  const contentRef = useRef<HTMLDivElement>(null)
  const found = useGuideSearch(contentRef, query, `${guide?.key ?? ''}/${page?.id ?? ''}/${text ? 'ok' : 'loading'}`)
  const pageCounts = usePageCounts(i18n.language, guide ?? null, query)

  if (!guide || !page)
    return (
      <div className="flex flex-col gap-4">
        <Callout tone="amber" icon="help">
          <div className="font-semibold">{t('guides.notFound')}</div>
          <div>{t('guides.notFoundHint')}</div>
        </Callout>
        <Link to="/guias" className="text-sm font-semibold text-accent">
          {t('guides.backToGuides')}
        </Link>
      </div>
    )

  const sectionKey = section as GuideSection
  const title = sectionKey === 'games' ? (gameVersionByKey(guide.key)?.name ?? guide.title) : guide.title
  const href = (id: string) => `/guias/${sectionKey}/${guide.key}/${id}`
  const prev = guide.sections[index - 1]
  const next = guide.sections[index + 1]
  const multi = guide.sections.length > 1
  // Página aberta: o que está na tela (inclui os blocos de dados); as outras: o texto do markdown.
  const hits = (id: string) => (id === page.id ? found.count : (pageCounts.get(id) ?? 0))

  return (
    <div className="flex flex-col gap-4">
      <nav className="flex flex-wrap items-center gap-1 text-[12.5px] text-muted" aria-label="breadcrumb">
        <Link to="/guias" className="hover:text-accent">
          {t('guides.title')}
        </Link>
        <Icon name="chevron_right" className="text-[16px]" />
        <Link to={`/guias#${sectionKey}`} className="hover:text-accent">
          {t(`guides.sections.${sectionKey}`)}
        </Link>
        <Icon name="chevron_right" className="text-[16px]" />
        <span className="font-semibold text-ink">{title}</span>
      </nav>
      <div>
        <h1 className="font-display text-[23px] leading-tight font-bold md:text-[28px]">{title}</h1>
        {multi && <p className="mt-0.5 text-sm text-muted">{page.title}</p>}
      </div>
      <GuideSearchBar query={query} onQuery={setQuery} count={found.count} current={found.current} onNext={found.next} onPrev={found.prev} />

      <div className="flex flex-col gap-4 lg:flex-row lg:items-start">
        {multi && (
          <>
            <label className="lg:hidden">
              <span className="sr-only">{t('guides.contents')}</span>
              <select className={selectClass} value={page.id} onChange={(e) => navigate(href(e.target.value))}>
                {guide.sections.map((p) => (
                  <option key={p.id} value={p.id}>
                    {hits(p.id) ? `${p.title} (${hits(p.id)})` : p.title}
                  </option>
                ))}
              </select>
            </label>
            <aside className="hidden w-52 shrink-0 lg:sticky lg:top-4 lg:block">
              <div className="mb-1.5 text-[11.5px] font-bold tracking-wide text-muted uppercase">{t('guides.contents')}</div>
              <ul className="flex flex-col gap-0.5">
                {guide.sections.map((p) => (
                  <li key={p.id}>
                    <Link
                      to={href(p.id)}
                      aria-current={p.id === page.id ? 'page' : undefined}
                      className={`flex items-center gap-1.5 rounded-md px-2.5 py-1.5 text-sm ${p.id === page.id ? 'bg-accent-soft font-bold text-ink' : 'text-muted hover:bg-surface2 hover:text-ink'}`}
                    >
                      <span className="min-w-0 flex-1">{p.title}</span>
                      {hits(p.id) > 0 && (
                        <span className="shrink-0 rounded-full bg-warn-soft px-1.5 text-[11px] font-bold text-warn tabular-nums" title={t('guides.search.inPage', { count: hits(p.id) })}>
                          {hits(p.id)}
                        </span>
                      )}
                    </Link>
                  </li>
                ))}
              </ul>
            </aside>
          </>
        )}

        <article className="min-w-0 flex-1 rounded-xl border border-line bg-surface p-4 md:p-5">
          {!text ? (
            <p className="text-sm text-muted">{t('guides.loading')}</p>
          ) : text.text === null ? (
            <Callout tone="amber" icon="cloud_off">
              {t('guides.pageError')}
            </Callout>
          ) : (
            <div ref={contentRef}>
              <GuideMarkdown text={text.text} game={sectionKey === 'games' ? guide.key : undefined} />
            </div>
          )}
          {(prev || next) && (
            <div className="mt-6 flex flex-wrap justify-between gap-2 border-t border-line pt-4">
              {prev ? (
                <Button variant="secondary" icon="arrow_back" onClick={() => navigate(href(prev.id))}>
                  {`${t('guides.previous')}: ${prev.title}`}
                </Button>
              ) : (
                <span />
              )}
              {next && (
                <Button variant="secondary" iconAfter="arrow_forward" onClick={() => navigate(href(next.id))}>
                  {`${t('guides.next')}: ${next.title}`}
                </Button>
              )}
            </div>
          )}
        </article>
      </div>
    </div>
  )
}
