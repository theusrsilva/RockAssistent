import { useTranslation } from 'react-i18next'
import { useParams } from 'react-router'
import { ApiError } from '../../lib/api'
import { dexLang, groupLearners, titleCase, useDexGame, useMove } from '../../lib/dex'
import { gameVersionByKey } from '../../lib/gameVersions'
import { Badge, Callout } from '../../components/ui'
import { GameSelector } from './GameSelector'
import { TypeBadge } from './TypeBadge'
import { BackLink, EffectText, Fact, Section, SpeciesChip } from './tabs/parts'

const methodLabel: Record<string, string> = { 'level-up': 'levelUp', machine: 'machine', tutor: 'tutor', egg: 'egg' }

/** Página de um golpe: dados no jogo escolhido, efeito e quem aprende. */
export default function MovePage() {
  const { t, i18n } = useTranslation()
  const id = Number(useParams().id)
  const valid = Number.isInteger(id) && id > 0
  const [game, chooseGame] = useDexGame()
  const move = useMove(valid ? id : 0, game)
  const english = dexLang(i18n.language) === 'pt-BR' ? t('dex.move.effectInEnglish') : null

  const back = <BackLink>{t('dex.back')}</BackLink>
  if (!valid || (move.error instanceof ApiError && move.error.status === 404))
    return (
      <div className="flex flex-col gap-4">
        {back}
        <Callout tone="amber" icon="help">
          {t('dex.move.notFound')}
        </Callout>
      </div>
    )

  const m = move.data
  const effect = m?.effect?.replaceAll('$effect_chance', String(m.effectChance ?? ''))

  return (
    <div className="flex flex-col gap-4">
      {back}
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center">
        <div className="min-w-0 flex-1 space-y-1">
          <h1 className="font-display text-[23px] leading-tight font-bold md:text-[28px]">{m?.name ?? '…'}</h1>
          {m && (
            <div className="flex flex-wrap items-center gap-1.5">
              <TypeBadge type={m.type} />
              {m.class && <Badge>{t(`dex.class.${m.class}`, { defaultValue: m.class })}</Badge>}
            </div>
          )}
        </div>
        <GameSelector value={game} onChange={chooseGame} />
      </div>

      {move.error && (
        <Callout tone="red" icon="error">
          {t('dex.move.error')}
        </Callout>
      )}
      {move.loading && <p className="text-sm text-muted">{t('common.loading')}</p>}

      {m && !m.existsInGame && (
        <Callout tone="amber" icon="info">
          {t('dex.move.notInGame', { game: gameVersionByKey(game).name })}
        </Callout>
      )}

      {m && (
        <>
          <div className="grid grid-cols-2 gap-2 sm:grid-cols-4">
            <Fact label={t('dex.move.power')}>{m.power ?? '—'}</Fact>
            <Fact label={t('dex.move.accuracy')}>{m.accuracy != null ? `${m.accuracy}%` : '—'}</Fact>
            <Fact label={t('dex.move.pp')}>{m.pp ?? '—'}</Fact>
            <Fact label={t('dex.move.priority')}>{m.priority > 0 ? `+${m.priority}` : m.priority}</Fact>
            {m.effectChance != null && <Fact label={t('dex.move.effectChance')}>{m.effectChance}%</Fact>}
            {m.machine && <Fact label={t('dex.move.machine')}>{m.machine}</Fact>}
            <Fact label={t('dex.move.generation')}>{t('dex.generation', { n: m.generation })}</Fact>
          </div>

          {effect && (
            <Section title={t('dex.move.effect')}>
              <EffectText text={effect} notice={english} />
            </Section>
          )}

          <Section title={t('dex.move.learners')}>
            {m.learners.length === 0 && <p className="text-sm text-muted">{t('dex.move.noLearners')}</p>}
            {groupLearners(m.learners).map((g) => (
              <div key={g.method} className="space-y-1.5">
                <h3 className="text-sm font-bold text-muted">
                  {methodLabel[g.method] ? t(`dex.moves.${methodLabel[g.method]}`) : t('dex.move.otherMethod')} ({g.learners.length})
                </h3>
                <div className="grid grid-cols-1 gap-1.5 sm:grid-cols-2 lg:grid-cols-3">
                  {g.learners.map((l, i) => (
                    <SpeciesChip key={`${l.speciesId}-${l.method}-${l.level}-${i}`} species={l.speciesId} name={l.name} game={game}>
                      {l.level != null && <span className="font-mono text-xs text-muted">{t('dex.move.level', { level: l.level })}</span>}
                      {g.method === 'other' && <span className="text-xs text-muted">{titleCase(l.method)}</span>}
                    </SpeciesChip>
                  ))}
                </div>
              </div>
            ))}
          </Section>
        </>
      )}
    </div>
  )
}
