# Weapon System — Implementation TODO

A 2.5D Post-Void-styled FPS weapon system. The "weapon" is the player's right hand
(index finger = barrel), dyed each switch by a left-hand paint bucket. Three slots:
Red Pistol (semi), Green Auto (full-auto, 3 RPS), Blue Shotgun (semi + spread).

This list is the source of truth for sequencing and acceptance. Check items as they land.
Each milestone should leave the project in a compiling, runnable state.

---

## M0 — Project audit (DONE)

- [x] Confirm EventBus API (`YigitcanCaliskan.EventBus`, static, `Publish<T> where T : IEvent`).
- [x] Confirm ServiceLocator API (`YigitcanCaliskan.ServiceLocator`, `Register/Get/TryGet<T>`).
- [x] Confirm InputManager pattern (polling props + `event Action OnX` handlers, `IInputService`).
- [x] Confirm FPS module pattern (`FPSModule.ExecutionOrder`, `Initialize/Tick/LateTick`).
- [x] Confirm Cinemachine version (3.1.6) and impulse usage (`FPSCameraShakeModule`).
- [x] Confirm folder conventions: `Assets/Scripts/<Feature>/`, `Assets/Data/`, `Assets/Prefabs/<Feature>/`.

## M1 — Input plumbing (extend existing manager, do not replace)

- [ ] Add `Slot1`, `Slot2`, `Slot3`, `Reload` button actions to `Assets/Input/PlayerInputs.inputactions` (Gameplay map).
- [ ] Bind to keyboard `1`, `2`, `3`, `R`. Leave gamepad bindings as TODO.
- [ ] Extend `IInputService` with: `event Action OnSlot1/2/3`, `event Action OnReload`, `bool AttackHeld { get; }`.
- [ ] Wire the new actions in `InputManager` (event-driven for slots/reload, polling for `AttackHeld`).

## M2 — Weapon events (EventBus contracts)

- [ ] `Assets/Scripts/Weapons/Events/WeaponEvents.cs`:
  - `WeaponSwitchStartedEvent { int slot, WeaponData next }`
  - `WeaponSwitchedEvent      { int slot, WeaponData weapon }`
  - `WeaponFiredEvent         { WeaponData weapon, Vector3 origin, Vector3 direction, int shotsLeftInMag }`
  - `WeaponReloadStartedEvent { WeaponData weapon }`
  - `WeaponReloadedEvent      { WeaponData weapon }`
  - `AmmoChangedEvent         { WeaponData weapon, int current, int max }`
- All structs implement `IEvent` and live in namespace `BloomJam.Weapons`.

## M3 — Data & strategy

- [ ] `WeaponData` ScriptableObject in `Assets/Scripts/Weapons/Data/WeaponData.cs`:
  fire mode, fire rate, magazine size, damage, range, spread (cone deg + pellet count),
  switch time, reload time, auto-reload flag, sprite sequence, particle prefab,
  muzzle flash prefab, impulse profile, screen-flash color/intensity/duration,
  audio clips (fire/reload/switch), tint color, display name.
- [ ] `IWeaponBehaviour` interface in `Assets/Scripts/Weapons/Behaviours/IWeaponBehaviour.cs`:
  `void Initialize(WeaponRuntimeContext ctx)`, `void Tick(float dt)`, `void OnAttackPressed()`,
  `void OnAttackReleased()`, `void OnEquip()`, `void OnHolster()`.
- [ ] `WeaponRuntimeContext` POCO carrying: WeaponController, WeaponData, magazine state, fire origin, player camera.

## M4 — Weapon strategies (one per slot)

- [ ] `PistolBehaviour`     — semi-auto, 1 shot per click, mag 6.
- [ ] `AutomaticBehaviour`  — full-auto while held, 3 RPS, mag 18.
- [ ] `ShotgunBehaviour`    — semi-auto, mag 4, N pellets in cone (configurable on data).
- All three share fire-cooldown, ammo-decrement, and event publication helpers via a small `WeaponBehaviourBase` abstract class.

## M5 — WeaponController + service

- [ ] `IWeaponService` (read-only API for HUD: `WeaponData CurrentWeapon`, `int CurrentAmmo`, `int MaxAmmo`, `bool IsBusy`).
- [ ] `WeaponController` MonoBehaviour:
  - Holds `WeaponData[3]` slot list (serialized).
  - Subscribes to `IInputService` events on `OnEnable`, unsubs on `OnDisable`.
  - Locks shoot/switch input during switch animation and reload.
  - Drives the active `IWeaponBehaviour.Tick` from `Update`.
  - Publishes all weapon events through `EventBus`.
