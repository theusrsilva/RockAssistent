import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Button, Callout, Field, Modal, Segmented, Toggle, inputClass, selectClass } from '../../components/ui'
import { api } from '../../lib/api'
import { versionName } from '../../lib/dex'
import { gameVersionByKey, gameVersionsByGeneration } from '../../lib/gameVersions'
import { DEFAULT_RULES, RULE_KEYS, nuzlockeErrorMessage, type NuzlockeRules, type RunView } from './nuzlocke'
import { StarterPicker, useStarters } from './StarterPicker'

/** Nova run: nome, jogo (os 22 grupos), versão quando o grupo tem duas, o inicial (opcional) e as regras. */
export default function NewRunDialog({ onClose, onCreated }: { onClose: () => void; onCreated: (id: string) => void }) {
  const { t } = useTranslation()
  const [name, setName] = useState('')
  const [gameKey, setGameKey] = useState('frlg')
  const [version, setVersion] = useState<string | null>(null)
  const [starter, setStarter] = useState<string | null>(null)
  const [rules, setRules] = useState<NuzlockeRules>(DEFAULT_RULES)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const game = gameVersionByKey(gameKey)
  const needsVersion = game.pokeApiVersions.length > 1
  const starters = useStarters(gameKey)

  const chooseGame = (key: string) => {
    setGameKey(key)
    setVersion(null)
    setStarter(null)
    setError(null)
  }

  async function create() {
    setBusy(true)
    setError(null)
    try {
      const view = await api.post<RunView>('/api/nuzlocke', {
        name: name.trim() || null,
        gameKey,
        version: needsVersion ? version : null,
        starter,
        rules: { ...rules, custom: rules.custom?.trim() || null },
      })
      onCreated(view.run.id)
    } catch (e) {
      setError(nuzlockeErrorMessage(t, e))
      setBusy(false)
    }
  }

  return (
    <Modal
      wide
      title={t('nuzlocke.newRun.title')}
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            {t('common.cancel')}
          </Button>
          <Button icon="play_arrow" disabled={busy || (needsVersion && !version)} onClick={() => void create()}>
            {t('nuzlocke.newRun.start')}
          </Button>
        </>
      }
    >
      <div className="flex flex-col gap-3.5">
        <Field label={t('nuzlocke.newRun.name')}>
          <input className={inputClass} value={name} maxLength={80} placeholder={t('nuzlocke.newRun.namePlaceholder')} onChange={(e) => setName(e.target.value)} />
        </Field>
        <Field label={t('nuzlocke.newRun.game')}>
          <select className={selectClass} value={gameKey} onChange={(e) => chooseGame(e.target.value)}>
            {gameVersionsByGeneration().map((g) => (
              <optgroup key={g.generation} label={t('nuzlocke.newRun.generation', { generation: g.generation })}>
                {g.games.map((v) => (
                  <option key={v.key} value={v.key}>
                    {v.name}
                  </option>
                ))}
              </optgroup>
            ))}
          </select>
        </Field>
        {needsVersion && (
          <div className="flex flex-col gap-1.5 text-sm">
            <span className="font-bold">{t('nuzlocke.newRun.version')}</span>
            <Segmented
              value={version ?? ''}
              onChange={setVersion}
              options={game.pokeApiVersions.map((v) => ({ value: v, label: versionName(v) }))}
              className="w-full sm:w-fit"
            />
            <span className="text-[12.5px] text-muted">{t('nuzlocke.newRun.versionHelp')}</span>
          </div>
        )}
        {starters.length > 1 && (
          <div className="flex flex-col gap-1.5 text-sm">
            <span className="font-bold">{t('nuzlocke.starter.label')}</span>
            <StarterPicker starters={starters} value={starter} onChange={setStarter} allowNone />
            <span className="text-[12.5px] text-muted">{t('nuzlocke.starter.help')}</span>
          </div>
        )}
        <div className="flex flex-col gap-2.5">
          <span className="text-sm font-bold">{t('nuzlocke.rules.title')}</span>
          {RULE_KEYS.map((k) => (
            <div key={k} className="flex flex-col gap-0.5">
              <Toggle small on={rules[k]} onChange={(on) => setRules((r) => ({ ...r, [k]: on }))} label={t(`nuzlocke.rules.${k}.name`)} />
              <span className="pl-[46px] text-[12.5px] text-muted">{t(`nuzlocke.rules.${k}.help`)}</span>
            </div>
          ))}
        </div>
        <Field label={t('nuzlocke.rules.custom')} help={t('nuzlocke.rules.customHelp')}>
          <textarea
            className={`${inputClass} h-20! py-2`}
            maxLength={2000}
            value={rules.custom ?? ''}
            placeholder={t('nuzlocke.rules.customPlaceholder')}
            onChange={(e) => setRules((r) => ({ ...r, custom: e.target.value }))}
          />
        </Field>
        {error && (
          <Callout tone="red" icon="error">
            {error}
          </Callout>
        )}
      </div>
    </Modal>
  )
}
