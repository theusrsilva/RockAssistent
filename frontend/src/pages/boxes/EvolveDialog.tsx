import { useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Button, Callout, Modal } from '../../components/ui'
import { api, spriteUrl } from '../../lib/api'
import { errorMessage, type EvolveLegality, type EvolveOption } from './boxes'

/**
 * "Evoluir por troca" (cofre e save): lista as evoluções por troca do Pokémon e aplica a escolhida.
 * Se o PKHeX achar algum problema depois de evoluir, o diálogo fica aberto com o aviso (não bloqueia).
 */
export function EvolveDialog({
  name,
  optionsUrl,
  submit,
  onError,
  onClose,
}: {
  name: string
  optionsUrl: string
  /** Faz o POST …/evolve e devolve o veredito de legalidade. */
  submit: (option: EvolveOption) => Promise<EvolveLegality>
  /** Erro do POST (ex.: 409 save_changed: quem abriu recarrega o que mostra). */
  onError?: (e: unknown) => void
  onClose: (evolved: boolean) => void
}) {
  const { t } = useTranslation()
  const [options, setOptions] = useState<EvolveOption[] | null>(null)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [warning, setWarning] = useState<{ to: string; legality: EvolveLegality } | null>(null)

  useEffect(() => {
    api
      .get<EvolveOption[]>(optionsUrl)
      .then(setOptions)
      .catch((e) => {
        setOptions([])
        setError(errorMessage(t, e))
      })
  }, [optionsUrl, t])

  async function choose(option: EvolveOption) {
    if (busy) return
    setBusy(true)
    setError(null)
    try {
      const legality = await submit(option)
      if (legality.valid) onClose(true)
      else setWarning({ to: option.speciesName, legality })
    } catch (e) {
      setError(errorMessage(t, e))
      onError?.(e)
    } finally {
      setBusy(false)
    }
  }

  if (warning) {
    return (
      <Modal title={t('boxes.evolveDialog.title', { name })} actions={<Button onClick={() => onClose(true)}>{t('common.close')}</Button>}>
        <div className="flex flex-col gap-3">
          <p>{t('boxes.evolveDialog.done', { species: warning.to })}</p>
          <Callout tone="amber" icon="warning">
            <p className="font-semibold">{t('boxes.evolveDialog.illegal')}</p>
            <ul className="mt-1 list-disc pl-5 text-[13px]">
              {warning.legality.messages.map((m) => (
                <li key={m}>{m}</li>
              ))}
            </ul>
          </Callout>
        </div>
      </Modal>
    )
  }

  return (
    <Modal
      title={t('boxes.evolveDialog.title', { name })}
      actions={
        <Button variant="secondary" disabled={busy} onClick={() => onClose(false)}>
          {t('common.cancel')}
        </Button>
      }
    >
      <div className="flex flex-col gap-3">
        <p className="text-muted">{t('boxes.evolveDialog.help')}</p>
        {options === null && <p className="text-muted">{t('common.loading')}</p>}
        {options?.length === 0 && !error && <Callout tone="slate" icon="block">{t('boxes.evolveDialog.none')}</Callout>}
        {options?.map((o) => (
          <button
            key={`${o.species}-${o.form}`}
            type="button"
            disabled={busy}
            onClick={() => void choose(o)}
            className="flex cursor-pointer items-center gap-3 rounded-lg border border-line bg-surface p-2.5 text-left hover:bg-surface2 disabled:cursor-wait disabled:opacity-60"
          >
            <img src={spriteUrl(o.species)} alt="" className="pixelated h-14 w-14 shrink-0" />
            <span className="flex min-w-0 flex-col">
              <span className="font-display text-base font-bold">{o.speciesName}</span>
              {o.requiredItemName && (
                <span className="text-[13px] text-muted">
                  {o.holdsItem
                    ? t('boxes.evolveDialog.usesItem', { item: o.requiredItemName })
                    : t('boxes.evolveDialog.needsItem', { item: o.requiredItemName })}
                </span>
              )}
              {o.tradeWithName && <span className="text-[13px] text-muted">{t('boxes.evolveDialog.tradeWith', { species: o.tradeWithName })}</span>}
            </span>
          </button>
        ))}
        {error && (
          <Callout tone="red" icon="error">
            {error}
          </Callout>
        )}
      </div>
    </Modal>
  )
}
