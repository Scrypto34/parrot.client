using GorillaLocomotion;
using Parrot.client.Classes;
using Parrot.client.Notifications;
using System.Collections.Generic;
using UnityEngine;
using static Parrot.client.Menu.Main;

namespace Parrot.client.Mods
{
    public class Players
    {
        public const int Category = 26;
        public const int InspectCategory = 27;

        public static VRRig copyTarget;

        public static void Open()
        {
            Parrot.client.Menu.Buttons.buttons[Category] = BuildList();
            currentCategory = Category;
        }

        private static ButtonInfo[] BuildList()
        {
            List<ButtonInfo> page = new List<ButtonInfo>
            {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},
                new ButtonInfo { buttonText = "Refresh Players", method =() => Open(), isTogglable = false, toolTip = "Rescans everyone in your lobby."},
            };

            int index = 0;
            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (rig == null || rig.isLocal || rig.isOfflineVRRig || rig.Creator == null)
                    continue;

                VRRig target = rig;
                string name = target.Creator.NickName ?? "Player";
                page.Add(new ButtonInfo
                {
                    buttonText = "player" + index++,
                    overlapText = name,
                    isTogglable = false,
                    method = () => Inspect(target),
                    toolTip = "Open actions for " + name + "."
                });
            }

            if (index == 0)
                page.Add(new ButtonInfo { buttonText = "noplayers", overlapText = "No players in your lobby", isTogglable = false, toolTip = "Nobody else is here." });

            return page.ToArray();
        }

        private static void Inspect(VRRig rig)
        {
            string name = rig != null && rig.Creator != null ? rig.Creator.NickName : "Player";

            Parrot.client.Menu.Buttons.buttons[InspectCategory] = new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Return to Players", method =() => currentCategory = Category, isTogglable = false, toolTip = "Back to the player list."},
                new ButtonInfo { buttonText = "playername", overlapText = name, isTogglable = false, toolTip = "Selected player."},
                new ButtonInfo { buttonText = "Teleport To Player", method =() => TeleportTo(rig), isTogglable = false, toolTip = "Teleport to " + name + "."},
                new ButtonInfo { buttonText = "Copy Their Movement", method =() => StartCopy(rig, name), isTogglable = false, toolTip = "Copy " + name + "'s movement (turn off with Copy Player in VRRig)."},
            };

            currentCategory = InspectCategory;
        }

        private static void TeleportTo(VRRig rig)
        {
            if (rig == null)
                return;

            GTPlayer.Instance.transform.position = rig.transform.position + Vector3.up * 0.2f;
            GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
            NotifiLib.SendNotification("<color=grey>[</color><color=cyan>PLAYERS</color><color=grey>]</color> Teleported to " + (rig.Creator != null ? rig.Creator.NickName : "player") + ".");
        }

        private static void StartCopy(VRRig rig, string name)
        {
            copyTarget = rig;

            ButtonInfo toggle = GetIndex("Copy Player");
            if (toggle != null && !toggle.enabled)
                toggle.enabled = true;

            NotifiLib.SendNotification("<color=grey>[</color><color=cyan>PLAYERS</color><color=grey>]</color> Copying " + name + ". Turn off with Copy Player in VRRig.");
            currentCategory = Category;
        }

        public static void CopyMovement()
        {
            VRRig target = copyTarget;
            if (target == null || target == VRRig.LocalRig)
                return;

            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = target.transform.position;
            VRRig.LocalRig.transform.rotation = target.transform.rotation;

            VRRig.LocalRig.head.rigTarget.transform.position = target.head.rigTarget.transform.position;
            VRRig.LocalRig.head.rigTarget.transform.rotation = target.head.rigTarget.transform.rotation;
            VRRig.LocalRig.leftHand.rigTarget.transform.position = target.leftHandTransform.position;
            VRRig.LocalRig.leftHand.rigTarget.transform.rotation = target.leftHandTransform.rotation;
            VRRig.LocalRig.rightHand.rigTarget.transform.position = target.rightHandTransform.position;
            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = target.rightHandTransform.rotation;
        }

        public static void StopCopy()
        {
            copyTarget = null;
            VRRig.LocalRig.enabled = true;
        }
    }
}
