import { useTranslation } from 'react-i18next'
import { Badge, Callout, Card } from '../../components/ui'
import type { RunView } from './nuzlocke'
import { BattlePokemon, CardTitle } from './parts'

/** Treinadores comuns da área (Gen 1–5); nos outros jogos, só o aviso. */
export default function TrainersCard({ view, names }: { view: RunView; names: Map<number, string> }) {
  const { t } = useTranslation()
  return (
    <Card className="flex flex-col gap-3">
      <CardTitle icon="groups">{t('nuzlocke.trainers.title')}</CardTitle>
      {!view.trainersAvailable ? (
        <Callout tone="sky" icon="info">
          {t('nuzlocke.trainers.unavailable')}
        </Callout>
      ) : view.areaTrainers.length === 0 ? (
        <p className="text-sm text-muted">{t('nuzlocke.trainers.empty')}</p>
      ) : (
        <div className="flex flex-col gap-3">
          {view.areaTrainers.map((tr) => (
            <div key={tr.id} className="flex flex-col gap-1.5">
              <div className="flex flex-wrap items-center gap-1.5 text-sm">
                <span className="text-muted">{tr.class}</span>
                <span className="font-bold">{tr.name}</span>
                {!tr.verified && <Badge>{t('nuzlocke.boss.notVerified')}</Badge>}
              </div>
              {tr.party.map((p, i) => (
                <BattlePokemon key={i} p={p} names={names} />
              ))}
            </div>
          ))}
        </div>
      )}
    </Card>
  )
}
