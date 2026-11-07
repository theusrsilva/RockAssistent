import { useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link, useNavigate, useParams } from 'react-router'
import { Button, Callout, Icon, selectClass } from '../../components/ui'
import { gameVersionByKey } from '../../lib/gameVersions'
import { findGuide, guideIndex, loadGuide, type GuideSection } from '../../lib/guides'
import { GuideMarkdown } from './GuideMarkdown'

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

/** Um guia: breadcrumb, título, sumário (lateral; seletor no celular), a página e anterior/próxima. */
export default function GuidePage() {
  const { t, i18n } = useTranslation()
  const navigate = useNavigate()
  const { section, slug, page: pageId } = useParams()
  const guide = findGuide(guideIndex(i18n.language), section, slug)
  const index = guide ? (pageId ? guide.sections.findIndex((p) => p.id === pageId) : 0) : -1
  const page = guide && index >= 0 ? guide.sections[index] : null
  const text = usePageText(i18n.language, page?.file ?? null)

  useEffect(() => {
    window.scrollTo?.(0, 0)
  }, [page?.file])

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

      <div className="flex flex-col gap-4 lg:flex-row lg:items-start">
        {multi && (
          <>
            <label className="lg:hidden">
              <span className="sr-only">{t('guides.contents')}</span>
              <select className={selectClass} value={page.id} onChange={(e) => navigate(href(e.target.value))}>
                {guide.sections.map((p) => (
                  <option key={p.id} value={p.id}>
                    {p.title}
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
                      className={`block rounded-md px-2.5 py-1.5 text-sm ${p.id === page.id ? 'bg-accent-soft font-bold text-ink' : 'text-muted hover:bg-surface2 hover:text-ink'}`}
                    >
                      {p.title}
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
            <GuideMarkdown text={text.text} game={sectionKey === 'games' ? guide.key : undefined} />
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
