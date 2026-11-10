import { useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Button, Callout, Icon, Modal } from '../../components/ui'
import { ApiError, api } from '../../lib/api'
import { gameByFamily } from '../../lib/games'
import type { GameFamily, VaultPokemon } from '../../lib/types'
import type { EvolveLegality } from '../boxes/boxes'

interface Destination {
  game: GameFamily
  refusal: string | null
}

/** Resposta de POST …/copy-for-game (D36): a cópia, o que não deu para copiar igual e onde ela "nasceu". */
export interface GameCopyResult {
  pokemon: VaultPokemon
  notes: string[]
  met: { location: string | null; level: number; type: string | null; version: string; ball: string | null }
  legality: EvolveLegality
}

/**
 * Mover um Pokémon para outro jogo, como o Pokémon HOME faz (Entrega 14d). O registro do cofre continua o mesmo; o
 * "Meu treinador" do jogo de destino passa a ser o treinador atual.
 *
 * Quando o jogo de destino não aceita a origem ("origin_not_accepted": nasceu num jogo mais novo) ou não há conversão
 * ("no_route": por exemplo do formato do Z-A para o Scarlet/Violet), ou quando a origem se perdeu no Sword/Shield
 * ("origin_lost_in_swsh"), a linha oferece
 * "Copiar para ser válido no jogo" (D36): depois de uma confirmação, nasce um Pokémon NOVO no jogo de destino, com o
 * "Meu treinador" de lá. O original fica; a cópia entra na Caixa de entrada.
 */
/** Recusas de mover em que a cópia nascida no jogo de destino é oferecida (D36). */
const COPY_REFUSALS = ['origin_not_accepted', 'no_route', 'origin_lost_in_swsh']

