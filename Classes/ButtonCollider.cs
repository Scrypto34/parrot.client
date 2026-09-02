using UnityEngine;
using static Parrot.client.Menu.Main;
using static Parrot.client.Settings;

namespace Parrot.client.Classes
{
	public class Button : MonoBehaviour
	{
		public string relatedText;

		public static float buttonCooldown = 0f;

		public void OnTriggerEnter(Collider collider)
		{
			if (Time.time > buttonCooldown && collider == buttonCollider && menu != null)
			{
                buttonCooldown = Time.time + 0.2f;
                GorillaTagger.Instance.StartVibration(rightHanded, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
                if (Parrot.client.Mods.Settings.Audio.clickSoundIndex == 0)
                    VRRig.LocalRig.PlayHandTapLocal(8, rightHanded, 0.4f);
                if (buttonAnimations && GetComponent<ButtonPunch>() == null)
                    gameObject.AddComponent<ButtonPunch>();

                bool pinMod = false;
                try
                {
                    ControllerInputPoller poller = ControllerInputPoller.instance;
                    pinMod = poller != null && (poller.rightControllerTriggerButton || poller.rightControllerIndexFloat > 0.5f);
                }
                catch { }

                if (pinMod)
                    TogglePin(this.relatedText);
                else
                    Toggle(this.relatedText);
            }
		}
	}
}
