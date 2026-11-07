import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link, useSearchParams } from 'react-router'
import { Badge, Button, Callout, Card, PageTitle, Segmented, useToast } from '../../../components/ui'
import { spriteUrl } from '../../../lib/api'
import { useApiGet } from '../../../lib/dex'
import { formatIsoDate, importPasteToBoxes, localized, type VgcRegulation, type VgcTeam } from '../../../lib/vgc'
import PasteDialog from './PasteDialog'
import UsageView from './UsageView'

/** "Copiar paste": área de transferência, com aviso. */
export function CopyPasteButton({ paste }: { paste: string }) {
  const { t } = useTranslation()
  const toast = useToast()
  return (
    <Button
      variant="secondary"
      size="sm"
      icon="content_copy"
      onClick={() =>
        void navigator.clipboard
          ?.writeText(paste)
          .then(() => toast(t('vgc.copied')))
          .catch(() => toast(t('vgc.copyFailed')))
      }
    >
      {t('vgc.copy')}
    </Button>
  )
}

/** "Importar para o Boxes": gera os 6 no Scarlet e guarda na Caixa de entrada do cofre. */
export function ImportButton({ paste, disabled = false }: { paste: string; disabled?: boolean }) {
  const { t, i18n } = useTranslation()
  const toast = useToast()
  const [busy, setBusy] = useState(false)
  const [failed, setFailed] = useState<string[]>([])
  async function run() {
    setBusy(true)
    setFailed([])
    try {
      const outcome = await importPasteToBoxes(paste, i18n.language)
      setFailed(outcome.failed)
      toast(t('vgc.imported', { count: outcome.saved }))
    } catch {
      toast(t('vgc.errors.import'))
    } finally {
      setBusy(false)
    }
  }
  return (
    <>
      <Button variant="secondary" size="sm" icon="move_to_inbox" disabled={busy || disabled} onClick={() => void run()}>
        {busy ? t('vgc.importing') : t('vgc.import')}
      </Button>
      {failed.length > 0 && (
        <Callout tone="amber" icon="warning" className="w-full">
          <div className="font-semibold">{t('vgc.importFailed')}</div>
          <div>{failed.join(', ')}</div>
        </Callout>
      )}
    </>
  )
}

function TeamCard({ team, onAnalyze }: { team: VgcTeam; onAnalyze: () => void }) {
  const { t, i18n } = useTranslation()
  const [showPaste, setShowPaste] = useState(false)
  const lang = i18n.language
  return (
    <Card id={team.id} className="flex scroll-mt-20 flex-col gap-3">
      <div className="flex flex-wrap items-center gap-2">
        <h2 className="font-display text-lg font-bold">{team.name}</h2>
        <Badge tone="accent">{t(`vgc.archetypes.${team.archetype}`, { defaultValue: team.archetype })}</Badge>
      </div>
      <div className="flex flex-wrap gap-1">
        {team.members.map((m, i) => (
          <div key={i} className="flex w-[84px] flex-col items-center rounded-lg bg-surface2 px-1 py-1.5 text-center" title={m.item ?? undefined}>
            <img src={spriteUrl(m.species)} alt="" className="size-12 [image-rendering:pixelated]" loading="lazy" />
            <span className="w-full truncate text-[12px] font-semibold">{m.name}</span>
            {m.item && <span className="w-full truncate text-[10.5px] text-muted">{m.item}</span>}
          </div>
        ))}
      </div>
      <dl className="flex flex-col gap-2 text-sm">
        <div>
          <dt className="font-bold">{t('vgc.idea')}</dt>
          <dd className="text-muted">{localized(lang, team.ideaPt, team.ideaEn)}</dd>
        </div>
        <div>
          <dt className="font-bold">{t('vgc.howToPlay')}</dt>
          <dd className="text-muted">{localized(lang, team.howToPlayPt, team.howToPlayEn)}</dd>
        </div>
        <div>
          <dt className="font-bold">{t('vgc.dangers')}</dt>
          <dd className="text-muted">{localized(lang, team.dangersPt, team.dangersEn)}</dd>
        </div>
      </dl>
      <div className="flex flex-wrap gap-2">
        <CopyPasteButton paste={team.paste} />
        <Button variant="secondary" size="sm" icon={showPaste ? 'visibility_off' : 'description'} onClick={() => setShowPaste(!showPaste)}>
          {showPaste ? t('vgc.hidePaste') : t('vgc.showPaste')}
        </Button>
        <Button variant="secondary" size="sm" icon="analytics" onClick={onAnalyze}>
          {t('vgc.analyze')}
        </Button>
        <ImportButton paste={team.paste} />
      </div>
      {showPaste && <pre className="max-h-96 overflow-auto rounded-lg bg-surface2 p-3 text-[12px] leading-snug whitespace-pre">{team.paste}</pre>}
    </Card>
  )
}

type TeamsTab = 'teams' | 'usage'

/** /guias/vgc/times — os times sugeridos da regulamentação vigente e, na outra aba, os mais usados (Smogon). */
export default function TeamsPage() {
  const { t, i18n } = useTranslation()
  const [params, setParams] = useSearchParams()
  const tab: TeamsTab = params.get('aba') === 'usage' ? 'usage' : 'teams'
  const teams = useApiGet<VgcTeam[]>('/api/vgc/teams')
  const regulation = useApiGet<VgcRegulation>('/api/vgc/regulation')
  const [analyzing, setAnalyzing] = useState<VgcTeam | null>(null)
  return (
    <div className="flex flex-col gap-4">
      <div>
        <Link to="/guias#vgc" className="text-sm font-semibold text-accent">
          {t('vgc.backToVgc')}
        </Link>
        <PageTitle>{t('vgc.teamsTitle')}</PageTitle>
        <p className="mt-1 text-sm text-muted">
          {regulation.data
            ? t('vgc.teamsSub', { regulation: regulation.data.name, from: formatIsoDate(regulation.data.validFrom, i18n.language),
                to: regulation.data.validTo ? formatIsoDate(regulation.data.validTo, i18n.language) : '…',
              })
            : t('vgc.teamsSubShort')}
        </p>
        <p className="mt-1 text-[12.5px] text-muted">{t('vgc.importHint')}</p>
      </div>
      <Segmented
        value={tab}
        onChange={(v) => setParams(v === 'usage' ? { aba: 'usage' } : {}, { replace: true })}
        options={[
          { value: 'teams', label: t('vgc.usage.tabTeams') },
          { value: 'usage', label: t('vgc.usage.tab') },
        ]}
        className="max-w-[420px]"
      />
      {tab === 'usage' ? (
        <UsageView />
      ) : teams.loading ? (
        <p className="text-muted">{t('common.loading')}</p>
      ) : teams.error || !teams.data ? (
        <Callout tone="amber" icon="cloud_off">
          {t('guides.block.error')}
        </Callout>
      ) : (
        teams.data.map((team) => <TeamCard key={team.id} team={team} onAnalyze={() => setAnalyzing(team)} />)
      )}
      <Link to="/guias/vgc/minha-equipe" className="text-sm font-semibold text-accent">
        {t('vgc.goToMyTeam')}
      </Link>
      {analyzing && <PasteDialog title={analyzing.name} paste={analyzing.paste} onClose={() => setAnalyzing(null)} />}
    </div>
  )
}
