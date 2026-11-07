import { useMemo, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { useNavigate } from 'react-router'
import { SearchSelect } from '../../components/SearchSelect'
import { Badge, Button, Callout, Card, Field, PageTitle, Segmented, Toggle, selectClass } from '../../components/ui'
import { api, spriteUrl } from '../../lib/api'
import { useApiGet } from '../../lib/dex'
import { formatOdds, huntErrorMessage, shinyName, useSpeciesOptions, type Hunt, type HuntGame, type HuntMethod, type ShinyModifier } from '../../lib/hunts'
import { BackLink } from '../dex/tabs/parts'

/** Valor inicial de cada modificador que é campo (os guiados pelo contador ficam de fora). */
function defaults(method: HuntMethod): Record<string, string> {
  const values: Record<string, string> = {}
  for (const m of method.modifiers) {
    if (m.counter) continue
    if (m.kind === 'toggle') values[m.id] = 'false'
    else if (m.steps?.length) values[m.id] = String(m.steps[0].min)
  }
  return values
}

/** Nova hunt: Pokémon → jogo em que ele é obtível → método → modificadores. */
export default function NewHuntWizard() {
  const { t, i18n } = useTranslation()
  const navigate = useNavigate()
  const species = useSpeciesOptions()
  const [chosen, setChosen] = useState<number | null>(null)
  const [gameKey, setGameKey] = useState<string | null>(null)
  const [methodId, setMethodId] = useState<string | null>(null)
  const [modifiers, setModifiers] = useState<Record<string, string>>({})
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const games = useApiGet<HuntGame[]>(chosen ? `/api/hunts/species/${chosen}/games` : null)
  const game = games.data?.find((g) => g.gameKey === gameKey) ?? null
  const method = game?.methods.find((m) => m.id === methodId) ?? null
  const options = useMemo(() => (species ?? []).map((s) => ({ value: s.value, text: `#${s.value} ${s.text}` })), [species])

  const chooseSpecies = (id: number | null) => {
    setChosen(id)
    setGameKey(null)
    setMethodId(null)
    setError(null)
  }
  const chooseGame = (key: string) => {
    setGameKey(key)
    setMethodId(null)
    setError(null)
  }
  const chooseMethod = (m: HuntMethod) => {
    setMethodId(m.id)
    setModifiers(defaults(m))
    setError(null)
  }

  async function start() {
    if (!chosen || !game || !method) return
    setBusy(true)
    setError(null)
    try {
      const hunt = await api.post<Hunt>('/api/hunts', { species: chosen, form: 0, gameKey: game.gameKey, methodId: method.id, modifiers })
      navigate(`/hunt/${hunt.id}`, { replace: true })
    } catch (e) {
      setError(huntErrorMessage(t, e))
      setBusy(false)
    }
  }

  return (
    <div className="flex max-w-[760px] flex-col gap-4">
      <BackLink to="/hunt">{t('hunt.back')}</BackLink>
      <PageTitle>{t('hunt.wizard.title')}</PageTitle>

      <Card className="flex flex-col gap-3">
        <Field label={t('hunt.wizard.pokemon')}>
          <SearchSelect
            options={options}
            value={chosen}
            onChange={chooseSpecies}
            placeholder={t('hunt.wizard.searchSpecies')}
            renderIcon={(v) => <img src={spriteUrl(v)} alt="" className="pixelated h-8 w-8" loading="lazy" />}
          />
        </Field>
      </Card>

      {chosen && (
        <Card className="flex flex-col gap-3">
          <div className="font-bold">{t('hunt.wizard.game')}</div>
          {games.loading ? (
            <p className="text-sm text-muted">{t('hunt.wizard.loadingGames')}</p>
          ) : games.error ? (
            <Callout tone="red" icon="error">
              {huntErrorMessage(t, games.error)}
            </Callout>
          ) : !games.data?.length ? (
            <Callout tone="amber" icon="info">
              {t('hunt.wizard.noGames')}
            </Callout>
          ) : (
            <div className="grid grid-cols-1 gap-2 sm:grid-cols-2">
              {games.data.map((g) => (
                <Choice key={g.gameKey} selected={g.gameKey === gameKey} onClick={() => chooseGame(g.gameKey)}>
                  <div className="font-semibold">{g.name}</div>
                  <div className="mt-1 flex flex-wrap gap-1">
                    {g.kinds.map((k) => (
                      <Badge key={k}>{t(`hunt.wizard.kinds.${k}`)}</Badge>
                    ))}
                  </div>
                </Choice>
              ))}
            </div>
          )}
        </Card>
      )}

      {game && (
        <Card className="flex flex-col gap-3">
          <div className="font-bold">{t('hunt.wizard.method')}</div>
          <div className="grid grid-cols-1 gap-2 sm:grid-cols-2">
            {game.methods.map((m) => (
              <Choice key={m.id} selected={m.id === methodId} onClick={() => chooseMethod(m)}>
                <div className="font-semibold">{shinyName(m, i18n.language)}</div>
                <div className="text-[12.5px] text-muted">{t('hunt.wizard.baseOdds', { odds: formatOdds(m.baseOdds, i18n.language) })}</div>
              </Choice>
            ))}
          </div>
        </Card>
      )}

      {method && (
        <Card className="flex flex-col gap-3">
          <div className="font-bold">{t('hunt.wizard.modifiers')}</div>
          {method.modifiers.length === 0 ? (
            <p className="text-sm text-muted">{t('hunt.wizard.noModifiers')}</p>
          ) : (
            <div className="flex flex-col gap-3">
              {method.modifiers.map((m) => (
                <ModifierField
                  key={m.id}
                  modifier={m}
                  value={modifiers[m.id]}
                  onChange={(v) => setModifiers((cur) => ({ ...cur, [m.id]: v }))}
                />
              ))}
            </div>
          )}
        </Card>
      )}

      {error && (
        <Callout tone="red" icon="error">
          {error}
        </Callout>
      )}

      {method && (
        <div className="flex justify-end">
          <Button size="lg" icon="play_arrow" className="w-full sm:w-auto" disabled={busy} onClick={() => void start()}>
            {t('hunt.wizard.start')}
          </Button>
        </div>
      )}
    </div>
  )
}

function Choice({ selected, onClick, children }: { selected: boolean; onClick: () => void; children: React.ReactNode }) {
  return (
    <button
      type="button"
      aria-pressed={selected}
      onClick={onClick}
      className={`cursor-pointer rounded-lg border px-3.5 py-2.5 text-left transition ${selected ? 'border-accent bg-accent-soft' : 'border-line bg-surface hover:bg-surface2'}`}
    >
      {children}
    </button>
  )
}

/** Toggle vira interruptor; degraus viram seletor (o valor enviado é o "min" do degrau, em texto). */
function ModifierField({ modifier, value, onChange }: { modifier: ShinyModifier; value: string | undefined; onChange: (v: string) => void }) {
  const { t, i18n } = useTranslation()
  const name = shinyName(modifier, i18n.language)
  if (modifier.counter)
    return (
      <p className="flex items-center gap-2 text-sm text-muted">
        {t('hunt.wizard.counterStep', { name })}
      </p>
    )
  if (modifier.kind === 'toggle') return <Toggle on={value === 'true'} onChange={(on) => onChange(on ? 'true' : 'false')} label={name} />
  const steps = modifier.steps ?? []
  if (steps.length <= 4)
    return (
      <div className="flex flex-col gap-1.5 text-sm">
        <span className="font-semibold">{name}</span>
        <Segmented
          value={value ?? ''}
          onChange={onChange}
          options={steps.map((s) => ({ value: String(s.min), label: s.label }))}
          className="w-full sm:w-fit"
        />
      </div>
    )
  return (
    <Field label={name}>
      <select className={selectClass} value={value ?? ''} onChange={(e) => onChange(e.target.value)}>
        {steps.map((s) => (
          <option key={s.min} value={String(s.min)}>
            {s.label}
          </option>
        ))}
      </select>
    </Field>
  )
}