- [ ] Implements `IBootstrapService` so `BootstrapRunner` registers it as `IWeaponService`.

## M6 — Visual feedback

- [ ] `WeaponHandView` MonoBehaviour: drives the right-hand `SpriteRenderer`, swaps to a shorter sprite per shot fired, restores original on reload. Reads sprite sequence from `WeaponData`.
- [ ] `MuzzleFlash` MonoBehaviour: short-lived flash sprite; spawned from `WeaponData.muzzleFlashPrefab`.
- [ ] `WeaponParticleSpawner`: spawns the per-weapon particle prefab tinted to its color.
- [ ] `WeaponCameraShake` MonoBehaviour: holds a `CinemachineImpulseSource`; subscribes to `WeaponFiredEvent` and applies the per-weapon impulse profile.
- [ ] `ScreenFlashController`: fullscreen UI image; on `WeaponFiredEvent` flashes color×intensity for duration. Tune per weapon: pistol punchy, auto rapid+light, shotgun heavy.

## M7 — Audio hooks (stubbed)

- [ ] `WeaponAudioPlayer` MonoBehaviour with one `AudioSource`; subscribes to fire/reload/switch events; plays clips from `WeaponData`. Audio clip fields are TODO assets in the SO.

## M8 — Assets & wiring (must be done in the Unity Editor)

- [ ] **Skipped intentionally:** No asmdef — project convention is Assembly-CSharp.
      Adding one would force every consumer to add a reference; not worth the friction at this stage.
- [ ] Create three `WeaponData` assets under `Assets/Data/Weapons/` via right-click →
      `Create → BloomJam → Weapons → Weapon Data`:
  - `Weapon_Pistol_Red`    — BehaviourKind=Pistol,    FireMode=SemiAuto, FireRate=4,
    MagazineSize=6,  TintColor=red,   ShakeForce=0.35, ScreenFlash=red×0.15×0.06s.
  - `Weapon_Auto_Green`    — BehaviourKind=Automatic, FireMode=FullAuto, FireRate=3,
    MagazineSize=18, TintColor=green, ShakeForce=0.15, ScreenFlash=green×0.08×0.04s.
  - `Weapon_Shotgun_Blue`  — BehaviourKind=Shotgun,   FireMode=SemiAuto, FireRate=1.2,
    MagazineSize=4,  PelletsPerShot=8, SpreadDegrees=8, TintColor=blue,
    ShakeForce=0.9,  ScreenFlash=blue×0.3×0.12s.
- [ ] Player prefab edits:
  - Add `WeaponController` to the player root, drag the three SOs into `Slots[0..2]`,
    set `FireOrigin` (a child at the muzzle/fingertip) and `AimSource` (the FPS camera).
  - Add a child `WeaponView` GameObject parented to the camera with:
    `WeaponHandView` (+ SpriteRenderer), `WeaponMuzzleView` (with a child `MuzzleAnchor`),
    `WeaponCameraShake` (+ CinemachineImpulseSource), `WeaponAudioPlayer` (+ AudioSource),
    optional `WeaponAnimatorRelay` (+ Animator).
  - Add a fullscreen UI Canvas with a stretched `Image` (alpha 0, raycastTarget off) and
    `WeaponScreenFlash` pointing at it.
- [ ] Bootstrap scene: ensure `WeaponController` is a child of `BootstrapRunner` so its
      `IBootstrapService.Register()` runs (or call `Replace<IWeaponService>` from another installer).
- All art / audio remain placeholder `// TODO assets` — leave the SO fields empty until art lands.

## M9 — Polish (later passes)

- [ ] Animation hooks: switch & reload anim events drive lock/unlock.
- [ ] Pour-onto-hand color transition (left-hand bucket → right-hand tint).
- [ ] Per-weapon recoil kick (slight FOV bump or camera punch).
- [ ] Hit-scan raycast result exposed on `WeaponFiredEvent` so a downstream damage system can subscribe.
- [ ] Per-weapon sprite-cycling tween easing.
- [ ] Editor inspector polish on `WeaponData` (preview color swatch).

## M10 — Scope explicitly excluded from this pass

- Enemy damage / hit reactions / projectile collision.
- Pickup/drop, ammo crates, weapon unlocking.
- HUD UI for ammo (event contract is in place; UI is downstream).
- Networking.

---

## Conventions to keep

- All tunables on `WeaponData`. No magic numbers in MonoBehaviours.
- `[SerializeField] [Tooltip(...)]` for every inspector field.
- Defensive null checks + `Debug.LogError` with class name + intent.
- Single-responsibility components; new behaviour = new file.
- No new singletons. No new event/input systems. Use `ServiceLocator` and `EventBus`.
