import { useEffect, useMemo, useRef, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link, useSearchParams } from 'react-router'
import { api, spriteUrl } from '../../lib/api'
import { TYPES, isDexGame, saveDexGame, type SpeciesSummary } from '../../lib/dex'
import {
  collectionProgress,
  loadCountVault,
  matchesCollectionFilter,
  ownedKey,
  ownedSource,
  parseCollectionImport,
  saveCountVault,
  useDexCollection,
  type CollectionFilter,
} from '../../lib/dexCollection'
import { GAME_VERSIONS } from '../../lib/gameVersions'
import { useDebouncedEffect } from '../../lib/useDebouncedEffect'
import { Button, Callout, Field, Icon, Modal, PageTitle, inputClass, selectClass } from '../../components/ui'
import { speciesEntryName } from './entryName'
import { OwnedButtons } from './OwnedButtons'
import { TypeList, useTypeName } from './TypeBadge'

/** Quantos cartões entram na tela de cada vez (a lista inteira vem numa busca só). */
const pageSize = 60
const canObserve = typeof IntersectionObserver !== 'undefined'
const filters: CollectionFilter[] = ['', 'tenho', 'faltam', 'shiny']

/**
 * Lista da Pokédex: busca por nome ou número e filtros de tipo, geração e jogo (guardados no endereço). As formas
 * regionais aparecem cada uma no seu cartão, logo depois da espécie; as outras formas, as fêmeas diferentes e as do
 * Unown, do Vivillon e do Alcremie, só quando ligadas (três caixas, desligadas por padrão). A lista continua sozinha ao
 * chegar perto do fim.
 *
 * Coleção: cada cartão tem os botões "tenho" e "tenho shiny". Há uma coleção geral e uma por jogo (a do jogo anda
 * junto com o filtro de jogo); o que está no cofre conta sozinho quando a opção está ligada.
 */
