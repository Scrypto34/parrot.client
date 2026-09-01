using Parrot.client.Classes;
using static Parrot.client.Menu.Main;

namespace Parrot.client.Mods.Settings
{
    public class OpenAnim
    {
        public static readonly string[] names = { "None", "Grow", "Rise", "Wide", "Pop" };
        public static int index = 1;

        public static void Cycle()
        {
            index++;
            index %= names.Length;

            ButtonInfo button = GetIndex("Open Animation");
            if (button != null)
                button.overlapText = "Open Animation: " + names[index];
        }
    }
}
