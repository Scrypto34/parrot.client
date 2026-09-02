using Parrot.client.Classes;
using Parrot.client.Notifications;
using System.Collections.Generic;
using static Parrot.client.Menu.Main;

namespace Parrot.client.Mods.Console
{

    public class Console
    {
        public const int Category = 15;

        public static void Open()
        {
            OwnerList.EnsureLoaded();

            if (OwnerList.TryGetName(out string name))
            {
                currentCategory = Category;
                NotifiLib.SendNotification("<color=grey>[</color><color=green>CONSOLE</color><color=grey>]</color> Welcome, Your an console admin now! " + name + ".");
                return;
            }

            if (!OwnerList.Loaded)
                NotifiLib.SendNotification("<color=grey>[</color><color=yellow>CONSOLE</color><color=grey>]</color> Still checking access, try again in a moment.");
            else
                NotifiLib.SendNotification("<color=grey>[</color><color=red>CONSOLE</color><color=grey>]</color> You are not a console admin.");
        }

        public static ButtonInfo[] BuildButtons()
        {
            List<ButtonInfo> page = new List<ButtonInfo>
            {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},

                new ButtonInfo { buttonText = "Ban Hammer", method = () => BanHammer.Run(), disableMethod = () => BanHammer.Despawn(), toolTip = "Spawns a hammer in your hand. Hit a player with it to kick them."},

                new ButtonInfo { buttonText = "Tung Tung Tung Sahur", method = () => Sahur.Run(), disableMethod = () => Sahur.Despawn(), toolTip = "Spawns a big Tung Tung Tung Sahur on your wrist. Only players with the checker can see it."},

                new ButtonInfo { buttonText = "Seal Gun", method = () => SealGun.Run(), toolTip = "Hold grip to aim and trigger to shoot seals. Only players with the checker can see them."},

                new ButtonInfo { buttonText = "Travis Scott", method = () => TravisScott.Run(), disableMethod = () => TravisScott.Despawn(), toolTip = "Spawns Travis Scott at a fixed spot (needs travis scott.bundle). Only players with the checker can see it."},

                new ButtonInfo { buttonText = "Carti Jackson", method = () => CartiJackson.Run(), disableMethod = () => CartiJackson.Despawn(), toolTip = "Spawns a twerking figure at a fixed spot. Only players with the checker can see it."},

                new ButtonInfo { buttonText = "Sans", method = () => Sans.Run(), disableMethod = () => Sans.Despawn(), toolTip = "Holds a Sans model on your wrist."},
            };

            return page.ToArray();
        }
    }
}
