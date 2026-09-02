using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Parrot.client.Mods
{
    public class Projectiles
    {
        private const float ShootStrength = 30f;
        private const float SpamStrength = 25f;
        private const float GunFireRate = 0.12f;
        private const int SnowballSize = 1;

        private static float nextGunFire;

        public static void ProjectileSpam()
        {
            if (!ControllerInputPoller.instance.rightGrab)
                return;

            Fire(GTPlayer.Instance.RightHand, GorillaTagger.Instance.rightHandTransform, RoomSystem.ProjectileSource.RightHand);
        }

        public static void SnowballGun()
        {
            Parrot.client.GunTools.Gunlib.StartBothGuns(() =>
            {
                if (Time.time < nextGunFire)
                    return;
                nextGunFire = Time.time + GunFireRate;

                Transform hand = GorillaTagger.Instance.rightHandTransform;

                Vector3 gunDir = Parrot.client.GunTools.Gunlib.rayDirection.sqrMagnitude > 0.001f
                    ? Parrot.client.GunTools.Gunlib.rayDirection.normalized
                    : -hand.up;

                Vector3 target = Parrot.client.GunTools.Gunlib.nray.collider != null
                    ? Parrot.client.GunTools.Gunlib.nray.point
                    : hand.position + gunDir * 60f;

                Vector3 dir = (target - hand.position).normalized;
                if (dir.sqrMagnitude < 0.001f)
                    dir = gunDir;

                Vector3 startpos = hand.position + dir * 0.2f + Vector3.up * 0.1f;
                Vector3 charvel = GorillaTagger.Instance.rigidbody.linearVelocity + dir * ShootStrength;

                var handVelBtn = Parrot.client.Menu.Main.GetIndex("Include Hand Velocity");
                if (handVelBtn != null && handVelBtn.enabled)
                    charvel = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false) + dir * ShootStrength;

                if (Mouse.current != null && Mouse.current.leftButton.isPressed)
                    charvel = dir * ShootStrength * 2f;

                Color c = Mods.Settings.GunColor.Get(true);

                RoomSystem.SendLaunchProjectile(
                    startpos,
                    charvel,
                    RoomSystem.ProjectileSource.RightHand,
                    SnowballSize,
                    false,
                    (byte)(c.r * 255f), (byte)(c.g * 255f), (byte)(c.b * 255f), 255);
            }, false);
        }

        public static void SnowballSpam()
        {
            if (ControllerInputPoller.instance == null || GTPlayer.Instance == null || GorillaTagger.Instance == null)
                return;

            bool right = ControllerInputPoller.instance.rightGrab ||
                         (Mouse.current != null && Mouse.current.leftButton.isPressed);
            bool left = ControllerInputPoller.instance.leftGrab;

            if (right)
                Fire(GTPlayer.Instance.RightHand, GorillaTagger.Instance.rightHandTransform, RoomSystem.ProjectileSource.RightHand);
            if (left)
                Fire(GTPlayer.Instance.LeftHand, GorillaTagger.Instance.leftHandTransform, RoomSystem.ProjectileSource.LeftHand);
        }

        private static void Fire(GTPlayer.HandState hand, Transform handTransform, RoomSystem.ProjectileSource source)
        {
            Vector3 dir = -handTransform.up;
            Vector3 pos = handTransform.position + dir * 0.2f;
            Vector3 vel = hand.velocityTracker.GetAverageVelocity(true, 0f, false) + dir * SpamStrength;

            RoomSystem.SendLaunchProjectile(
                pos,
                vel,
                source,
                1,
                false,
                255, 255, 255, 255);
        }
    }
}