export function TransferDialog({
  pokemon,
  onDone,
  onCopied,
  onClose,
}: {
  pokemon: VaultPokemon
  onDone: (moved: VaultPokemon) => void
  onCopied: (r: GameCopyResult) => void
  onClose: () => void
}) {
  const { t, i18n } = useTranslation()
  const [destinations, setDestinations] = useState<Destination[] | null>(null)
  const [choice, setChoice] = useState<GameFamily | null>(null)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState<string | null>(null)
  /** Jogo da cópia em confirmação (null = a lista de destinos). */
  const [copyTo, setCopyTo] = useState<GameFamily | null>(null)
  const [copyError, setCopyError] = useState<{ code: string; reason: string | null } | null>(null)
  const [copied, setCopied] = useState<GameCopyResult | null>(null)

  useEffect(() => {
    api
      .get<Destination[]>(`/api/vault/pokemon/${pokemon.id}/destinations`)
      .then(setDestinations)
      .catch(() => setDestinations([]))
  }, [pokemon.id])

  async function move() {
    if (!choice) return
    setBusy(true)
    setError(null)
    try {
      const r = await api.post<{ pokemon: VaultPokemon; notes: string[] }>(`/api/vault/pokemon/${pokemon.id}/transfer`, { game: choice })
      onDone(r.pokemon)
    } catch (e) {
      setError(e instanceof ApiError ? (e.code ?? 'no_route') : 'no_route')
    } finally {
      setBusy(false)
    }
  }

  async function copy() {
    if (!copyTo) return
    setBusy(true)
    setCopyError(null)
    try {
      const r = await api.post<GameCopyResult>(`/api/vault/pokemon/${pokemon.id}/copy-for-game`, { game: copyTo, language: i18n.language })
      setCopied(r)
      onCopied(r)
    } catch (e) {
      setCopyError(e instanceof ApiError ? { code: e.code ?? 'copy_not_possible', reason: e.reason } : { code: 'copy_not_possible', reason: null })
    } finally {
      setBusy(false)
    }
  }

  if (copyTo) {
    const game = gameByFamily(copyTo).name
    const name = pokemon.nickname || pokemon.speciesName
    const why = destinations?.find((d) => d.game === copyTo)?.refusal
    const warnKey = why === 'no_route' ? 'vault.copy.warnNoRoute' : why === 'origin_lost_in_swsh' ? 'vault.copy.warnLostOrigin' : 'vault.copy.warn'
    return (
      <Modal
        title={t('vault.copy.title', { game })}
        actions={
          copied ? (
            <Button onClick={onClose}>{t('common.close')}</Button>
          ) : (
            <>
              <Button
                variant="ghost"
                disabled={busy}
                onClick={() => {
                  setCopyTo(null)
                  setCopyError(null)
                }}
              >
                {t('common.back')}
              </Button>
              <Button icon="content_copy" disabled={busy} onClick={() => void copy()}>
                {busy ? t('vault.copy.working') : t('vault.copy.confirm')}
              </Button>
            </>
          )
        }
      >
        <div className="flex flex-col gap-3">
          {copied ? (
            <>
              <Callout tone="emerald" icon="check_circle" className="text-[13px]">
                {t('vault.copy.done', { game })}
              </Callout>
              <p>
                {t(copied.met.type === 'egg' ? 'vault.copy.metEgg' : 'vault.copy.met', {
                  location: copied.met.location ?? '?',
                  level: copied.met.level,
                  ball: copied.met.ball ?? '?',
                })}
              </p>
              {copied.notes.length > 0 && (
                <Callout tone="sky" icon="info" className="text-[13px]">
                  <b>{t('vault.copy.notesTitle')}</b>
                  <ul className="mt-1 list-disc pl-5">
                    {copied.notes.map((n) => (
                      <li key={n}>{t(`vault.copy.notes.${n}`, { defaultValue: n })}</li>
                    ))}
                  </ul>
                </Callout>
              )}
            </>
          ) : (
            <>
              <Callout tone="amber" icon="warning" className="text-[13px]">
                <b>{t('vault.copy.warnTitle')}</b>
                <span className="mt-1 block">{t(warnKey, { name, game })}</span>
              </Callout>
              <ul className="list-disc pl-5 text-[13.5px]">
                <li>{t('vault.copy.keepsOriginal', { name })}</li>
                <li>{t('vault.copy.bornThere', { game })}</li>
                <li>{t('vault.copy.trainer', { game })}</li>
                <li>{t('vault.copy.sameLook')}</li>
              </ul>
            </>
          )}
          {copyError && (
            <Callout tone="red" icon="error">
              {t(`vault.copy.refusal.${copyError.code}`, { defaultValue: t('vault.copy.refusal.copy_not_possible') })}
              {copyError.code === 'copy_not_possible' && copyError.reason && (
                <span className="mt-1 block text-[13px]">{t(`vault.copy.reason.${copyError.reason}`, { defaultValue: '' })}</span>
              )}
            </Callout>
          )}
        </div>
      </Modal>
    )
  }

  const others = destinations?.filter((d) => d.refusal !== 'same_game') ?? []
  return (
    <Modal
      title={t('vault.transfer.title')}
      actions={
        <>
          <Button variant="ghost" onClick={onClose}>
            {t('common.cancel')}
          </Button>
          <Button icon="move_up" disabled={!choice || busy} onClick={() => void move()}>
            {t('vault.transfer.confirm')}
          </Button>
        </>
      }
    >
      <div className="flex flex-col gap-3">
        <p className="text-muted">{t('vault.transfer.help')}</p>
        {destinations === null && <p className="text-muted">{t('common.loading')}</p>}
        <div className="flex flex-col gap-1.5" role="radiogroup">
          {others.map((d) =>
            d.refusal !== null && COPY_REFUSALS.includes(d.refusal) ? (
              // Não dá para mover, mas dá para criar uma cópia nascida lá (com confirmação). Em todos esses casos a
              // espécie existe no jogo de destino.
              <div key={d.game} className="flex flex-col gap-2 rounded-lg border border-line px-3 py-2">
                <div className="flex items-center gap-2.5">
                  <Icon name="block" className="text-[20px] text-muted" />
                  <span className="flex-1">
                    <b>{gameByFamily(d.game).name}</b>
                    <span className="block text-[12.5px] text-muted">
                      {t(`vault.transfer.refusal.${d.refusal}`)}
                    </span>
                  </span>
                </div>
                <Button variant="secondary" icon="content_copy" className="self-start" disabled={busy} onClick={() => setCopyTo(d.game)}>
                  {t('vault.copy.action', { game: gameByFamily(d.game).name })}
                </Button>
              </div>
            ) : (
              <button
                key={d.game}
                type="button"
                role="radio"
                aria-checked={choice === d.game}
                disabled={d.refusal !== null}
                onClick={() => setChoice(d.game)}
                className={`flex cursor-pointer items-center gap-2.5 rounded-lg border px-3 py-2 text-left disabled:cursor-not-allowed disabled:opacity-60 ${choice === d.game ? 'border-accent bg-accent-soft' : 'border-line'}`}
              >
                <Icon name={d.refusal ? 'block' : choice === d.game ? 'radio_button_checked' : 'radio_button_unchecked'} className="text-[20px] text-muted" />
                <span className="flex-1">
                  <b>{gameByFamily(d.game).name}</b>
                  {d.refusal && <span className="block text-[12.5px] text-muted">{t(`vault.transfer.refusal.${d.refusal}`, { defaultValue: d.refusal })}</span>}
                </span>
              </button>
            ),
          )}
        </div>
        <Callout tone="amber" icon="info" className="text-[13px]">
          {t('vault.transfer.tracker')}
        </Callout>
        {error && (
          <Callout tone="red" icon="error">
            {t(`vault.transfer.refusal.${error}`, { defaultValue: t('common.error') })}
          </Callout>
        )}
      </div>
    </Modal>
  )
}
