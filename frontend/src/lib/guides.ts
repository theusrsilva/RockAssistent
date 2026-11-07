// Guias (content/<idioma>/guides/**): índice em index.json e páginas em markdown, carregadas sob demanda (chunks
// separados do bundle principal). Diretivas: uma linha inteira ":::nome arg1 arg2" vira um bloco com dados
// (registro em pages/guides/blocks/registry.tsx). O mesmo formato é conferido por scripts/check-guides.py.

export const GUIDE_SECTIONS = ['games', 'mechanics', 'vgc', 'mythology'] as const
export type GuideSection = (typeof GUIDE_SECTIONS)[number]

/** Uma página do guia; <file> é relativo a content/<idioma>/guides/. */
export interface GuidePageRef {
  id: string
  title: string
  file: string
}

/** Um guia (um jogo, uma mecânica, um texto do VGC, uma região da mitologia). */
export interface GuideEntry {
  key: string
  title: string
  description?: string
  sections: GuidePageRef[]
}

export type GuideIndex = Record<GuideSection, GuideEntry[]>

const indexes = import.meta.glob('../../../content/*/guides/index.json', { import: 'default', eager: true }) as Record<string, GuideIndex>

const pages = import.meta.glob('../../../content/*/guides/**/*.md', { query: '?raw', import: 'default' }) as Record<
  string,
  () => Promise<string>
>

const empty: GuideIndex = { games: [], mechanics: [], vgc: [], mythology: [] }

function find<T>(files: Record<string, T>, lang: string, name: string): T | undefined {
  const pick = (l: string) => Object.entries(files).find(([path]) => path.endsWith(`/content/${l}/guides/${name}`))?.[1]
  return pick(lang) ?? pick('pt-BR')
}

/** Índice dos guias no idioma (cai no pt-BR). */
export function guideIndex(lang: string): GuideIndex {
  return { ...empty, ...(find(indexes, lang.startsWith('en') ? 'en' : lang, 'index.json') ?? {}) }
}

export const isGuideSection = (s: string | undefined): s is GuideSection => GUIDE_SECTIONS.includes(s as GuideSection)

export function findGuide(index: GuideIndex, section: string | undefined, slug: string | undefined): GuideEntry | null {
  if (!isGuideSection(section)) return null
  return index[section].find((g) => g.key === slug) ?? null
}

/** Texto de uma página (no idioma, senão no pt-BR); null se o arquivo não existe. */
export async function loadGuide(lang: string, file: string): Promise<string | null> {
  const loader = find(pages, lang.startsWith('en') ? 'en' : lang, file)
  return loader ? loader() : null
}

// ---- Diretivas ----

export type GuideBlock =
  | { type: 'markdown'; text: string }
  | { type: 'directive'; name: string; args: string[]; raw: string }

/** Linha inteira: ":::nome" e argumentos separados por espaço (pode ter recuo, para caber numa lista). */
export const DIRECTIVE_LINE = /^[ \t]*:::([A-Za-z0-9_][\w-]*)(?:[ \t]+(.*?))?[ \t]*$/
const FENCE = /^[ \t]*(```|~~~)/

/** Divide o markdown em blocos de texto e de diretiva. Ignora comentários HTML e o que está em blocos de código. */
export function parseDirectives(source: string): GuideBlock[] {
  const text = source.replace(/<!--[\s\S]*?-->\n?/g, '')
  const blocks: GuideBlock[] = []
  let buffer: string[] = []
  let fence: string | null = null
  const flush = () => {
    const chunk = buffer.join('\n').replace(/^\s*\n/, '').trimEnd()
    if (chunk.trim()) blocks.push({ type: 'markdown', text: chunk })
    buffer = []
  }
  for (const line of text.split('\n')) {
    const f = FENCE.exec(line)
    if (f) fence = fence === null ? f[1] : fence === f[1] ? null : fence
    const m = fence === null && !f ? DIRECTIVE_LINE.exec(line) : null
    if (!m) {
      buffer.push(line)
      continue
    }
    flush()
    const args = m[2] ? m[2].split(/\s+/).filter(Boolean) : []
    blocks.push({ type: 'directive', name: m[1], args, raw: [`:::${m[1]}`, ...args].join(' ') })
  }
  flush()
  return blocks
}
