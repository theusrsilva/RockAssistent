import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { useParams, useSearchParams } from 'react-router'
import { ApiError, spriteUrl } from '../../lib/api'
import { dexLang, loadDexGame, saveDexGame, useAvailability, useSpecies } from '../../lib/dex'
import { GAME_VERSIONS, gameVersionByKey } from '../../lib/gameVersions'
import { Badge, Callout, Icon } from '../../components/ui'
import { GameSelector } from './GameSelector'
import { TypeList } from './TypeBadge'
import { GeneralTab } from './tabs/GeneralTab'
import { LocationsTab } from './tabs/LocationsTab'
import { MovesTab } from './tabs/MovesTab'
import { StatsTab } from './tabs/StatsTab'
import { EggTab } from './tabs/EggTab'
import { SavesTab } from './tabs/SavesTab'
import { BackLink } from './tabs/parts'

const tabs = ['general', 'locations', 'moves', 'stats', 'egg', 'saves'] as const
type Tab = (typeof tabs)[number]

/** Página de uma espécie: cabeçalho, jogo escolhido e as abas com os dados daquele jogo. */
export default function SpeciesPage() {
  const { t, i18n } = useTranslation()
  const id = Number(useParams().id)
  const [params, setParams] = useSearchParams()
  const tab: Tab = (tabs as readonly string[]).includes(params.get('aba') ?? '') ? (params.get('aba') as Tab) : 'general'
  const [game, setGame] = useState(() => {
    const saved = loadDexGame()
    return GAME_VERSIONS.some((g) => g.key === saved) ? saved : 'frlg'
  })
  const lang = dexLang(i18n.language)
  const valid = Number.isInteger(id) && id > 0
  const species = useSpecies(valid ? id : 0, game, lang)
  const availability = useAvailability(valid ? id : 0)
  // Forma escolhida (id do pokemon no PokeAPI), a mesma no cabeçalho e na aba Status. Um id de outra espécie
  // (depois de navegar) não está em detail.forms e cai na forma padrão.
  const [formId, setFormId] = useState<number | null>(null)
  const [shiny, setShiny] = useState(false)

  const chooseGame = (key: string) => {
    setGame(key)
    saveDexGame(key)
  }
  const chooseTab = (next: Tab) =>
    setParams(
      (p) => {
        const n = new URLSearchParams(p)
        if (next === 'general') n.delete('aba')
        else n.set('aba', next)
        return n
      },
      { replace: true },
    )

  const back = <BackLink>{t('dex.back')}</BackLink>

  if (!valid || (species.error instanceof ApiError && species.error.status === 404))
    return (
      <div className="flex flex-col gap-4">
        {back}
        <Callout tone="amber" icon="help">
          {t('dex.notFound')}
        </Callout>
      </div>
    )

  const detail = species.data
  const gameAvailability = availability.data?.find((a) => a.gameKey === game)
  const kind = gameAvailability?.kind
  const defaultForm = detail?.forms.find((f) => f.isDefault) ?? detail?.forms[0]
  const form = detail?.forms.find((f) => f.id === formId) ?? defaultForm
  const types = form?.types ?? detail?.types

  return (
    <div className="flex flex-col gap-4">
      {back}

      <div className="flex flex-col gap-3 sm:flex-row sm:items-center">
        <div className="flex shrink-0 flex-col items-center gap-1.5 self-center sm:w-44 sm:self-start">
          <img
            src={spriteUrl(id, { shiny, form: form && !form.isDefault ? form.id : undefined })}
            alt=""
            className="h-24 w-24 [image-rendering:pixelated]"
          />
          <button
            type="button"
            aria-pressed={shiny}
            title={t('dex.sprite.shinyToggle')}
            onClick={() => setShiny((s) => !s)}
            className={`inline-flex cursor-pointer items-center gap-1 rounded-full px-2.5 py-0.5 text-xs font-semibold ${
              shiny ? 'bg-accent text-white' : 'bg-surface2 text-muted hover:text-ink'
            }`}
          >
            <Icon name="auto_awesome" className="text-[14px]" filled={shiny} />
            {t('dex.sprite.shiny')}
          </button>
        </div>
        <div className="min-w-0 flex-1 space-y-1 text-center sm:text-left">
          <div className="flex flex-wrap items-baseline justify-center gap-x-2 sm:justify-start">
            <h1 className="font-display text-[23px] leading-tight font-bold md:text-[28px]">{detail?.name ?? '…'}</h1>
            <span className="font-mono text-sm text-muted">#{String(id).padStart(4, '0')}</span>
          </div>
          {detail?.genus && <p className="text-sm text-muted">{detail.genus}</p>}
          <div className="flex flex-wrap items-center justify-center gap-1.5 sm:justify-start">
            {types && <TypeList types={types} />}
            {kind && <Badge tone={kind === 'none' ? 'slate' : 'emerald'}>{t(`dex.availability.${kind}`)}</Badge>}
            {gameAvailability?.requires && <Badge tone="amber">{t(`dex.requires.${gameAvailability.requires}`)}</Badge>}
            {gameAvailability?.shinyLocked && <Badge tone="amber">{t('dex.shinyLocked')}</Badge>}
          </div>
        </div>
        <GameSelector value={game} onChange={chooseGame} availability={availability.data} />
      </div>

      {detail && detail.forms.length > 1 && (
        <div className="-mt-1 flex gap-1 scroll-x pb-1" role="tablist" aria-label={t('dex.sprite.forms')}>
          {detail.forms.map((f) => (
            <button
              key={f.id}
              type="button"
              role="tab"
              aria-selected={form?.id === f.id}
              onClick={() => setFormId(f.id)}
              className={`flex shrink-0 cursor-pointer items-center gap-1 rounded-lg border px-1.5 py-1 text-xs font-semibold whitespace-nowrap ${
                form?.id === f.id ? 'border-accent bg-surface text-ink' : 'border-line bg-surface2 text-muted hover:text-ink'
              }`}
            >
              <img
                src={spriteUrl(id, { shiny, form: f.isDefault ? undefined : f.id })}
                alt=""
                loading="lazy"
                className="h-8 w-8 [image-rendering:pixelated]"
              />
              {f.isDefault ? t('dex.sprite.normal') : f.name}
            </button>
          ))}
        </div>
      )}

      <div className="-mx-1 flex gap-1 scroll-x border-b border-line px-1" role="tablist">
        {tabs.map((tb) => (
          <button
            key={tb}
            type="button"
            role="tab"
            aria-selected={tab === tb}
            onClick={() => chooseTab(tb)}
            className={`-mb-px cursor-pointer border-b-2 px-3 py-2 text-sm font-semibold whitespace-nowrap ${
              tab === tb ? 'border-accent text-ink' : 'border-transparent text-muted hover:text-ink'
            }`}
          >
            {t(`dex.tabs.${tb}`)}
          </button>
        ))}
      </div>

      {species.error && (
        <Callout tone="red" icon="error">
          {t('dex.error')}
        </Callout>
      )}
      {species.loading && <p className="text-sm text-muted">{t('common.loading')}</p>}

      {detail && !detail.existsInGame && (
        <Callout tone="amber" icon="info">
          {t('dex.notInGame', { game: gameVersionByKey(game).name })}
        </Callout>
      )}

      {detail && tab === 'general' && <GeneralTab detail={detail} game={game} />}
      {detail && tab === 'locations' && <LocationsTab detail={detail} />}
      {detail && tab === 'moves' && <MovesTab detail={detail} />}
      {detail && tab === 'stats' && <StatsTab detail={detail} form={form} onFormChange={setFormId} />}
      {detail && tab === 'egg' && <EggTab detail={detail} />}
      {tab === 'saves' && <SavesTab species={id} />}
    </div>
  )
}
