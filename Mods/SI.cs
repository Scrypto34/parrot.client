using BepInEx;
using GorillaGameModes;
using Photon.Pun;
using Parrot.client.Menu;
using Parrot.client.Menu;
using Parrot.client.Notifications;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Parrot.client.Mods
{
    internal class SI
    {
        static SuperInfectionManager _SuperInfectionManager;
        static GhostReactorManager _GhostReactorManager;
        static float SpawnDelay;

        static void GetSuperInfectionManager()
        {
            if (_SuperInfectionManager == null)
            {
                _SuperInfectionManager = UnityEngine.Object.FindObjectOfType<SuperInfectionManager>();
            }
        }

        static void GetGhostReactorManager()
        {
            if (_GhostReactorManager == null)
            {
                _GhostReactorManager = UnityEngine.Object.FindObjectOfType<GhostReactorManager>();
            }
        }

        public static void CreateItemSI(int hash, Vector3 position, Quaternion rotation, long[] createData = null)
        {
            GetSuperInfectionManager();
            if (!NetworkSystem.Instance.IsMasterClient)
            {
                NotifiLib.SendNotification("[ERROR] YOU ARE NOT MASTER!");
                return;
            }

            bool activeModes = GameMode.ActiveGameMode.GameType() == GameModeType.SuperInfect || GameMode.ActiveGameMode.GameType() == GameModeType.SuperCasual;
            if (!activeModes)
            {
                return;
            }

            if (ControllerInputPoller.instance.rightGrab || UnityInput.Current.GetMouseButton(0))
            {
                if (Time.time > SpawnDelay)
                {
                    SpawnDelay = Time.time + 0.1f;

                    var createNetIdMethod = typeof(GameEntityManager).GetMethod("CreateNetId", BindingFlags.NonPublic | BindingFlags.Instance);
                    int netId = (int)createNetIdMethod.Invoke(_SuperInfectionManager.gameEntityManager, new object[] { 1 });

                    if (createData == null) createData = new long[] { 0L };

                    _SuperInfectionManager.gameEntityManager.photonView.RPC("CreateItemRPC", RpcTarget.AllBuffered, new object[] { new int[] { netId }, new int[] { hash }, new long[] { BitPackUtils.PackWorldPosForNetwork(position) }, new int[] { BitPackUtils.PackQuaternionForNetwork(rotation) }, createData, new int[] { 0 } });

                    Safetyy.RPCProtection();
                }
            }
        }

        public static void CreateItemGR(int hash, Vector3 position, Quaternion rotation, long[] createData = null)
        {
            GetGhostReactorManager();
            if (!NetworkSystem.Instance.IsMasterClient)
            {
                NotifiLib.SendNotification("[ERROR] YOU ARE NOT MASTER!");
                return;
            }

            bool zones = ZoneManagement.instance.IsZoneActive(GTZone.ghostReactor) || ZoneManagement.instance.IsZoneActive(GTZone.ghostReactorDrill) || ZoneManagement.instance.IsZoneActive(GTZone.ghostReactorTunnel);
            if (!zones)
            {
                return;
            }

            if (ControllerInputPoller.instance.rightGrab || UnityInput.Current.GetMouseButton(0))
            {
                if (Time.time > SpawnDelay)
                {
                    SpawnDelay = Time.time + 0.1f;
                    var createNetIdMethod = typeof(GameEntityManager).GetMethod("CreateNetId", BindingFlags.NonPublic | BindingFlags.Instance);
                    int netId = (int)createNetIdMethod.Invoke(_GhostReactorManager.gameEntityManager, new object[] { 1 });

                    if (createData == null) createData = new long[] { 0L };

                    _GhostReactorManager.gameEntityManager.photonView.RPC("CreateItemRPC", RpcTarget.AllBuffered, new object[] { new int[] { netId }, new int[] { hash }, new long[] { BitPackUtils.PackWorldPosForNetwork(position) }, new int[] { BitPackUtils.PackQuaternionForNetwork(rotation) }, createData, new int[] { 0 } });

                    Safetyy.RPCProtection();
                }
            }
        }

       public static void VibratingSpring()
       {
           CreateItemSI(1618940484, GorillaTagger.Instance.rightHandTransform.position, Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f)));
       }

        public static void StrangeWood()
        {
            CreateItemSI(-894667703, GorillaTagger.Instance.rightHandTransform.position, Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f)));
        }

        public static void MonkeIdol()
        {
            CreateItemSI(1880272606, GorillaTagger.Instance.rightHandTransform.position, Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f)));
        }

        public static void Stilt()
        {
            CreateItemSI(1447779317, GorillaTagger.Instance.rightHandTransform.position, Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f)));
        }

        public static void YoYo()
        {
            CreateItemSI(1799386883, GorillaTagger.Instance.rightHandTransform.position, Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f)));
        }

        public static void BouncySand()
        {
            CreateItemSI(-1111610567, GorillaTagger.Instance.rightHandTransform.position, UnityEngine.Random.rotation);
        }

        public static void FloppyMetal()
        {
            CreateItemSI(-1409076879, GorillaTagger.Instance.rightHandTransform.position, UnityEngine.Random.rotation);
        }

        public static void WeirdGear()
        {
            CreateItemSI(1573124711, GorillaTagger.Instance.rightHandTransform.position, UnityEngine.Random.rotation);
        }

        public static void StiltExtendo()
        {
            CreateItemSI(683567723, GorillaTagger.Instance.rightHandTransform.position, UnityEngine.Random.rotation);
        }

        public static void StiltFixedScaledLong()
        {
            CreateItemSI(-1906115882, GorillaTagger.Instance.rightHandTransform.position, UnityEngine.Random.rotation);
        }

        public static void StiltFixedScaledShort()
        {
            CreateItemSI(-827046453, GorillaTagger.Instance.rightHandTransform.position, UnityEngine.Random.rotation);
        }

        public static void StiltMotorized2()
        {
            CreateItemSI(1428761418, GorillaTagger.Instance.rightHandTransform.position, UnityEngine.Random.rotation);
        }

        public static void StiltMotorized3()
        {
            CreateItemSI(1996041101, GorillaTagger.Instance.rightHandTransform.position, UnityEngine.Random.rotation);
        }

        public static void StiltTurkey()
        {
            CreateItemSI(686793174, GorillaTagger.Instance.rightHandTransform.position, UnityEngine.Random.rotation);
        }

        public static void TentacleArm()
        {
            CreateItemSI(621310034, GorillaTagger.Instance.rightHandTransform.position, UnityEngine.Random.rotation);
        }

    }
}
