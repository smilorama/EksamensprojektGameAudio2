# Tomedelic — Script Dokumentation

## Scripts oversigt

| Script | Placering | Ansvar |
|---|---|---|
| `PlayerHealth` | Player | HP, heal, damage, death events |
| `AudioMaterial` | Objekter i scenen | Definerer overflade type til footsteps |
| `TerrainAudioMaterial` | Terrain | Sampler painted terrain layers til overflade type |
| `Footsteps` | Player | Timer-baseret footstep lyd via Wwise; poster på separat emitter |
| `EnemyFootsteps` | Enemy | Animation event-drevet footstep lyd via Wwise med raycast |
| `Enemy` | Enemy | NavMesh, blend tree, aggro, angreb, death; sender RTPC ved aggro/de-aggro/death |
| `EnemyResetOnEmpty` | Enemy Animator (Empty state) | Frigiver attack lock når animation er færdig |
| `DamageZone` | Enemy hånd/våben child | Trigger collider der giver skade til Player; poster hit-lyd via Wwise Event |
| `PlayerDamageZone` | WeaponHolder child | Trigger collider der giver skade til Enemy; aktiveres af WeaponBob; poster hit-lyd |
| `EnemyHealthBar` | Enemy | Fast UI i højre hjørne med nameplate; vises ved aggro eller efter hit; Canvas sortingOrder 50 |
| `WeaponBob` | WeaponHolder (child af Camera) | Svævende hånd: bob, sway, keyframe-baseret melee angreb, swing-lyd |
| `GradientImage` | UI (runtime) | Horisontal/vertikal gradient MaskableGraphic til brug i UI |
| `TomeInteract` | Tome pickup objekt | F-interact: bog stiger op, svæver, emission fader, level design swappes, voiceline spilles; sætter TomeAuraStates+EndGameMusicStates TomePickup; starter 2D stereo aura |
| `WaterfallAudio` | Waterfall objekter | Poster waterfall event i Start; sætter RTPC og skifter renderer farve til #4E0079 når WaterfallAudio.TomePickedUp = true |
| `TomeAuraEmitter` | Cutscene NPC | Poster 3D aura loop i Start |
| `Item` | Pickup objekt | Consumable (heal) eller EventTrigger ved pickup |
| `DialogueUI` | Canvas | Singleton, viser tekst, flag-system |
| `DialogueTrigger` | NPC | Trigger zone, viser linjer, left click for næste; efter "Goddess Freed" dialogue: skifter Animator Controller, aktiverer hånd-objekt, enabler enemy script + healthbar, sætter tag til "Enemy", disabler sig selv |
| `PlayerHealthBar` | Canvas | Slider der viser spillerens HP |
| `PlayerDeathHandler` | Scene GameObject | OnDeath: disabler komponenter, tilføjer Rigidbody til player, fader til sort, viser You Died panel, restarter scene; fader fra sort ved scene load |
| `TomeChoiceStateTrigger` | Trigger collider i scenen | Sætter EndGameMusicStates → TomeChoice når TomePickedUp = true |

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
  → Wwise: PostEvent(_voicelineEvent) ved sequence start (samtidig med hover)
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
| `DamageZone` | — | Inspector: `_hitEvent` (AK.Wwise.Event) |
| `Enemy` | — | Inspector: `_deathEvent`, `_attackEvent` (AK.Wwise.Event), `_aggroRTPC` (AK.Wwise.RTPC) |
| `WaterfallAudio` | — | Inspector: `_waterfallEvent` (AK.Wwise.Event), `_pickupRTPC` (AK.Wwise.RTPC), `_waterfallRenderers` |
| `MusicManager` | — | `StartTome2DStereo()` → `Play_4_2_TomeAura_STEREO_2D`, `MusicBurnEvent()` → `2_5_TomeForeshadowing_SyretFeedbackReverb_BurningScream`, `SetStateTomeAuraNone()` → TomeAuraStates/None |

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

---

## Tome States & Music System

### Scripts

| Script | Placering | Ansvar |
|---|---|---|
| `MusicGameProgressManager` | Statisk klasse | Global boolean `TomePickedUp` |
| `MusicManager` | Scene GameObject (Singleton) | Poster musik events, holder MusicPhase og TomeAuraPhase |
| `MusicBeginningRoomStateTrigger` | Trigger collider i scenen | Sætter Wwise State ved rumingang — kun hvis tomen ikke er picked up |
| `TomeAuraStateTrigger` | Trigger collider i scenen | Sætter Wwise State for tome aura — kun hvis tomen ikke er picked up |
| `TomeInteract` | Tome pickup objekt | Starter endgame musik, sætter PickedUp state, disabler BrazierCutscene |
| `BrazierTomeOffer` | Brazier objekt | Brændingssekvens, sætter "Tome Burned" flag, resetter post processing |
| `TomeLeaveRoom` | Trigger collider ved udgang | Blocker eller outside trigger — sætter "Goddess Freed" flag |
| `BrazierCutscene` | Trigger collider ved brazier | NPC gang/brændings cutscene — disables af TomeInteract ved pickup |

---

### MusicPhase (enum i MusicManager)

| Fase | Hvornår | Wwise Event |
|---|---|---|
| `None` | Start | — |
| `Beginning` | Spilleren går ind i grotterum | `Play_2_Grotte_og_3_Dungeon` + `Play_4_TomeAura_3D_Loop` |
| `Endgame` | Spilleren interacter med tomen | `Play_EndGameMusicSwitchContainer` |

### TomeAuraPhase (enum i MusicManager)

