# Tomedelic — Script Dokumentation

## Scripts oversigt

| Script | Placering | Ansvar |
|---|---|---|
| `PlayerHealth` | Player | HP, heal, damage, death events |
| `AudioMaterial` | Objekter i scenen | Definerer overflade type til footsteps |
| `TerrainAudioMaterial` | Terrain | Sampler painted terrain layers til overflade type |
| `Footsteps` | Player | Timer-baseret footstep lyd via Wwise; poster på separat emitter |
| `EnemyFootsteps` | Enemy | Animation event-drevet footstep lyd via Wwise med raycast |
| `Enemy` | Enemy | NavMesh, blend tree, aggro, angreb, death |
| `EnemyResetOnEmpty` | Enemy Animator (Empty state) | Frigiver attack lock når animation er færdig |
| `DamageZone` | Enemy hånd/våben child | Trigger collider der giver skade til Player |
| `PlayerDamageZone` | WeaponHolder child | Trigger collider der giver skade til Enemy; aktiveres af WeaponBob; poster hit-lyd |
| `EnemyHealthBar` | Enemy | Fast UI i højre hjørne med nameplate; vises ved aggro eller efter hit |
| `WeaponBob` | WeaponHolder (child af Camera) | Svævende hånd: bob, sway, keyframe-baseret melee angreb, swing-lyd |
| `GradientImage` | UI (runtime) | Horisontal/vertikal gradient MaskableGraphic til brug i UI |
| `TomeInteract` | Tome pickup objekt | F-interact: bog stiger op, svæver, emission fader, level design swappes, voiceline spilles |
| `Item` | Pickup objekt | Consumable (heal) eller EventTrigger ved pickup |
| `DialogueUI` | Canvas | Singleton, viser tekst, flag-system |
| `DialogueTrigger` | NPC | Trigger zone, viser linjer, left click for næste |
| `PlayerHealthBar` | Canvas | Slider der viser spillerens HP |

---

## Afhængigheder

```
PlayerHealth
  ← DamageZone.TakeDamage()
  ← Item.Heal()
  → PlayerHealthBar.OnHealthChanged()

WeaponBob
  → CharacterController.velocity (læses fra Player root)
  → StarterAssetsInputs.look (sway)
  → PlayerDamageZone.Activate() / Deactivate()
  → Wwise: PostEvent(_swingEvent) ved angreb start

PlayerDamageZone
  → Enemy.TakeDamage()
  → Wwise: PostEvent(_hitEvent) ved kollision med Enemy

EnemyHealthBar
  → Enemy.CurrentHealth / MaxHealth / IsAggro

EnemyFootsteps
  → AudioMaterial (på objekter)
  → TerrainAudioMaterial (på Terrain)
  → Wwise: SetSwitch("Materials", ...) + PostEvent(_footstepEvent)
  ← Animation Events: Step()

Footsteps
  → AudioMaterial (på objekter)
  → TerrainAudioMaterial (på Terrain)
  → Wwise: SetSwitch("Materials", ...) + PostEvent(_footstepEvent) på _footstepEmitter

TomeInteract
  → Volume.profile (skifter til _postTomeProfile)
  → _leftHandWithTome.SetActive(true) ved pickup
  → _preTomeLevelDesign.SetActive(false) / _postTomeLevelDesign.SetActive(true)
  → Wwise: PostEvent(_voicelineEvent) ved sequence slut
  → Renderer._EmissionColor fader 0→3 ved F-tryk, derefter 3→6 i sidste sekund af hover

Enemy
  → EnemyResetOnEmpty (StateMachineBehaviour på Empty state i Action Override layer)
  → Animator: Vertical, Horizontal, isMoving, Attack, Death
  → NavMeshAgent: updatePosition=true, desiredVelocity driver blend tree
  → Wwise: PostEvent(_deathEvent) ved death

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
| `Footsteps` | `Materials` (Grass / Stone / Dirt / Tile) | Inspector: `_footstepEvent` |
| `EnemyFootsteps` | `Materials` (Grass / Stone / Dirt / Tile) | Inspector: `_footstepEvent` |
| `WeaponBob` | — | Inspector: `_swingEvent`, `_hitEvent` |
| `TomeInteract` | — | Inspector: `_voicelineEvent` |
| `PlayerDamageZone` | — | Inspector: `_hitEvent` |
| `Enemy` | — | Inspector: `_deathEvent` |

---

## Unity setup checkliste

**Player**
- `PlayerHealth`, `Footsteps`, `CharacterController` — tag: `Player`
- `Footsteps`: assign `_footstepEmitter` (GameObject med AkGameObj)
- Hierarki under Camera:
  ```
  Camera
    └── WeaponHolder          (WeaponBob.cs) — lokal startposition ca. (0.3, -0.3, 0.6)
          └── HandMesh        (visuelt asset: hånd + våben)
                └── DamageZoneChild   (PlayerDamageZone.cs + trigger collider + kinematic Rigidbody)
  ```
  - Træk `DamageZoneChild` ind i `WeaponBob`'s Damage Zone-slot i Inspector
  - Enemy-objekter skal have tag `Enemy`

**Enemy**
- `Enemy`, `NavMeshAgent`, `Animator`, `EnemyFootsteps`, `EnemyHealthBar`
- Animator: `Vertical` (Float), `Horizontal` (Float), `isMoving` (Bool), `Attack` (Trigger), `Death` (Trigger)
- Action Override layer: Empty state → `EnemyResetOnEmpty` StateMachineBehaviour
- Action Override layer: Death state spilles via `Animator.Play("Death", 1, 0f)`
- Child objekt på hånd/våben: `DamageZone` + trigger collider + Rigidbody (Is Kinematic)
- Animation events på walk/run: kald `Step()` på `EnemyFootsteps`
- `EnemyHealthBar`: sæt `_enemyName` i Inspector

**Terrain**
- `TerrainAudioMaterial` — map layer indeks til Grass/Stone/Dirt/Tile

**Objekter i scenen**
- `AudioMaterial` — vælg type i Inspector

**Canvas**
- `DialogueUI` — assign Panel og TextMeshProUGUI
- `PlayerHealthBar` — assign Slider

**NPC**
- `DialogueTrigger` — trigger collider + Rigidbody (Is Kinematic)

**Items**
- `Item` — trigger collider, vælg Consumable eller EventTrigger

**Tome**
- `TomeInteract` på tome-objektet
- Assign i Inspector:
  - `Global Volume` — scene Volume
  - `Post Tome Profile` — VolumeProfile der aktiveres ved pickup
  - `Left Hand With Tome` — Player child (starter inaktiv)
  - `Pre Tome Level Design` / `Post Tome Level Design` — GameObjects der swappes
  - `Tome Renderer` — Renderer på tome-mesh (til emission)
  - `Interact Prompt Panel` — `TomeInteractPromptPanel` under `TomeInteractPromptCanvas`
  - `Audio Emitter` + `Voiceline Event` — Wwise
- Tryk **F** inden for `_promptRange` (default 2u) for at trigge sekvensen
- Emission: 0→3 over 1.5s ved F-tryk, 3→6 i sidste `_emissionPeakFadeDuration` sekund af hover
