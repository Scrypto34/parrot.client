using Parrot.client.Mods;
using Parrot.client.Classes;
using Parrot.client.Mods;
using UnityEngine;
using static Parrot.client.Menu.Main;
using static Parrot.client.Settings;

namespace Parrot.client.Menu
{
    public class Buttons
    {

        public static ButtonInfo[][] buttons = new ButtonInfo[][]
        {
            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Settings", method =() => currentCategory = 1, isTogglable = false, toolTip = "Opens the main settings page for the menu."},
                new ButtonInfo { buttonText = "Room Mods", method =() => currentCategory = 4, isTogglable = false, toolTip = "Opens the room mods tab."},
                new ButtonInfo { buttonText = "Enabled Mods", method =() => Mods.EnabledMods.Open(), isTogglable = false, toolTip = "Shows every mod you have on. Tap one to turn it off."},
                new ButtonInfo { buttonText = "Players", method =() => Mods.Players.Open(), isTogglable = false, toolTip = "See everyone in your lobby and do stuff to them."},
                new ButtonInfo { buttonText = "VRRig", method =() => currentCategory = 25, isTogglable = false, toolTip = "Opens the VRRig mods tab."},
                new ButtonInfo { buttonText = "Movement Mods", method =() => currentCategory = 5, isTogglable = false, toolTip = "Opens the movement mods tab."},
                new ButtonInfo { buttonText = "OP Mods", method =() => currentCategory = 9, isTogglable = false, toolTip = "Opens the Overpowered mods tab."},
                new ButtonInfo { buttonText = "Safety Mods", method =() => currentCategory = 6, isTogglable = false, toolTip = "Opens the safety mods tab."},
                new ButtonInfo { buttonText = "Advantage ", method =() => currentCategory = 7, isTogglable = false, toolTip = "Opens the Advantage mods tab."},
                new ButtonInfo { buttonText = "Fun", method =() => currentCategory = 8, isTogglable = false, toolTip = "Opens the Fun mods tab."},
                new ButtonInfo { buttonText = "Visuals", method =() => currentCategory = 23, isTogglable = false, toolTip = "Opens the Visuals tab."},
                new ButtonInfo { buttonText = "Weather", method =() => currentCategory = 28, isTogglable = false, toolTip = "Change the time of day: Morning, Day, Evening, Night."},
                new ButtonInfo { buttonText = "Sounds", method = () => currentCategory = 10, isTogglable = false, toolTip = "Opens the Sound mods tab." },
                new ButtonInfo { buttonText = "Console", method = () => Parrot.client.Mods.Console.Console.Open(), isTogglable = false, toolTip = "Owner only tools." },
                new ButtonInfo { buttonText = "Soundboard", method = () => currentCategory = 17, isTogglable = false, toolTip = "Plays sounds from your parrot.client/sounds folder." },
                new ButtonInfo { buttonText = "Projectiles", method = () => currentCategory = 18, isTogglable = false, toolTip = "Opens the projectile mods tab." },
                new ButtonInfo { buttonText = "Master Client ", method = () => currentCategory = 19, isTogglable = false, toolTip = "Opens the master client mods tab." },
                new ButtonInfo { buttonText = "Detected Mods", method = () => currentCategory = 20, isTogglable = false, toolTip = "Opens the detected mods tab." },
                new ButtonInfo { buttonText = "Client Users", method = () => Mods.ClientUsers.Open(), isTogglable = false, toolTip = "See other menu users' theme and enabled mods." },

            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},
                new ButtonInfo { buttonText = "Menu", method =() => currentCategory = 2, isTogglable = false, toolTip = "Opens the settings for the menu."},
                new ButtonInfo { buttonText = "Mods", method =() => currentCategory = 3, isTogglable = false, toolTip = "Opens the mod settings (fly speed, prediction speed, tag aura distance)."},
                new ButtonInfo { buttonText = "Gun", method =() => currentCategory = 11, isTogglable = false, toolTip = "Opens the gun settings for the menu."},
                new ButtonInfo { buttonText = "Audio", method =() => currentCategory = 12, isTogglable = false, toolTip = "Opens the audio settings for the menu."},
                new ButtonInfo { buttonText = "Notification Settings", method =() => currentCategory = 29, isTogglable = false, toolTip = "Choose what you get notified for and the notification color."},
                new ButtonInfo { buttonText = "Console Settings", method =() => currentCategory = 16, isTogglable = false, visible = () => Parrot.client.Classes.OwnerList.HasAccess(), toolTip = "Console admin settings, like your crown color."},
            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Settings", method =() => currentCategory = 1, isTogglable = false, toolTip = "Returns to the main settings page for the menu."},
                new ButtonInfo { buttonText = "Panic", method =() => Menu.Main.Panic(), isTogglable = false, toolTip = "Instantly disables every mod and resets all settings to default."},
                new ButtonInfo { buttonText = "Right Hand", enableMethod =() => rightHanded = true, disableMethod =() => rightHanded = false, toolTip = "Puts the menu on your right hand."},
                new ButtonInfo { buttonText = "Notifications", enableMethod =() => disableNotifications = false, disableMethod =() => disableNotifications = true, enabled = !disableNotifications, toolTip = "Toggles the notifications."},
                new ButtonInfo { buttonText = "FPS Counter", enableMethod =() => fpsCounter = true, disableMethod =() => fpsCounter = false, enabled = fpsCounter, toolTip = "Toggles the FPS counter."},
                new ButtonInfo { buttonText = "Disconnect Button", enableMethod =() => disconnectButton = true, disableMethod =() => disconnectButton = false, enabled = disconnectButton, toolTip = "Toggles the disconnect button."},
                new ButtonInfo { buttonText = "Rounded Corners", enableMethod =() => roundedCorners = true, disableMethod =() => roundedCorners = false, enabled = roundedCorners, toolTip = "Rounds the corners of the menu buttons and background."},
                new ButtonInfo { buttonText = "Theme: Default", method =() => Classes.ThemeChanger.NextTheme(), isTogglable = false, toolTip = "Cycles through available themes."},
                new ButtonInfo { buttonText = "Theme Mode", overlapText = "Theme Mode: Dark", method =() => Classes.ThemeChanger.ToggleThemeMode(), isTogglable = false, toolTip = "Switches all themes between Dark and Light."},
                new ButtonInfo { buttonText = "Menu Smoothing", overlapText = "Menu Smoothing: Off", method =() => Mods.Settings.Smoothing.Cycle(), isTogglable = false, toolTip = "Makes the menu smoothly follow your hand. Off, Low, Medium, High."},
                new ButtonInfo { buttonText = "Arrow Style", overlapText = "Arrow Style: Default", method =() => Mods.Settings.ArrowStyle.Cycle(), isTogglable = false, toolTip = "Switches the page arrows between Default and big Side arrows."},
                new ButtonInfo { buttonText = "Menu Size", overlapText = "Menu Size: Normal", method =() => Mods.Settings.MenuScale.Cycle(), isTogglable = false, toolTip = "Changes how wide the whole menu is. Scales everything so the UI stays intact."},
                new ButtonInfo { buttonText = "Button Animations", enableMethod =() => buttonAnimations = true, disableMethod =() => buttonAnimations = false, enabled = buttonAnimations, toolTip = "Makes every button pop in when the menu opens."},
                new ButtonInfo { buttonText = "Menu Font", overlapText = "Menu Font: Default", method =() => Mods.Settings.Fonts.ChangeMenuFont(), isTogglable = false, toolTip = "Cycles through the fonts in Resources/Server/Fonts."},
                new ButtonInfo { buttonText = "Open Animation", overlapText = "Open Animation: Grow", method =() => Mods.Settings.OpenAnim.Cycle(), isTogglable = false, toolTip = "Cycles the menu open animation."},
                new ButtonInfo { buttonText = "Save Config", method =() => Classes.ThemeChanger.SaveConfig(), isTogglable = false, toolTip = "Saves your current theme and mod configuration."},
                new ButtonInfo { buttonText = "Load Config", method =() => Classes.ThemeChanger.LoadConfig(), isTogglable = false, toolTip = "Loads your saved theme and mod configuration."},
                new ButtonInfo { buttonText = "Outline", enableMethod =() => rainbowOutline = true, disableMethod =() => rainbowOutline = false, enabled = rainbowOutline, toolTip = "Puts an outline around the menu background."},
                new ButtonInfo { buttonText = "Outline Color", overlapText = "Outline Color [Gradient]", method =() => Mods.Settings.Outline.Cycle(), isTogglable = false, toolTip = "Changes the outline color: Gradient, Pink, Blue, Black, Cyan, Brown."},
                new ButtonInfo { buttonText = "Custom Boards", method =() => Mods.BoardMod.Apply(), disableMethod =() => Mods.BoardMod.Restore(), toolTip = "Recolors the map boards (stump, etc.) to match your menu theme color."},
            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Settings", method =() => currentCategory = 1, isTogglable = false, toolTip = "Returns to the main settings page for the menu."},

