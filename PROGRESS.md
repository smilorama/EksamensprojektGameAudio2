# Game Systems Progress

## Scripts placering
`EksamensprojektUnity/Assets/Scripts/`

---

## 1. PlayerHealth
- [x] `PlayerHealth.cs` — færdig

## 2. Audio Material
- [x] `AudioMaterial.cs` — enum + component til objekter i scenen
- [x] `TerrainAudioMaterial.cs` — sampler terrain painted layers

## 3. Footsteps
- [x] `Footsteps.cs` — timer-baseret, raycast, Wwise Switch + PostEvent

## 4. Enemy
- [x] `Enemy.cs` — NavMesh, RM blend tree (Vertical), Idle/Aggro, attack trigger
- [x] `DamageZone.cs` — trigger collider på hånd/våben, aktiveres via Animation Events

## 5. Item
- [x] `Item.cs` — Consumable (Heal) eller EventTrigger (UnityEvent), destroy on pickup

## 6. Dialogue
- [x] `DialogueUI.cs` — singleton, TextMeshPro, flag-system
- [x] `DialogueTrigger.cs` — liste af linjer med valgfrit flag-krav
