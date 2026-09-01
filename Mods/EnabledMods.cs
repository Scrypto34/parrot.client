using Parrot.client.Classes;
using System.Collections.Generic;
using static Parrot.client.Menu.Main;

namespace Parrot.client.Mods
{
    public class EnabledMods
    {
        public const int Category = 24;

        public static void Open()
        {
            Parrot.client.Menu.Buttons.buttons[Category] = BuildButtons();
            currentCategory = Category;
        }

        private static bool IsSkippedCategory(int c) =>
            c == 1 || c == 2 || c == 3 || c == 11 || c == 12 || c == 16 || c == 21 || c == 22 || c == Category;

        public static ButtonInfo[] BuildButtons()
        {
            List<ButtonInfo> page = new List<ButtonInfo>
            {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},
                new ButtonInfo { buttonText = "Refresh Enabled", method =() => Open(), isTogglable = false, toolTip = "Rescans your enabled mods."},
            };

            int index = 0;
            for (int c = 0; c < Parrot.client.Menu.Buttons.buttons.Length; c++)
            {
                if (IsSkippedCategory(c))
                    continue;

                foreach (ButtonInfo button in Parrot.client.Menu.Buttons.buttons[c])
                {
                    if (!button.isTogglable || !button.enabled)
                        continue;

                    ButtonInfo real = button;
                    string label = real.overlapText ?? real.buttonText;
                    page.Add(new ButtonInfo
                    {
                        buttonText = "enabledmod" + index++,
                        overlapText = label,
                        isTogglable = false,
                        toolTip = "Tap to turn off " + label + ".",
                        method = () =>
                        {
                            real.enabled = false;
                            if (real.disableMethod != null)
                                try { real.disableMethod.Invoke(); } catch { }
                            try { ClientSync.BroadcastNow(); } catch { }
                            Open();
                        }
                    });
                }
            }

            if (index == 0)
            {
                page.Add(new ButtonInfo { buttonText = "enabledmodnone", overlapText = "No mods enabled", isTogglable = false, toolTip = "You have no mods turned on." });
            }
            else
            {
                page.Insert(2, new ButtonInfo
                {
                    buttonText = "Disable All Mods",
                    isTogglable = false,
                    toolTip = "Turns off every enabled mod at once.",
                    method = () =>
                    {
                        for (int c = 0; c < Parrot.client.Menu.Buttons.buttons.Length; c++)
                        {
                            if (IsSkippedCategory(c))
                                continue;

                            foreach (ButtonInfo b in Parrot.client.Menu.Buttons.buttons[c])
                            {
                                if (b.isTogglable && b.enabled)
                                {
                                    b.enabled = false;
                                    if (b.disableMethod != null)
                                        try { b.disableMethod.Invoke(); } catch { }
                                }
                            }
                        }
                        try { ClientSync.BroadcastNow(); } catch { }
                        Open();
                    }
                });
            }

            return page.ToArray();
        }
    }
}