| Fase | Hvornår | Wwise |
|---|---|---|
| `None` | Start | — |
| `Foreshadowing` | Samme tidspunkt som Beginning | `Play_4_TomeAura_3D_Loop` starter |
| `SpiritualRoom` | Via `TomeAuraStateTrigger` | Wwise State sættes fra Inspector |
| `PickedUp` | Via `TomeInteract` | Wwise State sættes fra Inspector (`tomeAuraStateGroup` / `tomePickupStateValue`) |

---

### State Flow

```
[Start]
    |
[Spilleren går ind i grotterum]
    | MusicBeginningRoomStateTrigger → Wwise State (rum-specifik)
    | MusicManager.StartGrotteOgDungeonMusikEvent()
    | → Play_2_Grotte_og_3_Dungeon
    | → Play_4_TomeAura_3D_Loop
    | → MusicPhase: Beginning, TomeAuraPhase: Foreshadowing
    |
[Spilleren nærmer sig tome]
    | TomeAuraStateTrigger(s) → Wwise State: SpiritualRoom (eller andre)
    |
[Spilleren trykker F på tomen]
    | TomeInteract
    | → GlobalVolume.profile = _postTomeProfile
    | → Play_TomePickup_Voice
    | → Wwise State: PickedUp
    | → Play_EndGameMusicSwitchContainer
    | → TomePickedUp = true
    | → BrazierCutscene.collider disabled
    |
         ↙                    ↘
[Brænder tomen]        [Forlader rummet]
BrazierTomeOffer       TomeLeaveRoom (OutsideTrigger)
→ TomePickedUp = false → TomePickedUp = false
→ Flag: "Tome Burned"  → Flag: "Goddess Freed"
→ Post processing reset→ Dialogue audio (kun hvis IKKE brændt)
→ Skybox skifter
→ Fog density sænkes
```

---

### DialogueUI Flags

| Flag | Sat af | Bruges til |
|---|---|---|
| `"Tome Burned"` | `BrazierTomeOffer` | DialogueTrigger (specifikke linjer), ScaleByProximity stop, TextureRotateByProximity stop, GoddessVoiceTrigger silence |
| `"Goddess Freed"` | `TomeLeaveRoom` | DialogueTrigger (specifikke linjer), GoddessVoiceTrigger kræver dette flag |

---

### Wwise Events fra Tome-systemet

| Event | Script | Tidspunkt |
|---|---|---|
| `Play_2_Grotte_og_3_Dungeon` | MusicManager | Første gang spilleren går ind i grotterum |
| `Play_4_TomeAura_3D_Loop` | MusicManager | Samme tidspunkt |
| `Play_TomePickup_Voice` | TomeInteract | Når tomen interactes |
| `Play_EndGameMusicSwitchContainer` | MusicManager | Når tomen interactes |
| `_burnEvent` (Inspector) | BrazierTomeOffer | Brændingssekvens starter |
| `_restoreEvent` (Inspector) | BrazierTomeOffer | Brændingssekvens slutter |
| `_dialogueSoundEvent` (Inspector) | TomeLeaveRoom | Spilleren forlader med tome — kun hvis IKKE brændt |
| `_burnEvent` (Inspector) | BrazierCutscene | NPC antændes i cutscene |

---

## Wwise States — Oversigt

Tre state groups bruges i projektet. States sendes fra scripts via `AkUnitySoundEngine.SetState()`.

---

### BeginningMusicRoomStates
Styrer musikken i starten af spillet afhængigt af hvilket rum spilleren er i. Sættes via `MusicBeginningRoomStateTrigger` — state group og value konfigureres i Inspector. Ignoreres hvis `TomePickedUp = true`.

| State | Hvornår |
|---|---|
| `GrotteBeginning` | Spilleren går ind i grotterummet |
| `DungeonBeginning` | Spilleren går ind i dungeonrummet |

---

### EndGameMusicStates
Styrer musikken efter tomen er picked up. Alle states sendes via `MusicManager`.

| State | Sendt fra | Hvornår |
|---|---|---|
| `TomePickup` | `TomeInteract` | Spilleren interacter med tomen ([F] tryk) — sendes til **både** TomeAuraStates og EndGameMusicStates |
| `TomePossession` | `MusicManager.SetStateTomePossession()` | Hånden med tomen aktiveres tomen er nu i spillerens besiddelse |
| `TomeChoice` | `TomeChoiceStateTrigger` | Spilleren når et bestemt område med tomen |
| `TomeFire` | `BrazierTomeOffer` (ved F-tryk) + `MusicManager.SetStateTomeFire()` | Sendes straks når spilleren trykker F for at brænde tomen |
| `TomeOutside` | `MusicManager.SetStateTomeOutside()` | Spilleren forlader rummet **uden** at have brændt tomen dårlig ending |
| `HappyEndning` | `MusicManager.SetStateTomeOutside()` | Spilleren forlader rummet **efter** at have brændt tomen god ending |
| `PlayerDeath` | `MusicManager.SetStatePlayerDeath()` | Spillerens HP når 0 |

---

### TomeAuraStates
Styrer musikken knyttet til tomens tilstedeværelse i verdenen. Sættes via `TomeAuraStateTrigger` i scenen (state group og value konfigureres i Inspector) dog kun så længe tomen ikke er picked up. Ved pickup overtager `MusicManager` og sætter `TomePickup`.

| State | Sendt fra | Hvornår |
|---|---|---|
| `TomeForeshadowing` | `TomeAuraStateTrigger` | Spilleren nærmer sig tomens område auraen anes |
| `TomeSpiritualRoom` | `TomeAuraStateTrigger` | Spilleren er i det spirituelle rum direkte ved tomen |
| `TomePickup` | `TomeInteract` | Spilleren interacter med tomen ([F] tryk) |
