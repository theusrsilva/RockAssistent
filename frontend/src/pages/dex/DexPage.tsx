import { useEffect, useRef, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link, useSearchParams } from 'react-router'
import { api, spriteUrl } from '../../lib/api'
import { TYPES, saveDexGame, type SpeciesSummary } from '../../lib/dex'
import { GAME_VERSIONS } from '../../lib/gameVersions'
import { useDebouncedEffect } from '../../lib/useDebouncedEffect'
import { Button, Callout, Field, Icon, PageTitle, inputClass, selectClass } from '../../components/ui'
import { speciesEntryName } from './entryName'
import { TypeList, useTypeName } from './TypeBadge'

const pageSize = 60
const canObserve = typeof IntersectionObserver !== 'undefined'

/**
 * Lista da Pokédex: busca por nome ou número e filtros de tipo, geração e jogo (guardados no endereço). As formas
 * regionais aparecem cada uma no seu cartão, logo depois da espécie, e a lista continua sozinha ao chegar perto do fim.
 */
export default function DexPage() {
  const { t } = useTranslation()
  const typeName = useTypeName()
  const [params, setParams] = useSearchParams()
  const text = params.get('q') ?? ''
  const type = params.get('tipo') ?? ''
  const generation = params.get('geracao') ?? ''
  const game = params.get('jogo') ?? ''
  const [items, setItems] = useState<SpeciesSummary[] | null>(null)
  const [more, setMore] = useState(false)
  const [error, setError] = useState(false)
  const [loadingMore, setLoadingMore] = useState(false)
  const [moreError, setMoreError] = useState(false)
  const sentinel = useRef<HTMLDivElement>(null)
  // Cada busca nova invalida as páginas pedidas antes dela.
  const search = useRef(0)

  const setParam = (key: string, value: string) =>
    setParams(
      (p) => {
        const next = new URLSearchParams(p)
        if (value) next.set(key, value)
        else next.delete(key)
        return next
      },
      { replace: true },
    )

  const query = (offset: number) => {
    const q = new URLSearchParams({ offset: String(offset), limit: String(pageSize), forms: 'true' })
    if (text.trim()) q.set('text', text.trim())
    if (type) q.set('type', type)
    if (generation) q.set('generation', generation)
    if (game) q.set('game', game)
    return api.get<SpeciesSummary[]>(`/api/dex/species?${q}`)
  }

  useDebouncedEffect(
    (isCurrent) => {
      query(0)
        .then((list) => {
          if (!isCurrent()) return
          search.current++
          setItems(list)
          setMore(list.length === pageSize)
          setError(false)
          setMoreError(false)
          setLoadingMore(false)
        })
        .catch(() => isCurrent() && setError(true))
    },
    [text, type, generation, game],
    250,
  )

  const loadMore = () => {
    const current = items ?? []
    const id = search.current
    setLoadingMore(true)
    setMoreError(false)
    query(current.length)
      .then((list) => {
        if (id !== search.current) return
        setItems([...current, ...list])
        setMore(list.length === pageSize)
        setLoadingMore(false)
      })
      .catch(() => {
        if (id !== search.current) return
        setMoreError(true)
        setLoadingMore(false)
      })
  }

  // Carrega a próxima leva quando o fim da lista chega perto da tela. Depois de um erro, só pelo botão.
  const loadMoreRef = useRef(loadMore)
  loadMoreRef.current = loadMore
  const auto = more && !loadingMore && !moreError
  useEffect(() => {
    const node = sentinel.current
    if (!auto || !node || !canObserve) return
    const observer = new IntersectionObserver((entries) => entries.some((e) => e.isIntersecting) && loadMoreRef.current(), {
      rootMargin: '600px',
    })
    observer.observe(node)
    return () => observer.disconnect()
  }, [auto, items?.length])

  return (
    <div className="flex flex-col gap-5">
      <div>
        <PageTitle
          actions={
            <Link to="/pokedex/tipos" className="inline-flex items-center gap-1 text-sm font-semibold text-accent hover:underline">
              <Icon name="grid_view" className="text-[18px]" />
              {t('dex.chartLink')}
            </Link>
          }
        >
          {t('dex.title')}
        </PageTitle>
        <p className="mt-1 text-sm text-muted">{t('dex.sub')}</p>
      </div>

      <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
        <Field label={t('dex.search')}>
          <input
            className={inputClass}
            type="search"
            value={text}
            placeholder={t('dex.searchPlaceholder')}
            onChange={(e) => setParam('q', e.target.value)}
          />
        </Field>
        <Field label={t('dex.type')}>
          <select className={selectClass} value={type} onChange={(e) => setParam('tipo', e.target.value)}>
            <option value="">{t('dex.allTypes')}</option>
            {TYPES.map((ty) => (
              <option key={ty} value={ty}>
                {typeName(ty)}
              </option>
            ))}
          </select>
        </Field>
        <Field label={t('dex.generationLabel')}>
          <select className={selectClass} value={generation} onChange={(e) => setParam('geracao', e.target.value)}>
            <option value="">{t('dex.allGenerations')}</option>
            {[1, 2, 3, 4, 5, 6, 7, 8, 9].map((n) => (
              <option key={n} value={n}>
                {t('dex.generation', { n })}
              </option>
            ))}
          </select>
        </Field>
        <Field label={t('dex.gameFilter')}>
          <select className={selectClass} value={game} onChange={(e) => setParam('jogo', e.target.value)}>
            <option value="">{t('dex.allGames')}</option>
            {GAME_VERSIONS.map((g) => (
              <option key={g.key} value={g.key}>
                {g.name}
              </option>
            ))}
          </select>
        </Field>
      </div>

      {error && (
        <Callout tone="red" icon="error">
          {t('dex.error')}
        </Callout>
      )}
      {items === null && !error && <p className="text-sm text-muted">{t('common.loading')}</p>}
      {items?.length === 0 && <p className="text-sm text-muted">{t('dex.empty')}</p>}

      {items && items.length > 0 && (
        <div className="grid grid-cols-2 gap-2.5 sm:grid-cols-3 md:grid-cols-4 xl:grid-cols-6">
          {items.map((s) => (
            <Link
              key={`${s.id}-${s.formId ?? 0}`}
              to={s.formId ? `/pokedex/${s.id}?forma=${s.formId}` : `/pokedex/${s.id}`}
              onClick={() => game && saveDexGame(game)}
              className="flex flex-col items-center gap-1 rounded-xl border border-line bg-surface p-3 text-center hover:bg-surface2"
            >
              <img src={spriteUrl(s.id, { form: s.formId ?? undefined })} alt="" className="h-20 w-20 [image-rendering:pixelated]" loading="lazy" />
              <span className="font-mono text-xs text-muted">#{String(s.nationalNumber ?? s.id).padStart(4, '0')}</span>
              <span className="font-semibold">{speciesEntryName(s, t)}</span>
              <TypeList types={s.types} small />
            </Link>
          ))}
        </div>
      )}

      {more && (
        <div ref={sentinel} className="flex min-h-10 flex-col items-center gap-2">
          {loadingMore && <p className="text-sm text-muted">{t('common.loading')}</p>}
          {/* Navegador sem IntersectionObserver: a lista continua pelo botão. */}
          {!canObserve && !loadingMore && !moreError && (
            <Button variant="secondary" onClick={loadMore}>
              {t('dex.loadMore')}
            </Button>
          )}
          {moreError && (
            <>
              <p className="text-sm text-muted">{t('dex.moreError')}</p>
              <Button variant="secondary" onClick={loadMore}>
                {t('dex.retry')}
              </Button>
            </>
          )}
        </div>
      )}
    </div>
  )
}
