A **paste** is the text Showdown, team sites and Champions use to describe a team. Each Pokémon is one block; blocks
are separated by a blank line.

## One block, line by line

```
Incineroar @ Sitrus Berry
Ability: Intimidate
Level: 50
EVs: 252 HP / 4 Atk / 252 SpD
Careful Nature
- Fake Out
- Parting Shot
- Flare Blitz
- Darkest Lariat
```

| Line | What it says |
| --- | --- |
| `Incineroar @ Sitrus Berry` | Species and item. A nickname comes first, with the species in parentheses: `Kitty (Incineroar)` |
| `Ability:` | The ability. It matters a lot in doubles (Intimidate, Drizzle, Psychic Surge) |
| `Level: 50` | In VGC everyone fights at 50; the line is sometimes left out |
| `EVs:` | Where the training points went (in Champions, Stat Points) |
| `... Nature` | The nature: +10% to one stat, −10% to another |
| `- Move` | The 4 moves |
| `IVs:` | Only in pastes from other games (Champions has no IVs); `0 Spe` was for Trick Room |
| `Tera Type:` | Only in Scarlet/Violet pastes; Champions has no Terastallization |

## Common items

- **Sitrus Berry**: heals 25% at half HP. The "default" item for Pokémon that stay in.
- **Leftovers**: heals a little every turn.
- **Life Orb**: +30% damage, loses 10% HP per attack.
- **Choice Scarf**: +50% Speed, but locked into the first move until switching.
- **Focus Sash**: at full HP, survives any hit with 1 HP.
- **Resist Berries** (Chople, Occa, Colbur, Haban...): halve one super effective hit of one type.
- **Seeds** (Grassy, Psychic, Electric, Misty Seed): raise a defense when that terrain is active.
- **White Herb, Mental Herb**: undo one stat drop or one Taunt/Encore, once.
- **Mega Stones** (Charizardite Y, Tyranitarite...): let the Pokémon Mega Evolve. One Mega per battle.

## EVs and Stat Points

- The `252 HP / 4 Atk / 252 SpD` format is the main games' one: up to 252 per stat, 510 total.
- In Champions they are **Stat Points**: 66 total, up to 32 per stat, and each point adds 1 at level 50.
- Conversion: **SP ≈ EV ÷ 8**. 252 EVs ≈ 32 SP; 4 EVs ≈ 1 SP (or 0). The sample teams' pastes use EVs so they open in
  Showdown and the builder; in Champions, build them with the math above.

## Spreads you'll see

| Spread | What for |
| --- | --- |
| `252 Atk / 252 Spe` (or `SpA / Spe`) | Fast attacker: hit hard before the opponent |
| `252 HP / 252 Atk` | Slow or Trick Room attacker |
| `252 HP / 252 Def` or `252 HP / 252 SpD` | Support that needs to last (Follow Me, Trick Room, Intimidate) |
| "Odd" numbers (`124 HP / 76 Def...`) | Calculated to survive a specific hit or outspeed a specific opponent |

## Natures you'll see the most

| Nature | Raises | Lowers | Use |
| --- | --- | --- | --- |
| Adamant | Atk | SpA | Physical attacker |
| Jolly | Spe | SpA | Fast physical attacker |
| Modest | SpA | Atk | Special attacker |
| Timid | Spe | Atk | Fast special attacker |
| Brave / Quiet | Atk / SpA | Spe | Trick Room |
| Careful / Sassy / Bold / Relaxed | SpD / SpD / Def / Def | SpA / Spe / Atk / Spe | Bulky support |

## Reading a whole team

1. Find what starts the plan: weather (Drizzle, Drought, Sand Stream), terrain (Grassy/Psychic Surge), Tailwind or
   Trick Room.
2. Find the control: Fake Out, Intimidate, Follow Me/Rage Powder.
3. Find the hitters: the Pokémon with Life Orb, Choice items or 252 in an attacking stat.
4. Check the speeds: does the team want to be fast (Tailwind) or slow (Trick Room)?
5. Paste it in [My team](/guias/vgc/minha-equipe) to see weaknesses and coverage.
