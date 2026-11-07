import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { Button, Callout, Card, PageTitle, inputClass, useToast } from '../../../components/ui'
import { analyzePaste, loadMyTeam, MAX_PASTE_BYTES, pasteBytes, saveMyTeam, type AnalyzeResult } from '../../../lib/vgc'
import { analyzeErrorText, PasteLinkField, TeamReportView } from './PasteDialog'
import { CopyPasteButton, ImportButton } from './TeamsPage'

/** /guias/vgc/minha-equipe — cola um paste do Showdown, analisa e guarda neste computador. Sem internet. */
export default function MyTeamPage() {
  const { t } = useTranslation()
  const toast = useToast()
  const [paste, setPaste] = useState(loadMyTeam)
  const [busy, setBusy] = useState(false)
  const [result, setResult] = useState<AnalyzeResult | null>(null)
  const [error, setError] = useState<string | null>(null)
  const bytes = pasteBytes(paste)
  const tooBig = bytes > MAX_PASTE_BYTES
  const empty = paste.trim() === ''

  async function analyze() {
    setBusy(true)
    setError(null)
    try {
      setResult(await analyzePaste(paste))
    } catch (e) {
      setResult(null)
      setError(analyzeErrorText(t, e))
    } finally {
      setBusy(false)
    }
  }

  function save() {
    toast(saveMyTeam(paste) ? t('vgc.myTeam.saved') : t('vgc.myTeam.saveFailed'))
  }

  function clear() {
    setPaste('')
    setResult(null)
    setError(null)
    saveMyTeam('')
  }

  return (
    <div className="flex flex-col gap-4">
      <div>
        <Link to="/guias#vgc" className="text-sm font-semibold text-accent">
          {t('vgc.backToVgc')}
        </Link>
        <PageTitle>{t('vgc.myTeam.title')}</PageTitle>
        <p className="mt-1 text-sm text-muted">{t('vgc.myTeam.sub')}</p>
      </div>
      <Card className="flex flex-col gap-3">
        <PasteLinkField
          onPaste={(text) => {
            setPaste(text)
            setResult(null)
            setError(null)
          }}
        />
        <label htmlFor="vgc-paste" className="text-sm font-semibold">
          {t('vgc.myTeam.label')}
        </label>
        <textarea
          id="vgc-paste"
          value={paste}
          onChange={(e) => setPaste(e.target.value)}
          spellCheck={false}
          rows={14}
          placeholder={t('vgc.myTeam.placeholder')}
          className={`${inputClass} h-auto! min-h-64 py-2 font-mono text-[12.5px] leading-snug`}
        />
        <div className={`text-[12px] ${tooBig ? 'font-semibold text-bad' : 'text-muted'}`}>
          {t('vgc.myTeam.size', { kb: (bytes / 1024).toFixed(1), max: MAX_PASTE_BYTES / 1024 })}
        </div>
        <div className="flex flex-wrap gap-2">
          <Button icon="analytics" disabled={busy || empty || tooBig} onClick={() => void analyze()}>
            {busy ? t('vgc.analyzing') : t('vgc.analyze')}
          </Button>
          <Button variant="secondary" icon="save" disabled={empty || tooBig} onClick={save}>
            {t('vgc.myTeam.save')}
          </Button>
          {!empty && <CopyPasteButton paste={paste} />}
          <ImportButton paste={paste} disabled={empty || tooBig} />
          <Button variant="ghost" icon="delete" disabled={empty} onClick={clear}>
            {t('common.clear')}
          </Button>
        </div>
        <p className="text-[12px] text-muted">{t('vgc.myTeam.offline')}</p>
      </Card>
      {error && (
        <Callout tone="amber" icon="error">
          {error}
        </Callout>
      )}
      {result && (
        <Card>
          <TeamReportView result={result} />
        </Card>
      )}
      <Link to="/guias/vgc/times" className="text-sm font-semibold text-accent">
        {t('vgc.goToTeams')}
      </Link>
    </div>
  )
}
