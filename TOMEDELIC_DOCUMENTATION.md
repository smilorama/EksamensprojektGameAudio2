# Tomedelic — Script Dokumentation

## Scripts oversigt

| Script | Placering | Ansvar |
|---|---|---|
| `PlayerHealth` | Player | HP, heal, damage, death events |
| `AudioMaterial` | Objekter i scenen | Definerer overflade type til footsteps |
| `TerrainAudioMaterial` | Terrain | Sampler painted terrain layers til overflade type |
| `Footsteps` | Player | Timer-baseret footstep lyd via Wwise |
| `Enemy` | Enemy | NavMesh, blend tree, aggro, angreb |
| `EnemyResetOnEmpty` | Enemy Animator (Empty state) | Frigiver attack lock når animation er færdig |
| `DamageZone` | Enemy hånd/våben child + Player WeaponHolder | Trigger collider der giver skade; bruges af både Enemy og WeaponBob |
| `Item` | Pickup objekt | Consumable (heal) eller EventTrigger ved pickup |
| `DialogueUI` | Canvas | Singleton, viser tekst, flag-system |
| `DialogueTrigger` | NPC | Trigger zone, viser linjer, left click for næste |
| `PlayerHealthBar` | Canvas | Slider der viser spillerens HP |
| `WeaponBob` | WeaponHolder (child af Camera) | Lunacid-stil svævende hånd: bob, sway, melee angreb |
| `PlayerDamageZone` | WeaponHolder child | Trigger collider der giver skade til Enemy; aktiveres af WeaponBob |
| `EnemyHealthBar` | Enemy | Svævende HP-bar over enemies hoved; vises ved aggro eller efter hit |

---

## Afhængigheder

```
PlayerHealth
  ← DamageZone.TakeDamage()
  ← Item.Heal()
  → PlayerHealthBar.OnHealthChanged()

WeaponBob
  → CharacterController.velocity (læses fra Player root)
  → PlayerDamageZone.Activate() / Deactivate() (på WeaponHolder child)

PlayerDamageZone
  → Enemy.TakeDamage()
  ← WeaponBob.Activate() / Deactivate()

Footsteps
  → AudioMaterial (på objekter)
  → TerrainAudioMaterial (på Terrain)
  → Wwise: SetSwitch("Surface", ...) + PostEvent("Play_Footstep")

Enemy
  → EnemyResetOnEmpty (StateMachineBehaviour på Empty state i Action Override layer)
  → Animator: Vertical, Horizontal, isMoving, Attack
  → NavMeshAgent: desiredVelocity driver blend tree

DamageZone
  → PlayerHealth.TakeDamage()
  ← Animation Events: Activate() / Deactivate()

Item
  → PlayerHealth.Heal()  (Consumable)
  → UnityEvent onPickup  (EventTrigger)
  → DialogueUI.SetFlag() (kan kaldes fra onPickup for at låse op for dialog)

DialogueTrigger
  → DialogueUI.ShowLine()
  → DialogueUI.HasFlag()
  → DialogueUI.Hide()

DialogueUI
  ← DialogueTrigger
  ← Item.onPickup → SetFlag()
```

---

## Wwise setup

| Script | Switch Group | Event |
|---|---|---|
| `Footsteps` | `Surface` (Grass / Stone / Dirt) | `Play_Footstep` |

---

## Unity setup checkliste

**Player**
- `PlayerHealth`, `Footsteps`, `CharacterController` — tag: `Player`
- Hierarki under Camera:
  ```
  Camera
    └── WeaponHolder          (WeaponBob.cs) — lokal startposition ca. (0.3, -0.3, 0.6)
          └── HandMesh        (visuelt asset: hånd + våben)
                └── DamageZoneChild   (PlayerDamageZone.cs + trigger collider + kinematic Rigidbody)
  ```
  - `DamageZoneChild` følger HandMesh automatisk — collider'en svinger med våbnet
  - Træk `DamageZoneChild` ind i `WeaponBob`'s Damage Zone-slot i Inspector
  - Enemy-objekter skal have tag `Enemy`

**Enemy**
- `Enemy`, `NavMeshAgent`, `CharacterController`, `Animator`
- Animator: `Vertical` (Float), `Horizontal` (Float), `isMoving` (Bool), `Attack` (Trigger)
- Action Override layer: Empty state → `EnemyResetOnEmpty` StateMachineBehaviour
- Child objekt på hånd/våben: `DamageZone` + trigger collider + Rigidbody (Is Kinematic)

**Terrain**
- `TerrainAudioMaterial` — map layer indeks til Grass/Stone/Dirt

**Objekter i scenen**
- `AudioMaterial` — vælg type i Inspector

**Canvas**
- `DialogueUI` — assign Panel og TextMeshProUGUI
- `PlayerHealthBar` — assign Slider

**NPC**
- `DialogueTrigger` — trigger collider + Rigidbody (Is Kinematic)

**Items**
- `Item` — trigger collider, vælg Consumable eller EventTrigger