export default function DexPage() {
  const { t } = useTranslation()
  const typeName = useTypeName()
  const [params, setParams] = useSearchParams()
  const text = params.get('q') ?? ''
  const type = params.get('tipo') ?? ''
  const generation = params.get('geracao') ?? ''
  const game = params.get('jogo') ?? ''
  const otherForms = params.get('formas') === '1'
  const manyForms = params.get('muitas') === '1'
  const genderForms = params.get('genero') === '1'
  const scope = isDexGame(params.get('colecao')) ? (params.get('colecao') as string) : ''
  const show = (filters as string[]).includes(params.get('mostrar') ?? '') ? ((params.get('mostrar') ?? '') as CollectionFilter) : ''
  const [all, setAll] = useState<SpeciesSummary[] | null>(null)
  const [error, setError] = useState(false)
  const [shown, setShown] = useState(pageSize)
  const [countVault, setCountVault] = useState(loadCountVault)
  const sentinel = useRef<HTMLDivElement>(null)
  const collection = useDexCollection(scope)
  const [importOpen, setImportOpen] = useState(false)
  const [importText, setImportText] = useState('')
  const [importBusy, setImportBusy] = useState(false)
  const [importNote, setImportNote] = useState<{ ok: boolean; text: string } | null>(null)

  const closeImport = () => {
    setImportOpen(false)
    setImportText('')
    setImportNote(null)
  }
  const runImport = () => {
    const marks = parseCollectionImport(importText)
    if (!marks) {
      setImportNote({ ok: false, text: t('dex.collection.import.invalid') })
      return
    }
    setImportBusy(true)
    collection
      .importMarks(marks)
      .then((r) => setImportNote({ ok: true, text: t('dex.collection.import.done', { added: r.added, already: r.already, skipped: r.skipped }) }))
      .catch(() => setImportNote({ ok: false, text: t('dex.collection.import.failed') }))
      .finally(() => setImportBusy(false))
  }

  const update = (changes: Record<string, string>) =>
    setParams(
      (p) => {
        const next = new URLSearchParams(p)
        for (const [key, value] of Object.entries(changes)) {
          if (value) next.set(key, value)
          else next.delete(key)
        }
        return next
      },
      { replace: true },
    )
  const setParam = (key: string, value: string) => update({ [key]: value })
  // A coleção de um jogo é sempre a do jogo filtrado: os dois mudam juntos.
  const setGame = (key: string) => update(scope ? { jogo: key, colecao: key } : { jogo: key })
  const setScope = (key: string) => update(key ? { colecao: key, jogo: key } : { colecao: '' })

  useDebouncedEffect(
    (isCurrent) => {
      const q = new URLSearchParams({ limit: '2000', forms: 'true' })
      if (otherForms) q.set('otherForms', 'true')
      if (manyForms) q.set('manyForms', 'true')
      if (genderForms) q.set('genderForms', 'true')
      if (text.trim()) q.set('text', text.trim())
      if (type) q.set('type', type)
      if (generation) q.set('generation', generation)
      if (game) q.set('game', game)
      api
        .get<SpeciesSummary[]>(`/api/dex/species?${q}`)
        .then((list) => {
          if (!isCurrent()) return
          setAll(list)
          setShown(pageSize)
          setError(false)
        })
        .catch(() => isCurrent() && setError(true))
    },
    [text, type, generation, game, otherForms, manyForms, genderForms],
    250,
  )

  const progress = useMemo(() => collectionProgress(all ?? [], collection.sets, countVault), [all, collection.sets, countVault])
  const items = useMemo(
    () => (all ?? []).filter((s) => matchesCollectionFilter(s, show, collection.sets, countVault)),
    [all, show, collection.sets, countVault],
  )
  useEffect(() => setShown(pageSize), [show, scope])

  // Mostra a próxima leva quando o fim da lista chega perto da tela.
  const more = items.length > shown
  useEffect(() => {
    const node = sentinel.current
    if (!more || !node || !canObserve) return
    const observer = new IntersectionObserver(
      (entries) => entries.some((e) => e.isIntersecting) && setShown((n) => n + pageSize),
      { rootMargin: '600px' },
    )
    observer.observe(node)
    return () => observer.disconnect()
  }, [more, shown])

  const chip = 'inline-flex cursor-pointer items-center gap-1.5 rounded-full border border-line bg-surface px-3 py-1 text-[13px] select-none'

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
          <select className={selectClass} value={game} onChange={(e) => setGame(e.target.value)}>
            <option value="">{t('dex.allGames')}</option>
            {GAME_VERSIONS.map((g) => (
              <option key={g.key} value={g.key}>
                {g.name}
              </option>
            ))}
          </select>
        </Field>
        <Field label={t('dex.collection.scope')}>
          <select className={selectClass} value={scope} onChange={(e) => setScope(e.target.value)}>
            <option value="">{t('dex.collection.general')}</option>
            {GAME_VERSIONS.map((g) => (
              <option key={g.key} value={g.key}>
                {t('dex.collection.ofGame', { game: g.name })}
              </option>
            ))}
          </select>
        </Field>
        <Field label={t('dex.collection.show')}>
          <select className={selectClass} value={show} onChange={(e) => setParam('mostrar', e.target.value)}>
            {filters.map((f) => (
              <option key={f} value={f}>
                {t(`dex.collection.filter.${f || 'all'}`)}
              </option>
            ))}
          </select>
        </Field>
        <div className="flex flex-wrap items-end gap-2 sm:col-span-2">
          <label className={chip}>
            <input type="checkbox" className="accent-accent" checked={otherForms} onChange={(e) => setParam('formas', e.target.checked ? '1' : '')} />
            {t('dex.otherForms')}
          </label>
          <label className={chip}>
            <input type="checkbox" className="accent-accent" checked={genderForms} onChange={(e) => setParam('genero', e.target.checked ? '1' : '')} />
            {t('dex.genderForms')}
          </label>
          <label className={chip}>
            <input type="checkbox" className="accent-accent" checked={manyForms} onChange={(e) => setParam('muitas', e.target.checked ? '1' : '')} />
            {t('dex.manyForms')}
          </label>
          <label className={chip} title={t('dex.collection.countVaultHelp')}>
            <input
              type="checkbox"
              className="accent-accent"
              checked={countVault}
              onChange={(e) => {
                setCountVault(e.target.checked)
                saveCountVault(e.target.checked)
              }}
            />
            {t('dex.collection.countVault')}
          </label>
          <Button variant="secondary" size="sm" icon="upload_file" onClick={() => setImportOpen(true)}>
            {t('dex.collection.import.button')}
          </Button>
        </div>
      </div>

      {error && (
        <Callout tone="red" icon="error">
          {t('dex.error')}
        </Callout>
      )}
      {collection.error && (
        <Callout tone="amber" icon="warning">
          {t('dex.collection.error')}
        </Callout>
      )}
      {importOpen && (
        <Modal
          wide
          title={t('dex.collection.import.title')}
          actions={
            <>
              <Button variant="secondary" onClick={closeImport}>
                {t('common.close')}
              </Button>
              <Button icon="upload_file" disabled={importBusy || importText.trim() === ''} onClick={runImport}>
                {t('dex.collection.import.confirm')}
              </Button>
            </>
          }
        >
          <div className="flex flex-col gap-3">
            <p>
              {t('dex.collection.import.help', {
                collection: scope
                  ? t('dex.collection.ofGame', { game: GAME_VERSIONS.find((g) => g.key === scope)?.name ?? scope })
                  : t('dex.collection.general'),
              })}
            </p>
            <input
              type="file"
              accept=".json,application/json,text/plain"
              aria-label={t('dex.collection.import.file')}
              className="text-sm"
              onChange={(e) => {
                const file = e.target.files?.[0]
                if (!file) return
                void file.text().then((text) => {
                  setImportText(text)
                  setImportNote(null)
                })
              }}
            />
            <textarea
              className={`${inputClass} h-32 py-2 font-mono text-xs`}
              aria-label={t('dex.collection.import.paste')}
              placeholder={t('dex.collection.import.paste')}
              value={importText}
              onChange={(e) => {
                setImportText(e.target.value)
                setImportNote(null)
              }}
            />
            {importNote && (
              <Callout tone={importNote.ok ? 'emerald' : 'red'} icon={importNote.ok ? 'check_circle' : 'error'}>
                {importNote.text}
              </Callout>
            )}
          </div>
        </Modal>
      )}
      {all === null && !error && <p className="text-sm text-muted">{t('common.loading')}</p>}

      {all && all.length > 0 && (
        <p className="text-sm text-muted" aria-live="polite">
          {t('dex.collection.progress', { owned: progress.owned, total: progress.total, shiny: progress.shiny })}
        </p>
      )}
      {all && items.length === 0 && <p className="text-sm text-muted">{t('dex.empty')}</p>}

      {items.length > 0 && (
        <div className="grid grid-cols-2 gap-2.5 sm:grid-cols-3 md:grid-cols-4 xl:grid-cols-6">
          {items.slice(0, shown).map((s) => {
            const normal = ownedSource(collection.sets, countVault, ownedKey(s.id, s.formId, false))
            const shiny = ownedSource(collection.sets, countVault, ownedKey(s.id, s.formId, true))
            const page = new URLSearchParams()
            if (s.formId) page.set('forma', String(s.formId))
            if (scope) page.set('colecao', scope)
            const query = page.toString()
            return (
              <div
                key={`${s.id}-${s.formId ?? 0}`}
                className={`relative rounded-xl border ${normal || shiny ? 'border-ok bg-ok-soft' : 'border-line bg-surface'}`}
              >
                <Link
                  to={`/pokedex/${s.id}${query ? `?${query}` : ''}`}
                  onClick={() => game && saveDexGame(game)}
                  className="flex flex-col items-center gap-1 rounded-xl p-3 pt-9 text-center hover:bg-surface2/60"
                >
                  <img
                    src={spriteUrl(s.id, { form: s.formId ?? undefined, shiny: !normal && !!shiny })}
                    alt=""
                    className="h-20 w-20 [image-rendering:pixelated]"
                    loading="lazy"
                  />
                  <span className="font-mono text-xs text-muted">#{String(s.nationalNumber ?? s.id).padStart(4, '0')}</span>
                  <span className="font-semibold">{speciesEntryName(s, t)}</span>
                  <TypeList types={s.types} small />
                </Link>
                <span className="absolute top-1.5 right-1.5">
                  <OwnedButtons
                    speciesId={s.id}
                    formId={s.formId}
                    sets={collection.sets}
                    countVault={countVault}
                    onToggle={(isShiny, owned) => collection.setMark(s.id, s.formId, isShiny, owned)}
                  />
                </span>
              </div>
            )
          })}
        </div>
      )}

      {more && (
        <div ref={sentinel} className="flex min-h-10 justify-center">
          {/* Navegador sem IntersectionObserver: a lista continua pelo botão. */}
          {!canObserve && (
            <Button variant="secondary" onClick={() => setShown((n) => n + pageSize)}>
              {t('dex.loadMore')}
            </Button>
          )}
        </div>
      )}
    </div>
  )
}
