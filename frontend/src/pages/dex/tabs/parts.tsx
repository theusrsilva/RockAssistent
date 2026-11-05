import type { ReactNode } from 'react'

/** Título de seção dentro de uma aba. */
export function Section({ title, children, aside }: { title: ReactNode; children: ReactNode; aside?: ReactNode }) {
  return (
    <section className="flex flex-col gap-2.5 rounded-xl border border-line bg-surface p-4 sm:p-5">
      <div className="flex flex-wrap items-center gap-2">
        <h2 className="flex-1 font-display text-lg font-bold">{title}</h2>
        {aside}
      </div>
      {children}
    </section>
  )
}

/** Par rótulo/valor das fichas. */
export function Fact({ label, children }: { label: ReactNode; children: ReactNode }) {
  return (
    <div className="rounded-lg bg-surface2 px-3 py-2">
      <div className="text-[11.5px] font-semibold text-muted">{label}</div>
      <div className="text-sm font-semibold">{children}</div>
    </div>
  )
}
