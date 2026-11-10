import { useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { useNavigate } from 'react-router'
import { LegalityPanel } from '../../components/Legality'
import { SearchSelect } from '../../components/SearchSelect'
import { Badge, Button, Callout, HelpTip, Icon, IconButton, inputClass, selectClass, useToast } from '../../components/ui'
import { ApiError, api, spriteUrl } from '../../lib/api'
import { optionText, useBuilderData } from '../../lib/builderData'
import { familyOf, formatName } from '../../lib/games'
import { requestTrade } from '../../lib/tradeRequest'
import type {
  AdvancedResult,
  BoxSummary,
  BuildResult,
  LegalityReport,
  Option,
  PokemonSpec,
  RawPokemon,
  SpeciesDetails,
  Tag,
  TagColor,
  VaultPokemon,
} from '../../lib/types'
import { tagClass, tagColors } from './tagColors'
import { AdaptDialog } from './AdaptDialog'
import { TransferDialog } from './TransferDialog'
import { ComingSoonActions } from '../boxes/ComingSoonActions'
import { EvolveDialog } from '../boxes/EvolveDialog'
import { errorMessage, evolveBody, type EvolveLegality } from '../boxes/boxes'

type TabKey = 'resumo' | 'golpes' | 'status' | 'origem'
const STAT_KEYS = ['hp', 'atk', 'def', 'spa', 'spd', 'spe'] as const

/** Resumo de leitura dos formatos que o construtor não abre (Gen 1, 2 e 4 a 7; D34). */
interface LegacyView {
  moves: string[]
  ivs: number[]
  evs: number[]
  maxEv: number
  nature: string | null
  ability: string | null
  heldItem: string | null
  ball: string | null
  language: string
  metLevel: number | null
}

interface Draft {
  nickname: string
  level: string
  nature: number
  heldItem: number
  moves: number[]
  evs: number[]
}

/** Painel do Pokémon escolhido no cofre: resumo, golpes, status, origem, etiquetas, ações e edição. */
export function Details({
  pokemon,
  boxes,
  tags,
  onChanged,
  onTagsChanged,
  onExport,
  onMove,
  onMoveToSave,
  onClose,
}: {
  pokemon: VaultPokemon
  boxes: BoxSummary[]
  tags: Tag[]
  onChanged: () => void
  onTagsChanged: () => void
  onExport: (ids: string[]) => void
  onMove: (id: string, target: string) => void
  /** Só na janela do PC (tela Boxes): abre o Mover/Copiar/Anexar para um save. */
  onMoveToSave?: () => void
  onClose: () => void
}) {
  const { t, i18n } = useTranslation()
  const navigate = useNavigate()
  const toast = useToast()
  const [tab, setTab] = useState<TabKey>('resumo')
  // raw (dados brutos) só existe no FireRed/LeafGreen; nos jogos novos o painel usa o pedido do construtor (spec).
  const [loaded, setLoaded] = useState<{ id: string; spec: PokemonSpec; raw: RawPokemon | null } | null>(null)
  const data = useBuilderData(loaded?.spec.game ?? 'fire_red')
  const gen3 = familyOf(pokemon) === 'frlg'
  // Gen 1, 2 e 4 a 7: ficam no cofre, mas não trocam nem abrem no construtor (adapte para um jogo do Switch).
  const legacy = pokemon.tradable === false
  const [legacyView, setLegacyView] = useState<{ id: string; view: LegacyView } | null>(null)
  const [species, setSpecies] = useState<SpeciesDetails | null>(null)
  const [locations, setLocations] = useState<Option[]>([])
  const [report, setReport] = useState<LegalityReport | null>(null)
  const [newTag, setNewTag] = useState('')
  const [color, setColor] = useState<TagColor>('sky')
  const [draft, setDraft] = useState<Draft | null>(null)
  const [saving, setSaving] = useState(false)
  const [editError, setEditError] = useState<string | null>(null)
  const [transferOpen, setTransferOpen] = useState(false)
  const [adaptOpen, setAdaptOpen] = useState(false)
  const [evolveOpen, setEvolveOpen] = useState(false)
  const [cloning, setCloning] = useState(false)

  async function cloneInVault() {
    setCloning(true)
    try {
      const r = await api.post<{ pokemon: VaultPokemon; legality: EvolveLegality }>(
        `/api/vault/pokemon/${pokemon.id}/clone`,
        { language: i18n.language },
      )
      onChanged()
      toast(r.legality.valid ? t('boxes.clonedVault') : t('boxes.clonedVaultIllegal'))
    } catch (e) {
      toast(errorMessage(t, e))
    } finally {
      setCloning(false)
    }
  }

  // Dados completos (golpes, IVs, EVs, local) vêm do arquivo do Pokémon.
  useEffect(() => {
    setReport(null)
    setDraft(null)
    setEditError(null)
    setLoaded(null)
    setLegacyView(null)
    let alive = true
    if (legacy)
      api
        .get<LegacyView>(`/api/vault/pokemon/${pokemon.id}/view`)
        .then((view) => alive && setLegacyView({ id: pokemon.id, view }))
        .catch(() => undefined)
    else
      api
        .get<{ spec: PokemonSpec; raw: RawPokemon | null }>(`/api/builder/from/${pokemon.id}`)
        .then((r) => alive && setLoaded({ id: pokemon.id, ...r }))
        .catch(() => undefined)
    return () => {
      alive = false
    }
  }, [pokemon.id, pokemon.sha256, legacy])

  const specGame = loaded?.spec.game
  useEffect(() => {
    const query = specGame ? `?game=${specGame}&form=${pokemon.form ?? 0}` : ''
    api.get<SpeciesDetails>(`/api/builder/species/${pokemon.species}${query}`).then(setSpecies).catch(() => setSpecies(null))
  }, [pokemon.species, pokemon.form, specGame])

  const version = loaded?.raw?.version
  useEffect(() => {
    if (!version) return
    api.get<Option[]>(`/api/builder/locations?version=${version}`).then(setLocations).catch(() => setLocations([]))
  }, [version])

  const spec = loaded?.id === pokemon.id ? loaded.spec : null
  const raw = loaded?.id === pokemon.id ? loaded.raw : null
  // O que o painel mostra, venha dos dados brutos (Gen 3) ou do pedido do construtor (jogos novos).
  const view = spec
    ? raw
      ? { moves: raw.moves, ivs: raw.ivs, evs: raw.evs, heldItem: raw.heldItem, language: raw.language, abilitySlot: raw.abilityBit ? 1 : 0, ball: raw.ball, level: raw.level }
      : { moves: spec.moves, ivs: spec.ivs ?? [0, 0, 0, 0, 0, 0], evs: spec.evs, heldItem: spec.heldItem, language: spec.language, abilitySlot: spec.abilitySlot ?? 0, ball: spec.ball ?? 0, level: spec.level }
    : null
  const maxEv = data?.maxEv ?? 255
  const trash = !!pokemon.deletedAt
  const name = (pokemon.nickname || pokemon.speciesName).toUpperCase()
  const place = trash
    ? t('vault.trash')
    : pokemon.boxId
      ? t('vault.slotLabel', { box: boxes.find((b) => b.id === pokemon.boxId)?.name ?? '?', slot: (pokemon.slot ?? 0) + 1 })
      : t('vault.inbox')
  const legal = pokemon.legality
  const legalLook =
    legal === 'legal'
      ? { tone: 'emerald' as const, icon: 'verified', label: t('legality.legal') }
      : legal === 'illegal'
        ? { tone: 'red' as const, icon: 'error', label: t('legality.illegal') }
        : legal === 'unverifiable'
          ? { tone: 'amber' as const, icon: 'help', label: t('legality.unverifiable') }
          : { tone: 'slate' as const, icon: 'help', label: t('legality.unknown') }
  const gender = pokemon.gender === 0 ? '♂' : pokemon.gender === 1 ? '♀' : '—'
  const dash = '—'
  // Da Gen 7 em diante o jogo mostra o TID de 6 dígitos.
  const sixDigits = (pokemon.generation ?? (gen3 ? 3 : 8)) >= 7
  const shownLegacy = legacyView?.id === pokemon.id ? legacyView.view : null

  async function toggleTag(tagId: string, add: boolean) {
    await api.post('/api/vault/tags/assign', { pokemonIds: [pokemon.id], add: add ? [tagId] : [], remove: add ? [] : [tagId] })
    onChanged()
  }

  async function addTag() {
    const text = newTag.trim()
    if (!text) return
    const existing = tags.find((x) => x.name.toLowerCase() === text.toLowerCase())
    const tag = existing ?? (await api.post<Tag>('/api/vault/tags', { name: text, color }))
    await api.post('/api/vault/tags/assign', { pokemonIds: [pokemon.id], add: [tag.id] })
    setNewTag('')
    if (!existing) onTagsChanged()
    onChanged()
  }

  function startEdit() {
    if (!spec || !view) return
    setEditError(null)
    setDraft({
      nickname: spec.nickname ?? '',
      level: String(view.level),
      nature: spec.nature ?? 0,
      heldItem: view.heldItem,
      moves: [0, 1, 2, 3].map((i) => view.moves[i] ?? 0),
      evs: [...view.evs],
    })
  }

  const evTotal = draft ? draft.evs.reduce((a, b) => a + b, 0) : 0
  const levelNumber = draft ? Number(draft.level) : 0
  const draftValid =
    !!draft && evTotal <= 510 && draft.evs.every((v) => v <= maxEv) && Number.isInteger(levelNumber) && levelNumber >= 1 && levelNumber <= 100

  /**
   * Natureza igual: monta pelo editor avançado (PID, IVs, treinador e origem continuam idênticos).
   * Natureza diferente: na Gen 3 ela sai do PID, então o Pokémon é gerado de novo como no Criar.
   */
  async function saveEdit() {
    if (!draft || !spec || !draftValid) return
    setSaving(true)
    setEditError(null)
    const nickname = draft.nickname.trim() || null
    try {
      let bytes: string | null = null
      // Jogos novos: sempre gerado de novo pelo construtor (não há editor avançado fora da Gen 3).
      if (raw && draft.nature === spec.nature) {
        const next: RawPokemon = { ...raw, nickname, level: levelNumber, heldItem: draft.heldItem, moves: draft.moves, evs: draft.evs }
        const r = await api.post<AdvancedResult>('/api/builder/advanced', { raw: next, language: i18n.language })
        if (r.errors.length > 0) throw new Error(r.errors.map((e) => t(`builder.errors.${e}`, { defaultValue: e })).join(' '))
        bytes = r.data
      } else {
        const next: PokemonSpec = {
          ...spec,
          nickname,
          level: levelNumber,
          nature: draft.nature,
          heldItem: draft.heldItem,
          moves: draft.moves.filter((m) => m > 0),
          evs: draft.evs,
        }
        const r = await api.post<BuildResult>('/api/builder/generate', { spec: next, language: i18n.language })
        if (r.errors.length > 0) throw new Error(r.errors.map((e) => t(`builder.errors.${e}`, { defaultValue: e })).join(' '))
        if (!r.usable) throw new Error(t(r.generation === 'timeout' ? 'builder.genTimeout' : 'builder.genFailed'))
        bytes = r.data
      }
      if (!bytes) throw new Error(t('common.error'))
      const saved = await api.post<VaultPokemon>(`/api/vault/pokemon/${pokemon.id}/replace`, { data: bytes })
      setDraft(null)
      onChanged()
      toast(saved.legality === 'illegal' ? t('ui.vault.savedIllegal') : t('ui.vault.saved'))
    } catch (e) {
      setEditError(
        e instanceof ApiError ? t(`vault.errors.${e.code}`, { defaultValue: t(`ui.vault.errors.${e.code}`, { defaultValue: t('common.error') }) }) : (e as Error).message,
      )
    } finally {
      setSaving(false)
    }
  }

  function useInTrade() {
    requestTrade([pokemon.id])
    navigate(`/trocas/trocar?pokemon=${pokemon.id}`)
  }

  const statLabel = (i: number) => t(`ui.stats.${STAT_KEYS[i]}`)
  const row = (k: string, v: React.ReactNode) => (
    <div key={k} className="flex gap-2.5 text-sm">
      <span className="w-[110px] shrink-0 text-muted">{k}</span>
      <span className="min-w-0 flex-1 font-semibold [overflow-wrap:anywhere]">{v}</span>
    </div>
  )

  const tabs: { key: TabKey; label: string }[] = [
    { key: 'resumo', label: t('ui.vault.tabs.resumo') },
    { key: 'golpes', label: t('ui.vault.tabs.golpes') },
    { key: 'status', label: t('ui.vault.tabs.status') },
    { key: 'origem', label: t('ui.vault.tabs.origem') },
  ]
  const assigned = tags.filter((x) => pokemon.tagIds.includes(x.id))
  const unassigned = tags.filter((x) => !pokemon.tagIds.includes(x.id))

  return (
    <div className="flex min-h-0 flex-col">
      {evolveOpen && (
        <EvolveDialog
          name={pokemon.nickname || pokemon.speciesName}
          optionsUrl={`/api/vault/pokemon/${pokemon.id}/evolve-options`}
          submit={async (option) => {
            const r = await api.post<{ pokemon: VaultPokemon; legality: EvolveLegality }>(
              `/api/vault/pokemon/${pokemon.id}/evolve`,
              evolveBody(option, i18n.language),
            )
            onChanged()
            toast(t('boxes.evolveDialog.vaultDone', { species: option.speciesName }))
            return r.legality
          }}
          onClose={() => setEvolveOpen(false)}
        />
      )}
      {adaptOpen && <AdaptDialog pokemon={pokemon} onDone={() => onChanged()} onClose={() => setAdaptOpen(false)} />}
      {transferOpen && (
        <TransferDialog
          pokemon={pokemon}
          onClose={() => setTransferOpen(false)}
          onCopied={() => onChanged()}
          onMoved={() => onChanged()}
          onDone={(moved) => {
            setTransferOpen(false)
            onChanged()
            toast(t('vault.transfer.done', { game: formatName(moved.family) }))
          }}
        />
      )}
      <div className="flex items-center gap-2 border-b border-line px-3.5 py-3">
        <span className="flex-1 text-[13px] text-muted">{place}</span>
        <button
          type="button"
          onClick={onClose}
          aria-label={t('common.close')}
          className="flex h-[30px] w-[30px] cursor-pointer items-center justify-center rounded-full text-muted hover:bg-surface2"
        >
          <Icon name="close" />
        </button>
      </div>

      <div className="flex items-center gap-3.5 p-4">
        <div className="flex h-[104px] w-[104px] shrink-0 items-center justify-center rounded-lg bg-tile">
          <img src={spriteUrl(pokemon.species, { shiny: pokemon.isShiny })} alt="" className="pixelated h-24 w-24" />
        </div>
        <div className="flex min-w-0 flex-col gap-1">
          <div className="font-display text-[21px] leading-tight font-bold [overflow-wrap:anywhere]">{name}</div>
          <div className="text-[13px] text-muted">
            #{String(pokemon.species).padStart(3, '0')} {pokemon.speciesName} · {pokemon.isEgg ? t('pokemon.egg') : t('ui.lv', { level: pokemon.level })}
          </div>
          <div className="mt-0.5 flex flex-wrap gap-1.5">
            <Badge tone={legalLook.tone} icon={legalLook.icon}>
              {legalLook.label}
            </Badge>
            {pokemon.isShiny && (
              <Badge tone="amber" icon="auto_awesome">
                {t('pokemon.shiny')}
              </Badge>
            )}
            <Badge>{t(`pokemon.origin.${pokemon.origin}`)}</Badge>
            <Badge icon="sports_esports">{formatName(familyOf(pokemon))}</Badge>
          </div>
        </div>
      </div>

      <div className="flex flex-col gap-2 px-4 pb-3 empty:hidden">
        {pokemon.warnings.map((w, i) => (
          <Callout key={i} tone="amber" icon="warning" className="py-2 text-[13px]">
            {t(`warnings.${w.code}`, w.args)}
          </Callout>
        ))}
        {legacy && (
          <Callout tone="sky" icon="history" className="py-2 text-[13px]">
            {t('vault.legacy', { game: formatName(familyOf(pokemon)), generation: pokemon.generation })}
          </Callout>
        )}
        {pokemon.possibleDuplicate && (
          <Callout tone="violet" icon="content_copy" className="py-2 text-[13px]">
            <b>{t('vault.duplicate')}:</b> {t('vault.duplicateHelp')}
          </Callout>
        )}
      </div>

      {!draft ? (
        <>
          <div className="flex gap-0.5 scroll-x border-b border-line px-3" role="tablist">
            {tabs.map((x) => (
              <button
                key={x.key}
                type="button"
                role="tab"
                aria-selected={tab === x.key}
                onClick={() => setTab(x.key)}
                className={`h-[38px] shrink-0 cursor-pointer border-b-2 px-2.5 text-[13.5px] font-semibold ${tab === x.key ? 'border-accent text-ink' : 'border-transparent text-muted'}`}
              >
                {x.label}
              </button>
            ))}
          </div>
          <div className="flex min-h-[190px] flex-col gap-2 px-4 py-3.5">
            {legacy ? (
              !shownLegacy ? (
                <p className="text-sm text-muted">{t('common.loading')}</p>
              ) : (
                <LegacyTab
                  tab={tab}
                  view={shownLegacy}
                  pokemon={pokemon}
                  row={row}
                  gender={gender}
                  sixDigits={sixDigits}
                  statLabel={statLabel}
                  report={report}
                  onToggleReport={() =>
                    report ? setReport(null) : void api.get<LegalityReport>(`/api/pokemon/${pokemon.id}/legality?lang=${i18n.language}`).then(setReport)
                  }
                />
              )
            ) : !spec || !view ? (
              <p className="text-sm text-muted">{t('common.loading')}</p>
            ) : tab === 'resumo' ? (
              <>
                {row(t('builder.fields.nature'), optionText(data?.natures, spec.nature) ?? dash)}
                {row(t('builder.fields.ability'), species?.abilities.find((a) => a.value === view.abilitySlot)?.text ?? dash)}
                {row(t('builder.fields.heldItem'), view.heldItem ? (optionText(data?.items, view.heldItem) ?? `#${view.heldItem}`) : t('ui.vault.none'))}
                {row(t('builder.fields.gender'), gender)}
                {row(t('ui.vault.ot'), `${pokemon.otName} (${String(pokemon.tid).padStart(sixDigits ? 6 : 5, '0')})`)}
                {row(t('builder.fields.language'), optionText(data?.languages, view.language) ?? dash)}
                {row(t('vault.game'), formatName(familyOf(pokemon)))}
              </>
            ) : tab === 'golpes' ? (
              view.moves.filter((m) => m > 0).length === 0 ? (
                <p className="text-sm text-muted">{dash}</p>
              ) : (
                view.moves
                  .filter((m) => m > 0)
                  .map((m, i) => (
                    <div key={`${m}-${i}`} className="flex h-[38px] items-center rounded-lg bg-surface2 px-3 text-sm font-semibold">
                      {optionText(data?.moves, m) ?? `#${m}`}
                    </div>
                  ))
              )
            ) : tab === 'status' ? (
              <>
                <div className="flex items-center gap-2 text-[11.5px] text-muted">
                  <span className="w-11" />
                  <span className="flex-1">{t('ui.vault.ivEv')}</span>
                  <HelpTip>{t('ui.vault.ivEvTip')}</HelpTip>
                </div>
                {STAT_KEYS.map((_, i) => (
                  <div key={i} className="flex items-center gap-2 text-[13px]">
                    <span className="w-11 text-muted">{statLabel(i)}</span>
                    <div className="flex flex-1 flex-col gap-[3px]">
                      <div className="h-1.5 rounded-sm bg-surface2">
                        <div className="h-full rounded-sm bg-accent" style={{ width: `${(view.ivs[i] / 31) * 100}%` }} />
                      </div>
                      <div className="h-1.5 rounded-sm bg-surface2">
                        <div className="h-full rounded-sm bg-info" style={{ width: `${(view.evs[i] / maxEv) * 100}%` }} />
                      </div>
                    </div>
                    <span className="w-14 text-right font-mono text-xs">
                      {view.ivs[i]}·{view.evs[i]}
                    </span>
                  </div>
                ))}
              </>
            ) : (
              <>
                {row(t('ui.vault.originLabel'), t(`pokemon.origin.${pokemon.origin}`))}
                {row(t('builder.fields.version'), pokemon.originGame)}
                {raw && row(t('builder.fields.metLocation'), optionText(locations, raw.metLocation) ?? `#${raw.metLocation}`)}
                {raw && row(t('builder.fields.metLevel'), raw.metLevel || t('ui.vault.hatched'))}
                {row(t('builder.fields.ball'), optionText(data?.balls, view.ball) ?? dash)}
                {pokemon.fateful && row(t('builder.fields.fateful'), '✓')}
                {pokemon.sourceFileName && row(t('ui.vault.file'), pokemon.sourceFileName)}
                {row(t('ui.vault.added'), new Date(pokemon.addedAt).toLocaleString(i18n.language, { dateStyle: 'short', timeStyle: 'short' }))}
                <button
                  type="button"
                  className="mt-1 flex cursor-pointer items-center gap-1 self-start text-[13.5px] font-semibold text-accent"
                  onClick={() =>
                    report ? setReport(null) : void api.get<LegalityReport>(`/api/pokemon/${pokemon.id}/legality?lang=${i18n.language}`).then(setReport)
                  }
                >
                  <Icon name={report ? 'expand_less' : 'expand_more'} className="text-[18px]" />
                  {report ? t('legality.hide') : t('legality.view')}
                </button>
                {report && (
                  <div className="rounded-lg bg-surface2 p-3">
                    <LegalityPanel report={report} />
                  </div>
                )}
              </>
            )}

            <div className="mt-1.5 flex flex-wrap items-center gap-1.5 border-t border-dashed border-line pt-2.5">
              {assigned.map((tag) => (
                <span key={tag.id} className={`inline-flex h-[26px] items-center gap-1 rounded-full py-0 pr-1 pl-2.5 text-[12.5px] font-semibold ${tagClass[tag.color]}`}>
                  {tag.name}
                  <button
                    type="button"
                    aria-label={t('ui.vault.removeTag', { name: tag.name })}
                    onClick={() => void toggleTag(tag.id, false)}
                    className="flex h-[18px] w-[18px] cursor-pointer items-center justify-center rounded-full opacity-70 hover:opacity-100"
                  >
                    <Icon name="close" className="text-sm" />
                  </button>
                </span>
              ))}
              {unassigned.map((tag) => (
                <button
                  key={tag.id}
                  type="button"
                  onClick={() => void toggleTag(tag.id, true)}
                  className="h-[26px] cursor-pointer rounded-full border border-dashed border-line px-2.5 text-[12.5px] text-muted hover:text-ink"
                >
                  + {tag.name}
                </button>
              ))}
              <span className="inline-flex items-center gap-1">
                <input
                  value={newTag}
                  maxLength={24}
                  onChange={(e) => setNewTag(e.target.value)}
                  onKeyDown={(e) => e.key === 'Enter' && void addTag()}
                  placeholder={t('ui.vault.newTag')}
                  aria-label={t('vault.tagName')}
                  className="h-[26px] w-24 rounded-full border border-dashed border-line bg-transparent px-2.5 text-[12.5px] outline-none focus:border-accent"
                />
                {newTag.trim() && !tags.some((x) => x.name.toLowerCase() === newTag.trim().toLowerCase()) && (
                  <span className="flex gap-0.5" role="radiogroup" aria-label={t('ui.vault.tagColor')}>
                    {tagColors.map((c) => (
                      <button
                        key={c}
                        type="button"
                        role="radio"
                        aria-checked={color === c}
                        aria-label={c}
                        onClick={() => setColor(c)}
                        className={`h-4 w-4 cursor-pointer rounded-full ${tagClass[c]} ${color === c ? 'ring-2 ring-accent ring-offset-1 ring-offset-surface' : ''}`}
                      />
                    ))}
                  </span>
                )}
              </span>
            </div>
          </div>

          <div className="flex flex-col gap-2 border-t border-line px-4 py-3.5">
            {trash ? (
              <Button icon="restore_from_trash" onClick={() => void api.post('/api/vault/restore', { ids: [pokemon.id] }).then(onChanged)}>
                {t('vault.restore')}
              </Button>
            ) : (
              <>
                <div className="flex gap-2">
                  <Button
                    className="h-[42px] flex-1"
                    icon="edit"
                    disabled={legacy || !spec || !view || pokemon.isEgg}
                    title={legacy ? t('vault.legacyNoEdit') : undefined}
                    onClick={startEdit}
                  >
                    {t('ui.vault.edit')}
                  </Button>
                  <Button
                    className="h-[42px] flex-1"
                    variant="secondary"
                    icon="swap_horiz"
                    disabled={legacy}
                    title={legacy ? t('vault.legacyNoTrade') : undefined}
                    onClick={useInTrade}
                  >
                    {t('ui.vault.useInTrade')}
                  </Button>
                </div>
                {/* Gen 1 a 7 e Let's Go: caminhos oficiais para um jogo mais novo (D34); o original fica. */}
                {(pokemon.generation ?? 3) <= 7 && (
                  <Button className="h-9" variant="secondary" size="sm" icon="upgrade" disabled={pokemon.isEgg} title={pokemon.isEgg ? t('vault.adapt.refusal.egg') : undefined} onClick={() => setAdaptOpen(true)}>
                    {t('vault.adapt.action')}
                  </Button>
                )}
                {onMoveToSave && (
                  <Button className="h-9" variant="secondary" size="sm" icon="drive_file_move" onClick={onMoveToSave}>
                    {t('boxes.moveToSave')}
                  </Button>
                )}
                {/* Clonar no cofre: cópia com PID novo na Caixa de entrada. */}
                <ComingSoonActions
                  onClone={pokemon.isEgg || cloning ? undefined : () => void cloneInVault()}
                  cloneHint={t('boxes.evolveDialog.egg')}
                  evolveHint={t('boxes.evolveDialog.egg')}
                  onEvolve={pokemon.isEgg ? undefined : () => setEvolveOpen(true)}
                />
                <div className="flex items-center gap-1.5">
                  <select
                    className={`${selectClass} h-9! flex-1 text-[13px]!`}
                    value=""
                    onChange={(e) => e.target.value && onMove(pokemon.id, e.target.value)}
                    aria-label={t('vault.moveTo')}
                  >
                    <option value="">{t('vault.moveTo')}</option>
                    {pokemon.boxId && <option value="inbox">{t('vault.inbox')}</option>}
                    {boxes
                      .filter((b) => b.id !== pokemon.boxId)
                      .map((b) => (
                        <option key={b.id} value={b.id}>
                          {b.name}
                        </option>
                      ))}
                  </select>
                  <IconButton name="file_download" label={gen3 ? t('ui.vault.exportPk3') : t('ui.vault.exportFile')} onClick={() => onExport([pokemon.id])} />
                  {!legacy && <IconButton name="move_up" label={t('vault.transfer.action')} onClick={() => setTransferOpen(true)} />}
                  {!legacy && <IconButton name="tune" label={t('pokemon.editInBuilder')} onClick={() => navigate(`/criar/${pokemon.id}`)} />}
                  <IconButton name="menu_book" label={t('boxes.viewDex')} onClick={() => navigate(`/pokedex/${pokemon.species}`)} />
                  <IconButton
                    name="delete"
                    label={t('vault.toTrash')}
                    className="text-bad"
                    onClick={() => void api.post('/api/vault/trash', { ids: [pokemon.id] }).then(onChanged)}
                  />
                </div>
              </>
            )}
          </div>
        </>
      ) : (
        <>
          <div className="flex flex-col gap-3 overflow-x-hidden overflow-y-auto px-4 pt-1 pb-3.5 md:max-h-[520px]">
            <div className="grid grid-cols-[minmax(0,1fr)_84px] gap-2">
              <label className="flex min-w-0 flex-col gap-1">
                <span className="text-[12.5px] font-bold">{t('builder.fields.nickname')}</span>
                <input
                  className={`${inputClass} h-[38px]!`}
                  value={draft.nickname}
                  maxLength={10}
                  placeholder={pokemon.speciesName}
                  onChange={(e) => setDraft({ ...draft, nickname: e.target.value })}
                />
              </label>
              <label className="flex min-w-0 flex-col gap-1">
                <span className="text-[12.5px] font-bold">{t('builder.fields.level')}</span>
                <input
                  className={`${inputClass} h-[38px]!`}
                  inputMode="numeric"
                  value={draft.level}
                  onChange={(e) => setDraft({ ...draft, level: e.target.value.replace(/\D/g, '').slice(0, 3) })}
                />
              </label>
            </div>
            <div className="grid grid-cols-2 gap-2">
              <label className="flex min-w-0 flex-col gap-1">
                <span className="flex items-center gap-1.5 text-[12.5px] font-bold">
                  {t('builder.fields.nature')}
                  <HelpTip>{t('ui.vault.natureTip')}</HelpTip>
                </span>
                <select className={`${selectClass} h-[38px]!`} value={draft.nature} onChange={(e) => setDraft({ ...draft, nature: Number(e.target.value) })}>
                  {data?.natures.map((n) => (
                    <option key={n.value} value={n.value}>
                      {n.text}
                    </option>
                  ))}
                </select>
              </label>
              <div className="flex min-w-0 flex-col gap-1">
                <span className="text-[12.5px] font-bold">{t('builder.fields.heldItem')}</span>
                <SearchSelect
                  options={data?.items.filter((o) => o.value > 0) ?? []}
                  value={draft.heldItem || null}
                  onChange={(v) => setDraft({ ...draft, heldItem: v ?? 0 })}
                  emptyLabel={t('ui.vault.none')}
                />
              </div>
            </div>
            <div className="flex flex-col gap-1">
              <span className="text-[12.5px] font-bold">{t('ui.vault.tabs.golpes')}</span>
              <div className="grid grid-cols-1 gap-1.5 sm:grid-cols-2">
                {draft.moves.map((m, i) => (
                  <SearchSelect
                    key={i}
                    options={data?.moves.filter((o) => o.value > 0) ?? []}
                    value={m || null}
                    placeholder={t('ui.vault.moveN', { n: i + 1 })}
                    emptyLabel={dash}
                    onChange={(v) => {
                      const moves = [...draft.moves]
                      moves[i] = v ?? 0
                      setDraft({ ...draft, moves })
                    }}
                  />
                ))}
              </div>
            </div>
            <div className="flex flex-col gap-1.5">
              <div className="flex items-center">
                <span className="flex-1 text-[12.5px] font-bold">EVs</span>
                <span className={`text-[12.5px] font-semibold ${evTotal > 510 ? 'text-bad' : 'text-muted'}`}>{evTotal}/510</span>
              </div>
              {draft.evs.map((v, i) => (
                <label key={i} className="flex items-center gap-2 text-[13px]">
                  <span className="w-10 text-muted">{statLabel(i)}</span>
                  <input
                    type="range"
                    min={0}
                    max={255}
                    value={v}
                    onChange={(e) => {
                      const evs = [...draft.evs]
                      evs[i] = Number(e.target.value)
                      setDraft({ ...draft, evs })
                    }}
                    className="min-w-0 flex-1 accent-[var(--tr-accent)]"
                  />
                  <span className="w-8 text-right font-mono text-xs">{v}</span>
                </label>
              ))}
            </div>
            {draft.nature !== spec?.nature && <Callout tone="sky" icon="info" className="text-[13px]">{t('ui.vault.natureRegenerates')}</Callout>}
            {editError && <Callout tone="red" icon="error" className="text-[13px]">{editError}</Callout>}
          </div>
          <div className="flex gap-2 border-t border-line px-4 py-3.5">
            <Button variant="secondary" className="h-[42px] flex-1" disabled={saving} onClick={() => setDraft(null)}>
              {t('common.cancel')}
            </Button>
            <Button className="h-[42px] flex-[1.4]" disabled={!draftValid || saving} onClick={() => void saveEdit()}>
              {saving ? t('builder.generating') : t('ui.vault.saveChanges')}
            </Button>
          </div>
        </>
      )}
    </div>
  )
}

/** Abas do painel para os formatos que o construtor não abre (Gen 1, 2 e 4 a 7): só leitura. */
function LegacyTab({
  tab,
  view,
  pokemon,
  row,
  gender,
  sixDigits,
  statLabel,
  report,
  onToggleReport,
}: {
  tab: TabKey
  view: LegacyView
  pokemon: VaultPokemon
  row: (k: string, v: React.ReactNode) => React.ReactNode
  gender: string
  sixDigits: boolean
  statLabel: (i: number) => string
  report: LegalityReport | null
  onToggleReport: () => void
}) {
  const { t, i18n } = useTranslation()
  const dash = '—'
  if (tab === 'resumo')
    return (
      <>
        {view.nature && row(t('builder.fields.nature'), view.nature)}
        {view.ability && row(t('builder.fields.ability'), view.ability)}
        {row(t('builder.fields.heldItem'), view.heldItem ?? t('ui.vault.none'))}
        {row(t('builder.fields.gender'), gender)}
        {row(t('ui.vault.ot'), `${pokemon.otName} (${String(pokemon.tid).padStart(sixDigits ? 6 : 5, '0')})`)}
        {row(t('builder.fields.language'), view.language)}
        {row(t('vault.game'), formatName(familyOf(pokemon)))}
      </>
    )
  if (tab === 'golpes')
    return view.moves.length === 0 ? (
      <p className="text-sm text-muted">{dash}</p>
    ) : (
      <>
        {view.moves.map((m, i) => (
          <div key={`${m}-${i}`} className="flex h-[38px] items-center rounded-lg bg-surface2 px-3 text-sm font-semibold">
            {m}
          </div>
        ))}
      </>
    )
  if (tab === 'status')
    return (
      <>
        <div className="flex items-center gap-2 text-[11.5px] text-muted">
          <span className="w-11" />
          <span className="flex-1">{t('ui.vault.ivEv')}</span>
          <HelpTip>{t('ui.vault.ivEvTip')}</HelpTip>
        </div>
        {STAT_KEYS.map((_, i) => (
          <div key={i} className="flex items-center gap-2 text-[13px]">
            <span className="w-11 text-muted">{statLabel(i)}</span>
            <div className="flex flex-1 flex-col gap-[3px]">
              <div className="h-1.5 rounded-sm bg-surface2">
                <div className="h-full rounded-sm bg-accent" style={{ width: `${Math.min(100, (view.ivs[i] / (pokemon.generation <= 2 ? 15 : 31)) * 100)}%` }} />
              </div>
              <div className="h-1.5 rounded-sm bg-surface2">
                <div className="h-full rounded-sm bg-info" style={{ width: `${Math.min(100, (view.evs[i] / view.maxEv) * 100)}%` }} />
              </div>
            </div>
            <span className="w-14 text-right font-mono text-xs">
              {view.ivs[i]}·{view.evs[i]}
            </span>
          </div>
        ))}
      </>
    )
  return (
    <>
      {row(t('ui.vault.originLabel'), t(`pokemon.origin.${pokemon.origin}`))}
      {row(t('builder.fields.version'), pokemon.originGame)}
      {view.metLevel !== null && row(t('builder.fields.metLevel'), view.metLevel || t('ui.vault.hatched'))}
      {view.ball && row(t('builder.fields.ball'), view.ball)}
      {pokemon.sourceFileName && row(t('ui.vault.file'), pokemon.sourceFileName)}
      {row(t('ui.vault.added'), new Date(pokemon.addedAt).toLocaleString(i18n.language, { dateStyle: 'short', timeStyle: 'short' }))}
      <button type="button" className="mt-1 flex cursor-pointer items-center gap-1 self-start text-[13.5px] font-semibold text-accent" onClick={onToggleReport}>
        <Icon name={report ? 'expand_less' : 'expand_more'} className="text-[18px]" />
        {report ? t('legality.hide') : t('legality.view')}
      </button>
      {report && (
        <div className="rounded-lg bg-surface2 p-3">
          <LegalityPanel report={report} />
        </div>
      )}
    </>
  )
}