                new ButtonInfo { buttonText = "Change Fly Speed", overlapText = "Change Fly Speed [Medium]", method =() => Mods.Settings.Movement.ChangeFlySpeed(), isTogglable = false, toolTip = "Changes the speed of the fly mod."},
                new ButtonInfo { buttonText = "Wall Walk Speed", overlapText = "Wall Walk Speed [Medium]", method =() => Mods.Settings.Movement.ChangePullSpeed(), isTogglable = false, toolTip = "Changes how hard the Wall Walk mod pushes you forward."},
                new ButtonInfo { buttonText = "Tag Aura Distance", overlapText = "Tag Aura Distance [Medium]", method =() => Mods.Settings.Movement.ChangeTagAuraDistance(), isTogglable = false, toolTip = "Changes the range of the Tag Aura mod."},
                new ButtonInfo { buttonText = "Ring Distance", overlapText = "Ring Distance [Medium]", method =() => Mods.Settings.Movement.ChangeRingDistance(), isTogglable = false, toolTip = "Changes the size and tag range of the Aura Ring."},
                new ButtonInfo { buttonText = "Anti Report Sensitivity", overlapText = "Anti Report Sensitivity [Medium]", method =() => Mods.Settings.Movement.ChangeAntiReportSensitivity(), isTogglable = false, toolTip = "How close a report hand must get before Anti Report disconnects you. Higher = triggers from further."},
            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},

