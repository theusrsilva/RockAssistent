import { TYPES, formatMultiplier, multiplierClass } from '../../lib/dex'
import { useTypeName } from './TypeBadge'

/** Grade com o dano que cada tipo de golpe causa (18 células; tipos que o jogo não tem ficam de fora). */
export function DamageTakenGrid({ damageTaken }: { damageTaken: Record<string, number> }) {
  const typeName = useTypeName()
  const types = TYPES.filter((t) => t in damageTaken)
  return (
    <div className="grid grid-cols-3 gap-1.5 sm:grid-cols-6">
      {types.map((type) => {
        const m = damageTaken[type]
        return (
          <div key={type} className={`flex flex-col items-center rounded-lg px-1 py-1.5 ${multiplierClass(m)}`}>
            <span className="text-[11.5px] font-semibold">{typeName(type)}</span>
            <span className="font-mono text-sm font-bold">{formatMultiplier(m)}</span>
          </div>
        )
      })}
    </div>
  )
}
