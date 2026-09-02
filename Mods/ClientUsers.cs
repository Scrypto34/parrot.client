using Parrot.client.Classes;
using System.Collections.Generic;
using static Parrot.client.Menu.Main;

namespace Parrot.client.Mods
{
    public class ClientUsers
    {
        public const int Category = 21;
        public const int InspectCategory = 22;

        public static void Open()
        {
            Parrot.client.Menu.Buttons.buttons[Category] = BuildButtons();
            currentCategory = Category;
        }

        public static ButtonInfo[] BuildButtons()
        {
            List<ButtonInfo> page = new List<ButtonInfo>
            {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},
                new ButtonInfo { buttonText = "Refresh Users", method =() => Open(), isTogglable = false, toolTip = "Rescans who is running the menu."},
            };

            foreach (KeyValuePair<int, ClientSync.PlayerState> kv in ClientSync.states)
            {
                ClientSync.PlayerState state = kv.Value;
                page.Add(new ButtonInfo
                {
                    buttonText = state.name + " [" + state.theme + "]",
                    method = () => Inspect(state),
                    isTogglable = false,
                    toolTip = "Inspect this player's theme and mods."
                });
            }

            return page.ToArray();
        }

        private static void Inspect(ClientSync.PlayerState state)
        {
            List<ButtonInfo> page = new List<ButtonInfo>
            {
                new ButtonInfo { buttonText = "Return to Users", method =() => currentCategory = Category, isTogglable = false, toolTip = "Back to the user list."},
                new ButtonInfo { buttonText = "usersname", overlapText = state.name, isTogglable = false, toolTip = "Player name."},
                new ButtonInfo { buttonText = "userstheme", overlapText = "Theme: " + state.theme, isTogglable = false, toolTip = "Their theme."},
            };

            if (state.mods.Count == 0)
            {
                page.Add(new ButtonInfo { buttonText = "usernomods", overlapText = "No mods enabled", isTogglable = false, toolTip = ""});
            }
            else
            {
                for (int i = 0; i < state.mods.Count; i++)
                    page.Add(new ButtonInfo { buttonText = "usermod" + i, overlapText = state.mods[i], isTogglable = false, toolTip = "Enabled by " + state.name});
            }

            Parrot.client.Menu.Buttons.buttons[InspectCategory] = page.ToArray();
            currentCategory = InspectCategory;
        }
    }
}
