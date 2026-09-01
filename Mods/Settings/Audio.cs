using Parrot.client.Classes;
using System.Collections.Generic;
using static Parrot.client.Menu.Main;

namespace Parrot.client.Mods.Settings
{
    public class Sound
    {
        public string name = "None";
        public string fileName = null;
    }

    public class Audio
    {

        public static readonly Sound[] sounds = new Sound[]
        {
            new Sound { name = "None", fileName = null },
            new Sound { name = "Splash", fileName = "Splash.wav" },
            new Sound { name = "Tap", fileName = "Tap.wav" },
            new Sound { name = "Click", fileName = "Click.wav" },
            new Sound { name = "Button", fileName = "Button.mp3" },
            new Sound { name = "Tap 2", fileName = "Tap2.wav" },
            new Sound { name = "UI Click", fileName = "UIClick.mp3" },
            new Sound { name = "Wood", fileName = "Wood.ogg" },
            new Sound { name = "Zoom", fileName = "Zoom.ogg" },
            new Sound { name = "Slider", fileName = "Slider.ogg" },
            new Sound { name = "Gmod", fileName = "Gmod.ogg" },
        };

        public static int openSoundIndex = 1;
        public static int clickSoundIndex = 0;

        public static void PlayOpenSound()
        {
            PlaySound(openSoundIndex);
            AudioLib.Preload(GetSound(clickSoundIndex).fileName);
        }

        public static void PlayClickSound() => PlaySound(clickSoundIndex);

        private static void PlaySound(int index) => AudioLib.Play(GetSound(index).fileName);

        private static Sound GetSound(int index) =>
            index >= 0 && index < sounds.Length ? sounds[index] : sounds[0];

        public static ButtonInfo[] BuildSoundButtons(bool openSound)
        {
            List<ButtonInfo> page = new List<ButtonInfo>
            {
                new ButtonInfo { buttonText = "Return to Audio Settings", method =() => currentCategory = 12, isTogglable = false, toolTip = "Returns to the audio settings page."}
            };

            for (int i = 0; i < sounds.Length; i++)
            {
                int index = i;
                page.Add(new ButtonInfo
                {
                    buttonText = SoundButtonText(openSound, sounds[index].name),
                    overlapText = SoundButtonLabel(openSound, index),
                    method = () => SelectSound(openSound, index),
                    isTogglable = false,
                    toolTip = $"Plays {sounds[index].name} when {(openSound ? "the menu opens" : "you press a button")}."
                });
            }

            return page.ToArray();
        }

        public static void SelectSound(bool openSound, int index)
        {
            if (openSound)
                openSoundIndex = index;
            else
                clickSoundIndex = index;

            RefreshLabels();
            PlaySound(index);
        }

        public static void RefreshLabels()
        {
            ButtonInfo openNav = GetIndex("Open Sound");
            if (openNav != null)
                openNav.overlapText = $"Open Sound [{GetSound(openSoundIndex).name}]";

            ButtonInfo clickNav = GetIndex("Click Sound");
            if (clickNav != null)
                clickNav.overlapText = $"Click Sound [{GetSound(clickSoundIndex).name}]";

            for (int i = 0; i < sounds.Length; i++)
            {
                ButtonInfo openButton = GetIndex(SoundButtonText(true, sounds[i].name));
                if (openButton != null)
                    openButton.overlapText = SoundButtonLabel(true, i);

                ButtonInfo clickButton = GetIndex(SoundButtonText(false, sounds[i].name));
                if (clickButton != null)
                    clickButton.overlapText = SoundButtonLabel(false, i);
            }
        }

        private static string SoundButtonText(bool openSound, string name) =>
            (openSound ? "Open Sound: " : "Click Sound: ") + name;

        private static string SoundButtonLabel(bool openSound, int index)
        {
            bool selected = index == (openSound ? openSoundIndex : clickSoundIndex);
            return selected ? $"[{sounds[index].name}]" : sounds[index].name;
        }
    }
}
