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

        public static readonly Sound[] openSounds = new Sound[]
        {
            new Sound { name = "None", fileName = null },
            new Sound { name = "Splash", fileName = "Splash.wav" },
            new Sound { name = "Gmod", fileName = "Gmod.ogg" },
        };

        public static readonly Sound[] clickSounds = new Sound[]
        {
            new Sound { name = "None", fileName = null },
            new Sound { name = "Tap", fileName = "Tap.wav" },
            new Sound { name = "Click", fileName = "Click.wav" },
            new Sound { name = "Button", fileName = "Button.mp3" },
            new Sound { name = "Tap 2", fileName = "Tap2.wav" },
            new Sound { name = "UI Click", fileName = "UIClick.mp3" },
            new Sound { name = "Wood", fileName = "Wood.ogg" },
            new Sound { name = "Zoom", fileName = "Zoom.ogg" },
            new Sound { name = "Slider", fileName = "Slider.ogg" },
            new Sound { name = "Roblox Button", fileName = "robloxbutton.ogg" },
            new Sound { name = "Click 2", fileName = "click.ogg" },
            new Sound { name = "Steal", fileName = "steal.ogg" },
            new Sound { name = "Sensation", fileName = "sensation.ogg" },
        };

        public static int openSoundIndex = 1;
        public static int clickSoundIndex = 0;

        private static Sound[] SoundsFor(bool openSound) => openSound ? openSounds : clickSounds;

        private static Sound GetOpenSound(int index) =>
            index >= 0 && index < openSounds.Length ? openSounds[index] : openSounds[0];

        private static Sound GetClickSound(int index) =>
            index >= 0 && index < clickSounds.Length ? clickSounds[index] : clickSounds[0];

        public static void PlayOpenSound()
        {
            AudioLib.Play(GetOpenSound(openSoundIndex).fileName);
            AudioLib.Preload(GetClickSound(clickSoundIndex).fileName);
        }

        public static void PlayClickSound() => AudioLib.Play(GetClickSound(clickSoundIndex).fileName);

        public static ButtonInfo[] BuildSoundButtons(bool openSound)
        {
            Sound[] list = SoundsFor(openSound);

            List<ButtonInfo> page = new List<ButtonInfo>
            {
                new ButtonInfo { buttonText = "Return to Audio Settings", method =() => currentCategory = 12, isTogglable = false, toolTip = "Returns to the audio settings page."}
            };

            for (int i = 0; i < list.Length; i++)
            {
                int index = i;
                page.Add(new ButtonInfo
                {
                    buttonText = SoundButtonText(openSound, list[index].name),
                    overlapText = SoundButtonLabel(openSound, index),
                    method = () => SelectSound(openSound, index),
                    isTogglable = false,
                    toolTip = $"Plays {list[index].name} when {(openSound ? "the menu opens" : "you press a button")}."
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
            AudioLib.Play(SoundsFor(openSound)[index].fileName);
        }

        public static void RefreshLabels()
        {
            ButtonInfo openNav = GetIndex("Open Sound");
            if (openNav != null)
                openNav.overlapText = $"Open Sound [{GetOpenSound(openSoundIndex).name}]";

            ButtonInfo clickNav = GetIndex("Click Sound");
            if (clickNav != null)
                clickNav.overlapText = $"Click Sound [{GetClickSound(clickSoundIndex).name}]";

            for (int i = 0; i < openSounds.Length; i++)
            {
                ButtonInfo openButton = GetIndex(SoundButtonText(true, openSounds[i].name));
                if (openButton != null)
                    openButton.overlapText = SoundButtonLabel(true, i);
            }

            for (int i = 0; i < clickSounds.Length; i++)
            {
                ButtonInfo clickButton = GetIndex(SoundButtonText(false, clickSounds[i].name));
                if (clickButton != null)
                    clickButton.overlapText = SoundButtonLabel(false, i);
            }
        }

        private static string SoundButtonText(bool openSound, string name) =>
            (openSound ? "Open Sound: " : "Click Sound: ") + name;

        private static string SoundButtonLabel(bool openSound, int index)
        {
            Sound[] list = SoundsFor(openSound);
            bool selected = index == (openSound ? openSoundIndex : clickSoundIndex);
            return selected ? $"[{list[index].name}]" : list[index].name;
        }
    }
}
