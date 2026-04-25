using UnityEngine;
using YigitcanCaliskan.EventBus;

namespace BloomJam.Weapons
{
    public readonly struct WeaponSwitchStartedEvent : IEvent
    {
        public readonly int Slot;
        public readonly WeaponData Next;
        public readonly float Duration;

        public WeaponSwitchStartedEvent(int slot, WeaponData next, float duration)
        {
            Slot = slot;
            Next = next;
            Duration = duration;
        }
    }

    public readonly struct WeaponSwitchedEvent : IEvent
    {
        public readonly int Slot;
        public readonly WeaponData Weapon;

        public WeaponSwitchedEvent(int slot, WeaponData weapon)
        {
            Slot = slot;
            Weapon = weapon;
        }
    }

    public readonly struct WeaponFiredEvent : IEvent
    {
        public readonly WeaponData Weapon;
        public readonly Vector3 Origin;
        public readonly Vector3 Direction;
        public readonly int ShotsLeftInMag;

        public WeaponFiredEvent(WeaponData weapon, Vector3 origin, Vector3 direction, int shotsLeftInMag)
        {
            Weapon = weapon;
            Origin = origin;
            Direction = direction;
            ShotsLeftInMag = shotsLeftInMag;
        }
    }

    public readonly struct WeaponReloadStartedEvent : IEvent
    {
        public readonly WeaponData Weapon;
        public readonly float Duration;

        public WeaponReloadStartedEvent(WeaponData weapon, float duration)
        {
            Weapon = weapon;
            Duration = duration;
        }
    }

    public readonly struct WeaponReloadedEvent : IEvent
    {
        public readonly WeaponData Weapon;

        public WeaponReloadedEvent(WeaponData weapon)
        {
            Weapon = weapon;
        }
    }

    public readonly struct WeaponDryFireEvent : IEvent
    {
        public readonly WeaponData Weapon;

        public WeaponDryFireEvent(WeaponData weapon)
        {
            Weapon = weapon;
        }
    }

    public readonly struct AmmoChangedEvent : IEvent
    {
        public readonly WeaponData Weapon;
        public readonly int Current;
        public readonly int Max;

        public AmmoChangedEvent(WeaponData weapon, int current, int max)
        {
            Weapon = weapon;
            Current = current;
            Max = max;
        }
    }
}
