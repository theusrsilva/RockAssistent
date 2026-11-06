import ReactMarkdown, { type Components } from 'react-markdown'
import remarkGfm from 'remark-gfm'
import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { Callout } from '../../components/ui'
import { parseDirectives } from '../../lib/guides'
import { GUIDE_BLOCKS } from './blocks/registry'

const prose =
  'text-sm leading-relaxed [&_p]:my-2 [&_ol]:my-2 [&_ol]:list-decimal [&_ol]:space-y-1 [&_ol]:pl-5 [&_ul]:my-2 [&_ul]:list-disc [&_ul]:space-y-1 [&_ul]:pl-5 [&_code]:rounded [&_code]:bg-surface2 [&_code]:px-1 [&_strong]:font-semibold [&_h2]:mt-6 [&_h2]:mb-2 [&_h2]:font-display [&_h2]:text-xl [&_h2]:font-bold [&_h3]:mt-4 [&_h3]:mb-1.5 [&_h3]:font-bold [&_h3]:text-base [&_a]:text-accent [&_a]:underline [&_hr]:my-4 [&_hr]:border-line'

// Tabelas do markdown com rolagem lateral no celular; links internos pelo roteador.
const components: Components = {
  table: ({ children }) => (
    <div className="my-3 overflow-x-auto rounded-lg border border-line">
      <table className="w-full border-collapse text-left text-[13px]">{children}</table>
    </div>
  ),
  thead: ({ children }) => <thead className="bg-surface2 text-[11.5px] tracking-wide text-muted uppercase">{children}</thead>,
  tbody: ({ children }) => <tbody className="divide-y divide-line">{children}</tbody>,
  th: ({ children }) => <th className="px-2.5 py-2 font-bold">{children}</th>,
  td: ({ children }) => <td className="px-2.5 py-1.5 align-top">{children}</td>,
  a: ({ href, children }) =>
    href?.startsWith('/') ? (
      <Link to={href}>{children}</Link>
    ) : (
      <a href={href} target="_blank" rel="noreferrer">
        {children}
      </a>
    ),
}

/** Uma página de guia: markdown (com tabelas) e os blocos das diretivas. */
export function GuideMarkdown({ text }: { text: string }) {
  const { t } = useTranslation()
  return (
    <div className={prose}>
      {parseDirectives(text).map((block, i) => {
        if (block.type === 'markdown')
          return (
            <ReactMarkdown key={i} remarkPlugins={[remarkGfm]} components={components}>
              {block.text}
            </ReactMarkdown>
          )
        const Block = GUIDE_BLOCKS[block.name]
        if (!Block)
          return (
            <Callout key={i} tone="amber" icon="warning" className="my-3">
              <div className="font-semibold">{t('guides.block.unknown', { name: block.raw })}</div>
              <div>{t('guides.block.unknownHint')}</div>
            </Callout>
          )
        return <Block key={`${i}-${block.raw}`} args={block.args} />
      })}
    </div>
  )
}
