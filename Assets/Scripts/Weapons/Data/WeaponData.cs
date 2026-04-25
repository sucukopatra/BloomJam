using UnityEngine;

namespace BloomJam.Weapons
{
    public enum WeaponBehaviourKind
    {
        Pistol,
        Automatic,
        Shotgun,
    }

    [CreateAssetMenu(menuName = "BloomJam/Weapons/Weapon Data", fileName = "Weapon_New")]
    public sealed class WeaponData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Display name shown in HUD/debug.")]
        public string DisplayName = "Weapon";

        [Tooltip("Picks which IWeaponBehaviour strategy WeaponController spawns at runtime.")]
        public WeaponBehaviourKind BehaviourKind = WeaponBehaviourKind.Pistol;

        [Tooltip("Tint color for particles, screen flash, and (eventually) the right-hand sprite dye.")]
        public Color TintColor = Color.white;

        [Header("Firing")]
        [Tooltip("Rounds per second. For semi-auto this caps click spam.")]
        [Min(0.01f)] public float FireRate = 4f;

        [Tooltip("Damage per shot (per pellet for shotguns). Hit-detection is downstream.")]
        public float Damage = 10f;

        [Tooltip("Hit-scan range in meters.")]
        [Min(0f)] public float Range = 100f;

        [Tooltip("Cone half-angle in degrees. 0 = perfectly straight.")]
        [Range(0f, 30f)] public float SpreadDegrees = 0f;

        [Tooltip("Pellets per trigger pull. 1 for pistol/auto, >1 for shotgun.")]
        [Min(1)] public int PelletsPerShot = 1;

        [Header("Ammo")]
        [Tooltip("Magazine capacity. Reserve is treated as infinite.")]
        [Min(1)] public int MagazineSize = 6;

        [Tooltip("If true, an empty magazine triggers a reload automatically on next fire attempt.")]
        public bool AutoReloadOnEmpty = true;

        [Header("Timings")]
        [Tooltip("Seconds the switch animation plays. Input is locked during this window.")]
        [Min(0f)] public float SwitchTime = 0.35f;

        [Tooltip("Seconds the reload animation plays. Input is locked during this window.")]
        [Min(0f)] public float ReloadTime = 1.2f;

        [Header("Hand Sprite Cycle (TODO art)")]
        [Tooltip("Index 0 = full-length hand (also restored on reload). Each subsequent shot advances the index, clamped to last.")]
        public Sprite[] HandSpriteCycle;

        [Header("VFX (TODO art)")]
        [Tooltip("Particle prefab spawned at the muzzle each shot. Will be tinted to TintColor.")]
        public GameObject MuzzleParticlePrefab;

        [Tooltip("Sprite/quad muzzle flash prefab spawned for one frame at the muzzle.")]
        public GameObject MuzzleFlashPrefab;

        [Header("Camera Shake")]
        [Tooltip("Force passed to CinemachineImpulseSource.GenerateImpulse on each shot.")]
        [Min(0f)] public float ShakeForce = 0.25f;

        [Header("Screen Flash")]
        public Color ScreenFlashColor = Color.white;
        [Range(0f, 1f)] public float ScreenFlashIntensity = 0.15f;
        [Min(0f)] public float ScreenFlashDuration = 0.06f;

        [Header("Audio (TODO clips)")]
        public AudioClip FireClip;
        public AudioClip ReloadClip;
        public AudioClip SwitchClip;
        public AudioClip DryFireClip;

        [Header("Animation (optional)")]
        [Tooltip("Animator trigger fired on equip. Empty = no trigger.")]
        public string SwitchAnimTrigger = "Switch";

        [Tooltip("Animator trigger fired on reload. Empty = no trigger.")]
        public string ReloadAnimTrigger = "Reload";

        [Tooltip("Animator trigger fired on each shot. Empty = no trigger.")]
        public string FireAnimTrigger = "Fire";

        [Tooltip("Animator trigger fired when the player tries to fire on an empty mag. Empty = no trigger.")]
        public string DryFireAnimTrigger = "DryFire";
    }
}