                new ButtonInfo { buttonText = "Disconnect", method =() => NetworkSystem.Instance.ReturnToSinglePlayer(), isTogglable = false, toolTip = "Disconnects you from the room."},
                new ButtonInfo { buttonText = "Lobby Hop", method =() => Rooms.LobbyHop(), isTogglable = false, toolTip = "Leaves and rejoins a fresh public lobby."},
                new ButtonInfo { buttonText = "Set Room ID To Femboy (CS)", method =() => Rooms.SetRoomIdToFemboy(), isTogglable = false, toolTip = "Sets the room ID to 'femboy'."},
                new ButtonInfo { buttonText = "Set Room ID To UWU (CS)", method =() => Rooms.SetRoomIdToUwu(), isTogglable = false, toolTip = "Sets the room ID to 'uwu'."},
                new ButtonInfo { buttonText = "Set Room ID To Scrypto Is a Boykisser (CS)", method =() => Rooms.SetRoomIdToScryptoIsaBoyKisser(), isTogglable = false, toolTip = "Sets the room ID to 'scryptoisaboykisser'."},
                new ButtonInfo { buttonText = "Set Room ID To FemboyFurry (CS)", method =() => Rooms.SetRoomIdToFemboyFurry(), isTogglable = false, toolTip = "Sets the room ID to 'FEMBOYFURRY'."},
                new ButtonInfo { buttonText = "Set Room ID To Parrot Stinks (CS)", method =() => Rooms.SetRoomIdToParrotStinks(), isTogglable = false, toolTip = "Sets the room ID to 'PARROT STINKS'."},
                new ButtonInfo { buttonText = "Join Code MOD", method =() => Rooms.JoinCode("MOD"), isTogglable = false, toolTip = "Joins the MOD code."},
                new ButtonInfo { buttonText = "Join Code MODS", method =() => Rooms.JoinCode("MODS"), isTogglable = false, toolTip = "Joins the MODS code."},
                new ButtonInfo { buttonText = "Join Code PARROT", method =() => Rooms.JoinCode("PARROT"), isTogglable = false, toolTip = "Joins the PARROT code."},
                new ButtonInfo { buttonText = "Join Code SCRYPTO", method =() => Rooms.JoinCode("SCRYPTO"), isTogglable = false, toolTip = "Joins the SCRYPTO code."},
                new ButtonInfo { buttonText = "Join Code HIDE", method =() => Rooms.JoinCode("HIDE"), isTogglable = false, toolTip = "Joins the HIDE code."},
                new ButtonInfo { buttonText = "Join Code GHOST", method =() => Rooms.JoinCode("GHOST"), isTogglable = false, toolTip = "Joins the GHOST code."},
                new ButtonInfo { buttonText = "Join Code BANSHEE", method =() => Rooms.JoinCode("BANSHEE"), isTogglable = false, toolTip = "Joins the BANSHEE code."},
                new ButtonInfo { buttonText = "Join Code SEEK", method =() => Rooms.JoinCode("SEEK"), isTogglable = false, toolTip = "Joins the SEEK code."},
                new ButtonInfo { buttonText = "Join Code J3VU", method =() => Rooms.JoinCode("J3VU"), isTogglable = false, toolTip = "Joins the J3VU code."},
                new ButtonInfo { buttonText = "Join Code FEMBOY", method =() => Rooms.JoinCode("FEMBOY"), isTogglable = false, toolTip = "Joins the FEMBOY code."},
                new ButtonInfo { buttonText = "Join Code UWU", method =() => Rooms.JoinCode("UWU"), isTogglable = false, toolTip = "Joins the UWU code."},
                new ButtonInfo { buttonText = "Join Code TRANS", method =() => Rooms.JoinCode("TRANS"), isTogglable = false, toolTip = "Joins the TRANS code."},
                new ButtonInfo { buttonText = "Join Code LGBTQ", method =() => Rooms.JoinCode("LGBTQ"), isTogglable = false, toolTip = "Joins the LGBTQ code."},
                new ButtonInfo { buttonText = "Join Code CAT", method =() => Rooms.JoinCode("CAT"), isTogglable = false, toolTip = "Joins the CAT code."},
                new ButtonInfo { buttonText = "Join Code DOG", method =() => Rooms.JoinCode("DOG"), isTogglable = false, toolTip = "Joins the DOG code."},
                new ButtonInfo { buttonText = "Join Code MENU", method =() => Rooms.JoinCode("MENU"), isTogglable = false, toolTip = "Joins the MENU code."},
                new ButtonInfo { buttonText = "Join Code BIGBACK", method =() => Rooms.JoinCode("BIGBACK"), isTogglable = false, toolTip = "Joins the BIGBACK code."},
                new ButtonInfo { buttonText = "Join Code FAT", method =() => Rooms.JoinCode("FAT"), isTogglable = false, toolTip = "Joins the FAT code."},
                new ButtonInfo { buttonText = "Join Code HI", method =() => Rooms.JoinCode("HIDE"), isTogglable = false, toolTip = "Joins the HI code."},
                new ButtonInfo { buttonText = "Join Code NEW", method =() => Rooms.JoinCode("HIDE"), isTogglable = false, toolTip = "Joins the NEW code."},
                new ButtonInfo { buttonText = "Join Code NOOB", method =() => Rooms.JoinCode("NOOB"), isTogglable = false, toolTip = "Joins the NOOB code."},
                new ButtonInfo { buttonText = "Join Code GTAG", method =() => Rooms.JoinCode("GTAG"), isTogglable = false, toolTip = "Joins the GTAG code."},
                new ButtonInfo { buttonText = "Join Code 67", method =() => Rooms.JoinCode("67"), isTogglable = false, toolTip = "Joins the 67 code."},
                new ButtonInfo { buttonText = "Join Code 69", method =() => Rooms.JoinCode("69"), isTogglable = false, toolTip = "Joins the 69 code."},
                new ButtonInfo { buttonText = "Join Code BRAINROT", method =() => Rooms.JoinCode("BRAINROT"), isTogglable = false, toolTip = "Joins the BRAINROT code."},
                new ButtonInfo { buttonText = "Join Code GHOSTS", method =() => Rooms.JoinCode("GHOSTS"), isTogglable = false, toolTip = "Joins the GHOSTS code."},

            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},

                new ButtonInfo { buttonText = "Fly (RP)", method =() => Movement.Fly(), toolTip = "Fly forward while holding the right primary button."},
                new ButtonInfo { buttonText = "Auto Funny Run (RG)", method =() => Fun.AutoFunnyRun(), toolTip = "Hold right grip to swing your hands in a funny running motion."},
                new ButtonInfo { buttonText = "Auto Elevator Climb (RG)", method =() => Movement.AutoElevatorClimb(), toolTip = "Hold right grip to auto climb like an elevator."},
                new ButtonInfo { buttonText = "Wall Walk", method =() => Movement.Pull(), toolTip = "Hold right grip to push forward where you look and climb along walls your hands touch."},
                new ButtonInfo { buttonText = "Grapple Gun (RG)", method =() => Movement.GrappleGun(), toolTip = "Hold grip and pull the trigger to yank yourself toward where you aim."},
                new ButtonInfo { buttonText = "No Speed Limit", method =() => Movement.NoSpeedLimit(), disableMethod =() => Movement.RestoreSpeedLimit(), toolTip = "Removes your movement speed cap."},
                new ButtonInfo { buttonText = "Slingshot Fly", method =() => Movement.SlingshotFly(), toolTip = "Fly with accelerating movement while holding the right primary button."},
                new ButtonInfo { buttonText = "WASD Fly ", method =() => Movement.WASDFly(), toolTip = "Fly using WASD, Space, and Ctrl."},
                new ButtonInfo { buttonText = "Disable Stationary WASD Fly", toolTip = "When enabled, WASD fly falls when you stop moving instead of hovering."},
                new ButtonInfo { buttonText = "Platforms (RG)", method =() => Movement.Platforms(), toolTip = "Creates platforms under your hands."},
                new ButtonInfo { buttonText = "Teleport Gun (RG)", method =() => Movement.TeleportGun(), toolTip = "Teleport to the location targeted by the gun."},
                new ButtonInfo { buttonText = "Motion Trail", method =() => Fun.MotionTrail(), disableMethod =() => Fun.MotionTrailStop(), toolTip = "Leaves a glowing trail behind you in your menu theme color as you move."},
                new ButtonInfo { buttonText = "Double Jump", method =() => Movement.DoubleJump(), toolTip = "Press A (right primary) in the air to jump again."},
                new ButtonInfo { buttonText = "Fly Toward Gun", method =() => Movement.FlyTowardGun(), toolTip = "Hold grip to aim at a player, pull the trigger to fly toward them."},
                new ButtonInfo { buttonText = "Speed Boost", method =() => Movement.SpeedBoost(), toolTip = "Increases your jump speed."},
                new ButtonInfo { buttonText = "IShowSpeedJR", method =() => Movement.IshowSpeedJR(), toolTip = "Greatly increases your jump speed."},
                new ButtonInfo { buttonText = "Walk On Water", method =() => Movement.WalkOnWater(), toolTip = "Allows you to walk on water."},
                new ButtonInfo { buttonText = "Bouncy", method =() => Movement.Bouncy(), disableMethod =() => Movement.ResetBouncy(), toolTip = "Makes your body bouncy."},
                new ButtonInfo { buttonText = "Joystick Fly (J)", method =() => Movement.JoystickFly(), toolTip = "Fly using the controller joysticks."},
                new ButtonInfo { buttonText = "Mosaboost", method =() => Movement.Mosaboost(), toolTip = "Greatly increases jump speed and multiplier."},
                new ButtonInfo { buttonText = "No Tag Freeze", method =() => Movement.NoTagFreeze(), toolTip = "Disables the tag movement freeze."},
                new ButtonInfo { buttonText = "Tag Freeze", method =() => Movement.TagFreeze(), toolTip = "Freezes your movement."},
                new ButtonInfo { buttonText = "Air Swim", method =() => Movement.AirSwim(), toolTip = "Allows you to swim through the air."},
                new ButtonInfo { buttonText = "Disable Air Swim", method =() => Movement.DisableAirSwim(), toolTip = "Disables Air Swim."},
                new ButtonInfo { buttonText = "Fast Swim", method =() => Movement.FastSwim(), toolTip = "Makes you swim faster."},
                new ButtonInfo { buttonText = "Car Monkey (RP) (LP)", method =() => Movement.CarMonkey(), toolTip = "Move forward and backward like a car."},
                new ButtonInfo { buttonText = "TP To Stump", method =() => Movement.TpToStump(), toolTip = "Teleports you to the stump."},
                new ButtonInfo { buttonText = "TP To City", method =() => Movement.TpToCity(), toolTip = "Teleports you to the city."},
                new ButtonInfo { buttonText = "TP To Tut", method =() => Movement.TpToTut(), toolTip = "Teleports you to the tutorial area."},
                new ButtonInfo { buttonText = "Iron Monke (RT)", method =() => Movement.IronMonke(), toolTip = "Punch with increased force."},

            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},

                new ButtonInfo { buttonText = "Anti Report", method =() => Safety.AntiReportDisconnect(), toolTip = "Disconnects you when someone tries to report you."},
                new ButtonInfo { buttonText = "Hide Name", enableMethod =() => Safety.HideNameOnLeaderboard(), disableMethod =() => Safety.RestoreName(), toolTip = "Blanks your name on the leaderboard. Turn off to restore it."},
            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},
                new ButtonInfo { buttonText = "Tag Gun (RG)", method = () => Advantage.TagGun(), disableMethod = () => Advantage.ReleaseTagGun(), toolTip = "Tags the player youre aiming at." },
                new ButtonInfo { buttonText = "Tag All", method = () => Advantage.TagAll(), toolTip = "Tags everyone." },
                new ButtonInfo { buttonText = "Tag Aura", method = () => Advantage.TagAura(), toolTip = "While infected, auto-tags any player who comes within the tag aura distance." },
                new ButtonInfo { buttonText = "Aura Ring", method = () => Advantage.AuraRing(), disableMethod = () => Advantage.StopAuraRing(), toolTip = "A spinning glowing ring around you in your theme color. While infected, it tags anyone who steps inside it." },
                new ButtonInfo { buttonText = "Tag Self", method = () => Advantage.TagSelf(),isTogglable = true, disableMethod = () => Advantage.ReleaseTagSelf(), toolTip = "Tags yourself." },
                new ButtonInfo { buttonText = "No Tag On Join", method = () => Advantage.NoTagOnJoin(), toolTip = "No tag on join" },
                new ButtonInfo { buttonText = "Untag Self", method = () => Advantage.UntagSelf(), toolTip = "Removes the tag/infection from yourself while enabled." },
            },

            new ButtonInfo [] {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},

                new ButtonInfo { buttonText = "Loud Hand Taps (B)", method = () => Fun.LoudHandTaps(), toolTip = "Makes hand taps extremely loud." },
                new ButtonInfo { buttonText = "Silent Hand Taps (B)", method = () => Fun.SilentHandTaps(), toolTip = "Disables hand tap sounds." },
                new ButtonInfo { buttonText = "Prioritize Voice Gun (RG)", method = () => Fun.PrioritizeVoiceGun(), toolTip = "Makes the locked player's voice louder and other voices quieter." },
                new ButtonInfo { buttonText = "Mute Gun (RG)", method = () => Fun.MuteGun(), toolTip = "Toggles the locked player's mute status." },

                new ButtonInfo { buttonText = "Watergun (RG)", method = () => Fun.Watergun(), toolTip = "Shoots water splash effects at the locked player." },
                new ButtonInfo { buttonText = "Give Water Splash Hands Gun (RG)", method = () => Fun.GiveWaterSplashHandsGun(), toolTip = "Makes the locked player's hands create water splash effects." },
                new ButtonInfo { buttonText = "Water Splash Aura", method = () => Fun.WaterSplashAura(), toolTip = "Creates water splash effects around you." },
                new ButtonInfo { buttonText = "Orbit Water Splash", method = () => Fun.OrbitWaterSplash(), toolTip = "Creates orbiting water splash effects around you." },
                new ButtonInfo { buttonText = "Splash Annoy All", method = () => Fun.SplashAnnoyAll(), toolTip = "Sends splash effects to other players." },
                new ButtonInfo { buttonText = "Water Splash (RG)", method = () => Overpowered.Watersplash(), toolTip = "Creates water splash effects from your hands." },

                new ButtonInfo { buttonText = "Glider Gun (RG)", method = () => Fun.GliderGun(), toolTip = "Moves your glider to the selected gun position." },
                new ButtonInfo { buttonText = "Orbit Gliders", method = () => Fun.OrbitGliders(), toolTip = "Makes your gliders orbit around you." },
                new ButtonInfo { buttonText = "Glider Orbit Player Gun (RG)", method = () => Fun.GliderOrbitPlayerGun(), toolTip = "Makes your gliders orbit around the locked player." },
                new ButtonInfo { buttonText = "Glider Blind Gun (RG)", method = () => Fun.GliderBlindGun(), toolTip = "Places your gliders on the locked player's head." },

                new ButtonInfo { buttonText = "Shoot Hoverboards (RG?)", method = () => Fun.ShootHoverboards(), toolTip = "Shoots hoverboards forward from your right hand." },
                new ButtonInfo { buttonText = "Hoverboard Gun (RG)", method = () => Fun.HoverboardGun(), toolTip = "Spawns a hoverboard at the selected gun position." },
                new ButtonInfo { buttonText = "Orbit Hoverboards", method = () => Fun.OrbitHoverboards(), toolTip = "Spawns hoverboards orbiting around you." },
                new ButtonInfo { buttonText = "Hoverboard Minigun (RT)", method = () => Overpowered.HoverboardMinigun(), toolTip = "Drops hoverboards from your hands." },

                new ButtonInfo { buttonText = "Get Bracelet", method = () => Fun.Get_Bracelet(true, false), toolTip = "Enables the bracelet on your right hand." },
                new ButtonInfo { buttonText = "Get L Bracelet", method = () => Fun.Get_Bracelet(true, true), toolTip = "Enables the bracelet on your left hand." },
                new ButtonInfo { buttonText = "Unlock Lemming", method = () => Fun.UnlockLemming(), toolTip = "Unlocks the Lemming cosmetic." },
                new ButtonInfo { buttonText = "RGB Monke", method = () => Fun.RGBMonke(), toolTip = "Makes your monkey RGB colors." },
                new ButtonInfo { buttonText = "Draw Mod (RG)", method = () => Fun.Draw(), disableMethod = () => Fun.StopDraw(), toolTip = "Hold right grip to draw dots in the air. Left grip also draws while active. Right primary changes color." },
                new ButtonInfo { buttonText = "Orbit Balls", method = () => Fun.OrbitBalls(), disableMethod = () => Fun.OrbitBallsStop(), toolTip = "Glowing orbs orbit around you in your menu theme color." },
                new ButtonInfo { buttonText = "Size Changer", overlapText = "Size Changer: Normal", method = () => Fun.SizeChanger(), isTogglable = false, toolTip = "Cycles your size: Tiny, Small, Normal, Big, Giant." },
                new ButtonInfo { buttonText = "Spam Bracelet", method = () => Overpowered.SpamBracelet(), toolTip = "Rapidly toggles your bracelet on and off." },
                new ButtonInfo { buttonText = "Ghost Monkey (RP)", method = () => Fun.GhostMonkey(), disableMethod = () => Fun.GhostMonkeyReset(), toolTip = "Hold right primary to turn half-transparent white while still walking around, release to come back." },
                new ButtonInfo { buttonText = "Invisible Monkey (RP)", method = () => Fun.InvisibleMonkey(), disableMethod = () => Fun.InvisibleMonkeyReset(), toolTip = "Hold right primary to go almost fully see-through with white hands while still walking around, release to come back." },
                new ButtonInfo { buttonText = "Fling Rope Gun (RG)", method = () => Fun.FlingRopeGun(), toolTip = "Point at a rope and hold the trigger to fling it around." },
                new ButtonInfo { buttonText = "Snowball Fling Gun (GR)", method = () => Fun.SnowballFlingGun(), toolTip = "Lock a player who's offering their hand (take-my-hand) to fling them into the air." },

                new ButtonInfo { buttonText = "Grab Beach Ball (RG)", method = Fun.GrabBeachBall, isTogglable = true, toolTip = "Grab the beach ball." },
                new ButtonInfo { buttonText = "Beach Ball Gun (RG)", method = Fun.BeachBallGun, isTogglable = true, toolTip = "Move the beach ball with the gun." },

                new ButtonInfo { buttonText = "Break Mod Checkers", method = () => Fun.BreakModCheckers(), toolTip = "Breaks all the mod checkers." },
            },

            new ButtonInfo [] {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},

               
                new ButtonInfo { buttonText = "Get FP (SS)", method = () => Overpowered.GetFP(), isTogglable = false, toolTip = "Adds the Finger Painter badge to your wardrobe." },
                new ButtonInfo { buttonText = "Guardian Gun (RG)", method = () => Overpowered.GuardianGun(), toolTip = "In Guardian mode, point at a player to make them the guardian." },
                new ButtonInfo { buttonText = "Unguardian Gun (RG)", method = () => Overpowered.UnguardianGun(), toolTip = "In Guardian mode, point at the guardian to remove them." },
                new ButtonInfo { buttonText = "Flick Tag Gun (RG)", method = () => Overpowered.FlickTagGun(), toolTip = "Aim at a player and pull the trigger to flick your hand out and tag them." },
                new ButtonInfo { buttonText = "Stump Kick All", method = () => Overpowered.STumpkickall(), toolTip = "Kicks everyone out of the group" },
                new ButtonInfo { buttonText = "Grab Fling Gun (RG)", method = () => Overpowered.GrabFlingGun(), toolTip = "Flings the locked player with a grab" },
                new ButtonInfo { buttonText = "Fling Gun (RG) (NW)", method = () => Overpowered.FlingGun(), toolTip = "Point at a player to fling them straight up into the air." },
                new ButtonInfo { buttonText = "Barrel Fling Gun (RG)", method = () => Overpowered.BarrelFlingGun(), toolTip = "Lock onto a player to blast them up with a burst of projectiles and fling them." },
                new ButtonInfo { buttonText = "Grab Fling All (RT)", method = () => Overpowered.GrabFlingAll(), toolTip = "Flings all players with a grab" },
                new ButtonInfo { buttonText = "Hoverboard Minigun (RT)", method = () => Overpowered.HoverboardMinigun(), toolTip = "Drops hoverboards rapidly" },
                new ButtonInfo { buttonText = "Watersplash (RG)", method = () => Overpowered.Watersplash(), toolTip = "Creates a water splash effect" },
                new ButtonInfo { buttonText = "Watergun (NW)", method = () => Overpowered.Watergun(), toolTip = "Shoots water at the locked player" },
                new ButtonInfo { buttonText = "Elevator Kick Gun (RG)", method = () => Overpowered.ElevatorKickGun(), toolTip = "Kicks the locked player to the ghost reactor elevator" },
                new ButtonInfo { buttonText = "Elevator Kick All", method = () => Overpowered.ElevatorKickAll(), toolTip = "Kicks all players to the ghost reactor elevator" },
                new ButtonInfo { buttonText = "Lag All", method =() => Overpowered.LagAll(), isTogglable = false, toolTip = "Lags everyone"},
                new ButtonInfo { buttonText = "Lag On Touch", method = () => Overpowered.LagOnTouch(), toolTip = "Lags players on touch" }

            },

            new ButtonInfo [] {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},

                new ButtonInfo { buttonText = "Metal spam (RT)", method =() => Sound.MetalSpam(), toolTip = "Spams the metal sound" },
                new ButtonInfo { buttonText = "Crystal spam (RT)", method =() => Sound.HugeCrystalSpam(), toolTip = "Spams the crystal sound" },
                new ButtonInfo { buttonText = "AK47 spam (RT)", method =() => Sound.AK47Spam(), toolTip = "Spams the AK47 sound" },
                new ButtonInfo { buttonText = "Random spam (RT)", method =() => Sound.RandomSpam(), toolTip = "Spams Random sounds" },
                new ButtonInfo { buttonText = "Jman spam (RT)", method =() => Sound.JmanSpam(), toolTip = "Spams the Jman screaming sound" },
            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Settings", method =() => currentCategory = 1, isTogglable = false, toolTip = "Returns to the main settings page for the menu."},

                new ButtonInfo { buttonText = "Change Gun Type", overlapText = "Change Gun Type [Normal]", method =() => Mods.Settings.Gun.ChangeGunType(), isTogglable = false, toolTip = "Changes the look of the gun trail: Normal, Electric or Wiggly."},
                new ButtonInfo { buttonText = "Gun Color", overlapText = "Gun Color: Theme", method =() => Mods.Settings.GunColor.Cycle(), isTogglable = false, toolTip = "Changes the gun pointer color. Normal: green while holding trigger, red when not."},
                new ButtonInfo { buttonText = "Gun Size", overlapText = "Gun Size: Normal", method =() => Mods.Settings.GunSize.Cycle(), isTogglable = false, toolTip = "Changes the size of the gun pointer and trail."},
                new ButtonInfo { buttonText = "Gun Lock", overlapText = "Gun Lock (On)", method =() => Mods.Settings.Gun.ToggleGunLock(), isTogglable = false, toolTip = "On: guns lock onto players you aim at. Off: free aim - point anywhere."},
            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Settings", method =() => currentCategory = 1, isTogglable = false, toolTip = "Returns to the main settings page for the menu."},

                new ButtonInfo { buttonText = "Open Sound", overlapText = "Open Sound [Splash]", method =() => currentCategory = 13, isTogglable = false, toolTip = "Picks the sound that plays when the menu opens."},
                new ButtonInfo { buttonText = "Click Sound", overlapText = "Click Sound [None]", method =() => currentCategory = 14, isTogglable = false, toolTip = "Picks the sound that plays when you press a button."},
            },

            Mods.Settings.Audio.BuildSoundButtons(true),
            Mods.Settings.Audio.BuildSoundButtons(false),

            Mods.Console.Console.BuildButtons(),

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Settings", method =() => currentCategory = 1, isTogglable = false, toolTip = "Returns to the main settings page for the menu."},

                new ButtonInfo { buttonText = "Hide My Crown", enableMethod =() => Classes.AdminTags.hideOwnCrown = true, disableMethod =() => Classes.AdminTags.hideOwnCrown = false, enabled = Classes.AdminTags.hideOwnCrown, toolTip = "Hides the console crown above your own head, only from your own view."},

                new ButtonInfo { buttonText = "Crown Color", overlapText = "Crown Color [Yellow]", method =() => Classes.AdminTags.CycleCrownColor(), isTogglable = false, toolTip = "Changes the color of your console crown. Everyone using the menu sees it."},
            },

            Mods.Soundboard.BuildButtons(),

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},

                new ButtonInfo { buttonText = "Snowball Gun", method =() => Mods.Projectiles.SnowballGun(), toolTip = "Hold grip to aim, pull the trigger to shoot snowballs where you point."},
                new ButtonInfo { buttonText = "Include Hand Velocity", toolTip = "On: snowballs also carry your hand's swing velocity when firing."},
                new ButtonInfo { buttonText = "Snowball Spam", method =() => Mods.Projectiles.SnowballSpam(), toolTip = "Hold grip (either hand) to spam snowballs. PC: left click for right hand."},
            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},

                new ButtonInfo { buttonText = "Become Master", method =() => Master.BecomeMaster(), isTogglable = false, toolTip = "Makes you the room host, so host-only mods and kicking work."},
                new ButtonInfo { buttonText = "Kick Everyone (M)", method =() => Master.KickEveryone(), isTogglable = false, toolTip = "Kicks every other player from the room. Requires being host."},
                new ButtonInfo { buttonText = "Guardian All (M)", method =() => Overpowered.GuardianAll(), isTogglable = false, toolTip = "You need to be master client to use this but it makes everyone guardian even outside of guardian lobbies."},
            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},

                new ButtonInfo { buttonText = "Kick Gun (P)", method =() => Overpowered.KickGun(), toolTip = "Point at a player to kick them from the room."},
                new ButtonInfo { buttonText = "Kick All (P)", method =() => Master.KickEveryone(), isTogglable = false, toolTip = "Kicks every other player from the room. Become host first."},

            },

            Mods.ClientUsers.BuildButtons(),

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Users", method =() => currentCategory = 21, isTogglable = false, toolTip = "Back to the user list."},
            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},
                new ButtonInfo { buttonText = "Tracers", method =() => Mods.Visuals.CasualTracers(), disableMethod =() => Mods.Visuals.HideTracers(), toolTip = "Draws a line from your hand to every player."},
                new ButtonInfo { buttonText = "Player ESP", method =() => Mods.Visuals.PlayerESP(), disableMethod =() => Mods.Visuals.HideESP(), toolTip = "Draws a box around every player, visible through walls."},
                new ButtonInfo { buttonText = "ESP Menu Color", toolTip = "Colors the ESP boxes with your menu theme instead of each player's color."},
                new ButtonInfo { buttonText = "Thin Tracers", toolTip = "Makes the tracer lines thinner."},
                new ButtonInfo { buttonText = "Follow Menu Theme", toolTip = "Colors tracers with your menu theme instead of each player's color."},
                new ButtonInfo { buttonText = "Transparent Theme", toolTip = "Makes the tracer lines semi-transparent."},
                new ButtonInfo { buttonText = "Scale With Player", toolTip = "Scales the tracer thickness with your size."},
            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},
            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},
                new ButtonInfo { buttonText = "PC Button Click", method =() => VRRigMods.PCButtonClick(), toolTip = "On PC, hold left click to move your right hand to where you point."},
                new ButtonInfo { buttonText = "Stare At Closest", method =() => VRRigMods.StareAtClosestPlayer(), toolTip = "Makes your head stare at the closest player."},
                new ButtonInfo { buttonText = "Fix Head", method =() => VRRigMods.FixHead(), isTogglable = false, toolTip = "Resets your head tracking if it got stuck."},
                new ButtonInfo { buttonText = "Grab Rig (RG)", method = () => Fun.GrabRig(), toolTip = "Moves your rig to your right hand while holding grip." },
                new ButtonInfo { buttonText = "Move Rig Gun (RG)", method = () => Fun.MoveRigGun(), toolTip = "Moves your rig to the gun pointer." },
                new ButtonInfo { buttonText = "Spectate Gun (RG)", method = () => Fun.SpectateGun(), toolTip = "Spectates the player targeted by the gun." },
                new ButtonInfo { buttonText = "Copy Movement Gun (RG)", method = () => Fun.CopyMovementGun(), toolTip = "Copies the locked player's moves." },
                new ButtonInfo { buttonText = "Copy Player", method =() => Mods.Players.CopyMovement(), disableMethod =() => Mods.Players.StopCopy(), toolTip = "Copies the player you picked in Players. Turn off to stop."},
            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},
            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Players", method =() => currentCategory = 26, isTogglable = false, toolTip = "Back to the player list."},
            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},
                new ButtonInfo { buttonText = "Morning", method =() => Mods.Weather.Morning(), isTogglable = false, toolTip = "Sets the time of day to morning."},
                new ButtonInfo { buttonText = "Day", method =() => Mods.Weather.Day(), isTogglable = false, toolTip = "Sets the time of day to day."},
                new ButtonInfo { buttonText = "Evening", method =() => Mods.Weather.Evening(), isTogglable = false, toolTip = "Sets the time of day to evening."},
                new ButtonInfo { buttonText = "Night", method =() => Mods.Weather.Night(), isTogglable = false, toolTip = "Sets the time of day to night."},
            },

            new ButtonInfo[] {
                new ButtonInfo { buttonText = "Return to Settings", method =() => currentCategory = 1, isTogglable = false, toolTip = "Returns to the main settings page for the menu."},
                new ButtonInfo { buttonText = "Room Activity", enableMethod =() => Notifications.NotifiLib.RoomActivity = true, disableMethod =() => Notifications.NotifiLib.RoomActivity = false, enabled = Notifications.NotifiLib.RoomActivity, toolTip = "Notifications when players join or leave the room."},
                new ButtonInfo { buttonText = "Mod Activity", enableMethod =() => Notifications.NotifiLib.ModActivity = true, disableMethod =() => Notifications.NotifiLib.ModActivity = false, enabled = Notifications.NotifiLib.ModActivity, toolTip = "Notifications for what mods do (tags, kicks, etc.)."},
                new ButtonInfo { buttonText = "Notification Color", overlapText = "Notification Color: Purple", method =() => Mods.Settings.Notifs.CycleColor(), isTogglable = false, toolTip = "Changes the accent color of the notifications."},
            }
        };
    }
}
