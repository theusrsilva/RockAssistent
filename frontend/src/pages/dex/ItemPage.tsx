import { useTranslation } from 'react-i18next'
import { useParams } from 'react-router'
import { ApiError } from '../../lib/api'
import { dexLang, titleCase, useItem } from '../../lib/dex'
import { Callout } from '../../components/ui'
import { BackLink, EffectText, Fact, Section } from './tabs/parts'

/** Página de um item: categoria, preço, força do Fling e efeito. */
export default function ItemPage() {
  const { t, i18n } = useTranslation()
  const id = Number(useParams().id)
  const valid = Number.isInteger(id) && id > 0
  const item = useItem(valid ? id : 0)
  const english = dexLang(i18n.language) === 'pt-BR' ? t('dex.item.effectInEnglish') : null

  const back = <BackLink history>{t('dex.back')}</BackLink>
  if (!valid || (item.error instanceof ApiError && item.error.status === 404))
    return (
      <div className="flex flex-col gap-4">
        {back}
        <Callout tone="amber" icon="help">
          {t('dex.item.notFound')}
        </Callout>
      </div>
    )

  const it = item.data
  return (
    <div className="flex flex-col gap-4">
      {back}
      <h1 className="font-display text-[23px] leading-tight font-bold md:text-[28px]">{it?.name ?? '…'}</h1>

      {item.error && (
        <Callout tone="red" icon="error">
          {t('dex.item.error')}
        </Callout>
      )}
      {item.loading && <p className="text-sm text-muted">{t('common.loading')}</p>}

      {it && (
        <>
          <div className="grid grid-cols-2 gap-2 sm:grid-cols-3">
            <Fact label={t('dex.item.category')}>{it.category ? titleCase(it.category) : '—'}</Fact>
            <Fact label={t('dex.item.cost')}>{it.cost ? `₽ ${it.cost.toLocaleString(i18n.language)}` : '—'}</Fact>
            <Fact label={t('dex.item.fling')}>{it.flingPower ?? '—'}</Fact>
          </div>
          {it.effect && (
            <Section title={t('dex.item.effect')}>
              <EffectText text={it.effect} notice={english} />
            </Section>
          )}
        </>
      )}
    </div>
  )
}
