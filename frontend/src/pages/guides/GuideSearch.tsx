import { useCallback, useEffect, useRef, useState, type RefObject } from 'react'
import { useTranslation } from 'react-i18next'
import { IconButton, inputClass } from '../../components/ui'
import { findMatches, locate, normalizeQuery } from '../../lib/textSearch'

const ALL = 'guide-search'
const CURRENT = 'guide-search-current'

/**
 * Destaque sem mexer no DOM do React (CSS Custom Highlight API). Sem ela (WebKitGTK < 2.44, Firefox < 140), só o
 * resultado atual ganha uma caixa por cima (<see Overlay>).
 */
const highlights = (): HighlightRegistry | null => (typeof CSS !== 'undefined' && 'highlights' in CSS ? CSS.highlights : null)

/**
 * Caixas fixas (uma por linha do resultado) sobre o resultado atual, nas posições de <c>range.getClientRects()</c>.
 * Recalcula a cada rolagem (do <main> ou de qualquer lugar, na captura) e a cada resize, num rAF.
 */
class Overlay {
  private boxes: HTMLDivElement[] = []
  private range: Range | null = null
  private frame = 0
  private readonly schedule = () => {
    cancelAnimationFrame(this.frame)
    this.frame = requestAnimationFrame(() => this.place())
  }

  show(range: Range | null) {
    this.range = range
    if (range) {
      window.addEventListener('scroll', this.schedule, { capture: true, passive: true })
      window.addEventListener('resize', this.schedule)
    } else this.unlisten()
    this.place()
  }

  dispose() {
    this.show(null)
  }

  private unlisten() {
    cancelAnimationFrame(this.frame)
    window.removeEventListener('scroll', this.schedule, { capture: true })
    window.removeEventListener('resize', this.schedule)
  }

  private place() {
    const rects = this.range ? Array.from(this.range.getClientRects()).filter((r) => r.width > 0 && r.height > 0) : []
    while (this.boxes.length > rects.length) this.boxes.pop()?.remove()
    while (this.boxes.length < rects.length) {
      const box = document.createElement('div')
      box.setAttribute('aria-hidden', 'true')
      box.style.cssText = 'position:fixed;pointer-events:none;z-index:5;border-radius:2px;background:var(--tr-warn);opacity:0.4'
      document.body.appendChild(box)
      this.boxes.push(box)
    }
    rects.forEach((r, i) => {
      const s = this.boxes[i].style
      s.left = `${r.left}px`
      s.top = `${r.top}px`
      s.width = `${r.width}px`
      s.height = `${r.height}px`
    })
  }
}

let overlay: Overlay | null = null

function paint(ranges: Range[], index: number) {
  const registry = highlights()
  if (!registry) {
    if (!overlay) overlay = new Overlay()
    overlay.show(ranges[index] ?? null)
    return
  }
  if (ranges.length === 0) {
    registry.delete(ALL)
    registry.delete(CURRENT)
    return
  }
  registry.set(ALL, new Highlight(...ranges))
  if (ranges[index]) registry.set(CURRENT, new Highlight(ranges[index]))
  else registry.delete(CURRENT)
}

/** Os resultados da consulta nos nós de texto de <root> (o texto todo junto, então acha também entre nós vizinhos). */
function search(root: HTMLElement, query: string): Range[] {
  const nodes: Text[] = []
  const starts: number[] = []
  let joined = ''
  const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT)
  for (let n = walker.nextNode(); n; n = walker.nextNode()) {
    const text = n as Text
    if (!text.data) continue
    nodes.push(text)
    starts.push(joined.length)
    joined += text.data
  }
  if (nodes.length === 0) return []
  return findMatches(joined, query).map((m) => {
    const a = locate(starts, m.start)
    const b = locate(starts, m.end - 1)
    const range = document.createRange()
    range.setStart(nodes[a.node], a.offset)
    range.setEnd(nodes[b.node], b.offset + 1)
    return range
  })
}

/** Abre os <details> fechados em volta do resultado e o centraliza na tela. */
function reveal(range: Range) {
  let el: HTMLElement | null = range.startContainer.parentElement
  for (let p = el; p; p = p.parentElement) if (p instanceof HTMLDetailsElement && !p.open) p.open = true
  el?.scrollIntoView({ block: 'center' })
}

/**
 * Busca dentro da página do guia já renderizada (markdown e blocos de dados). <contentKey> muda quando a página muda:
 * os destaques antigos somem e a busca refaz na página nova, sem rolar (não atrapalha a volta da rolagem). A rolagem
 * só acontece quando a pessoa digita ou anda pelos resultados.
 */
