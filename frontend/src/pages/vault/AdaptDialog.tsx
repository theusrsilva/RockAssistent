import { useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Button, Callout, Icon, Modal } from '../../components/ui'
import { ApiError, api } from '../../lib/api'
import { formatName } from '../../lib/games'
import type { VaultFormatKey, VaultPokemon } from '../../lib/types'
import type { EvolveLegality } from '../boxes/boxes'

/** Etapas oficiais de transferência (D34), em snake_case como na API. */
export type TransferStepKey = 'virtual_console' | 'pal_park' | 'poke_transfer' | 'poke_transporter' | 'pokemon_bank' | 'home'

export interface AdaptOption {
  target: VaultFormatKey
  generation: number
  tradable: boolean
  steps: TransferStepKey[]
  refusal: string | null
  refusedAt: TransferStepKey | null
}

export interface AdaptResult {
  pokemon: VaultPokemon
  notes: string[]
  legality: EvolveLegality
}

/**
 * "Adaptar para um jogo mais novo" (D34): os caminhos oficiais (Virtual Console, Pal Park, Poké Transfer, Poké
 * Transporter, Pokémon Bank e HOME), sempre para a frente. O original continua no cofre; a cópia adaptada entra na
 * Caixa de entrada. A legalidade só avisa.
 */
export function AdaptDialog({ pokemon, onDone, onClose }: { pokemon: VaultPokemon; onDone: (r: AdaptResult) => void; onClose: () => void }) {
  const { t, i18n } = useTranslation()
  const [options, setOptions] = useState<AdaptOption[] | null>(null)
  const [choice, setChoice] = useState<VaultFormatKey | null>(null)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [result, setResult] = useState<AdaptResult | null>(null)

  useEffect(() => {
    api
      .get<AdaptOption[]>(`/api/vault/pokemon/${pokemon.id}/adapt-options`)
      .then(setOptions)
      .catch(() => setOptions([]))
  }, [pokemon.id])

  async function adapt() {
    if (!choice) return
    setBusy(true)
    setError(null)
    try {
      const r = await api.post<AdaptResult>(`/api/vault/pokemon/${pokemon.id}/adapt`, { target: choice, language: i18n.language })
      setResult(r)
      onDone(r)
    } catch (e) {
      setError(e instanceof ApiError ? (e.code ?? 'no_route') : 'no_route')
    } finally {
      setBusy(false)
    }
  }

  const refusal = (code: string) => t(`vault.adapt.refusal.${code}`, { defaultValue: t('vault.adapt.refusal.no_route') })
  const path = (o: AdaptOption) => o.steps.map((s) => t(`vault.adapt.steps.${s}`)).join(' → ')

  return (
    <Modal
      title={t('vault.adapt.title')}
      actions={
        result ? (
          <Button onClick={onClose}>{t('common.close')}</Button>
        ) : (
          <>
            <Button variant="ghost" onClick={onClose}>
              {t('common.cancel')}
            </Button>
            <Button icon="upgrade" disabled={!choice || busy} onClick={() => void adapt()}>
              {t('vault.adapt.confirm')}
            </Button>
          </>
        )
      }
    >
      <div className="flex flex-col gap-3">
        <p className="text-muted">{t('vault.adapt.help')}</p>
        {options === null && <p className="text-muted">{t('common.loading')}</p>}
        {options?.length === 0 && <p className="text-muted">{t('vault.adapt.none')}</p>}
        {!result && (
          <div className="flex flex-col gap-1.5" role="radiogroup">
            {options?.map((o) => (
              <button
                key={o.target}
                type="button"
                role="radio"
                aria-checked={choice === o.target}
                disabled={o.refusal !== null}
                onClick={() => setChoice(o.target)}
                className={`flex cursor-pointer items-center gap-2.5 rounded-lg border px-3 py-2 text-left disabled:cursor-not-allowed disabled:opacity-60 ${choice === o.target ? 'border-accent bg-accent-soft' : 'border-line'}`}
              >
                <Icon name={o.refusal ? 'block' : choice === o.target ? 'radio_button_checked' : 'radio_button_unchecked'} className="text-[20px] text-muted" />
                <span className="min-w-0 flex-1">
                  <b>
                    Gen {o.generation} · {formatName(o.target)}
                  </b>
                  {o.refusal ? (
                    <span className="block text-[12.5px] text-muted">
                      {o.refusedAt && `${t(`vault.adapt.steps.${o.refusedAt}`)}: `}
                      {refusal(o.refusal)}
                    </span>
                  ) : (
                    <span className="block text-[12.5px] text-muted">{path(o)}</span>
                  )}
                </span>
              </button>
            ))}
          </div>
        )}
        {result ? (
          <>
            <Callout tone="emerald" icon="check_circle" className="text-[13px]">
              {t('vault.adapt.done', { game: formatName(result.pokemon.family) })}
            </Callout>
            {result.notes.map((n) => (
              <Callout key={n} tone="sky" icon="info" className="text-[13px]">
                {t(`vault.adapt.notes.${n}`, { defaultValue: n })}
              </Callout>
            ))}
            {result.legality.status === 'unverifiable' && (
              <Callout tone="amber" icon="info" className="text-[13px]">
                {t('vault.transfer.tracker')}
              </Callout>
            )}
            {!result.legality.valid && (
              <Callout tone="amber" icon="warning" className="text-[13px]">
                <b>{t('vault.adapt.illegal')}</b>
                {result.legality.messages.length > 0 && (
                  <ul className="mt-1 list-disc pl-5">
                    {result.legality.messages.map((m) => (
                      <li key={m}>{m}</li>
                    ))}
                  </ul>
                )}
              </Callout>
            )}
          </>
        ) : (
          <Callout tone="sky" icon="info" className="text-[13px]">
            {t('vault.adapt.keeps')}
          </Callout>
        )}
        {error && (
          <Callout tone="red" icon="error">
            {refusal(error)}
          </Callout>
        )}
      </div>
    </Modal>
  )
}