export function useGuideSearch(root: RefObject<HTMLElement | null>, query: string, contentKey: string) {
  const [ranges, setRanges] = useState<Range[]>([])
  const [index, setIndex] = useState(0)
  const scrollNext = useRef(false)

  const run = useCallback(
    (scroll: boolean) => {
      const el = root.current
      const found = el && normalizeQuery(query) ? search(el, query) : []
      setRanges(found)
      setIndex((i) => (scroll ? 0 : Math.min(i, Math.max(found.length - 1, 0))))
      scrollNext.current = scroll
    },
    [root, query],
  )

  // A pessoa digitou: busca (com uma pausa curta) e rola até o primeiro.
  useEffect(() => {
    const id = setTimeout(() => run(true), 150)
    return () => clearTimeout(id)
  }, [run])

  // Página nova ou conteúdo que carregou depois (blocos de dados): refaz sem rolar.
  useEffect(() => {
    const el = root.current
    run(false)
    if (!el) return
    let id: ReturnType<typeof setTimeout> | undefined
    const observer = new MutationObserver(() => {
      clearTimeout(id)
      id = setTimeout(() => run(false), 200)
    })
    observer.observe(el, { childList: true, subtree: true, characterData: true })
    return () => {
      clearTimeout(id)
      observer.disconnect()
    }
    // run fica de fora de propósito: a consulta nova já é tratada pelo efeito de cima.
  }, [root, contentKey])

  useEffect(() => {
    paint(ranges, index)
    if (scrollNext.current && ranges[index]) reveal(ranges[index])
    scrollNext.current = false
  }, [ranges, index])

  useEffect(
    () => () => {
      paint([], 0)
      overlay?.dispose()
    },
    [],
  )

  const go = (step: number) => {
    if (ranges.length === 0) return
    scrollNext.current = true
    setIndex((i) => (i + step + ranges.length) % ranges.length)
  }
  return { count: ranges.length, current: ranges.length > 0 ? index + 1 : 0, next: () => go(1), prev: () => go(-1) }
}

/** Ctrl+F/Cmd+F foca o campo, a não ser que o foco já esteja num campo de texto. */
function useFindShortcut(input: RefObject<HTMLInputElement | null>) {
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => {
      if (!(e.ctrlKey || e.metaKey) || e.altKey || e.key.toLowerCase() !== 'f') return
      const active = document.activeElement
      if (active instanceof HTMLElement && active !== input.current && (active.isContentEditable || active.closest('input, textarea, select'))) return
      e.preventDefault()
      input.current?.focus()
      input.current?.select()
    }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  }, [input])
}

/** Campo "Buscar neste guia": contador, anterior/próximo e limpar. Enter = próximo, Shift+Enter = anterior, Esc limpa. */
export function GuideSearchBar({
  query,
  onQuery,
  count,
  current,
  onNext,
  onPrev,
}: {
  query: string
  onQuery: (q: string) => void
  count: number
  current: number
  onNext: () => void
  onPrev: () => void
}) {
  const { t } = useTranslation()
  const input = useRef<HTMLInputElement>(null)
  useFindShortcut(input)
  const active = normalizeQuery(query) !== ''
  return (
    <div className="sticky top-0 z-10 -mx-1 flex items-center gap-1.5 bg-bg/90 px-1 py-1.5 backdrop-blur-sm" role="search">
      <div className="relative min-w-0 flex-1 sm:max-w-sm">
        <span aria-hidden className="icon pointer-events-none absolute top-1/2 left-2.5 -translate-y-1/2 text-[18px] text-muted">
          search
        </span>
        <input
          ref={input}
          type="text"
          className={`${inputClass} pl-8`}
          value={query}
          placeholder={t('guides.search.placeholder')}
          aria-label={t('guides.search.placeholder')}
          onChange={(e) => onQuery(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === 'Enter') {
              e.preventDefault()
              if (e.shiftKey) onPrev()
              else onNext()
            } else if (e.key === 'Escape') {
              e.preventDefault()
              onQuery('')
            }
          }}
        />
      </div>
      {active && (
        <>
          <span className="shrink-0 text-[12.5px] text-muted tabular-nums" aria-live="polite">
            {count === 0 ? t('guides.search.none') : t('guides.search.count', { current, total: count })}
          </span>
          <IconButton name="expand_less" label={t('guides.search.prev')} className="h-10 w-10" disabled={count === 0} onClick={onPrev} />
          <IconButton name="expand_more" label={t('guides.search.next')} className="h-10 w-10" disabled={count === 0} onClick={onNext} />
          <IconButton name="close" label={t('guides.search.clear')} className="h-10 w-10" onClick={() => onQuery('')} />
        </>
      )}
    </div>
  )
}
