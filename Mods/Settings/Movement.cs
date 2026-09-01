using static Parrot.client.Menu.Main;

namespace Parrot.client.Mods.Settings
{
    public class Movement
    {
        private static readonly string[] Levels = { "Low", "Medium", "High", "Really High", "Extreme" };

        public static int flySpeedIndex = 1;
        public static float flySpeed = 15f;

        public static int predictionIndex = 1;
        public static float predictionAmount = 0.35f;

        public static int tagAuraIndex = 1;

        public static int pullSpeedIndex = 1;

        public static int antiReportIndex = 1;

        public static void ChangeFlySpeed()
        {
            float[] values = new float[] { 10f, 15f, 25f, 40f, 60f };

            flySpeedIndex = (flySpeedIndex + 1) % Levels.Length;
            flySpeed = values[flySpeedIndex];

            var button = GetIndex("Change Fly Speed");
            if (button != null)
                button.overlapText = $"Change Fly Speed [{Levels[flySpeedIndex]}]";
        }

        public static void ChangePullSpeed()
        {
            float[] values = new float[] { 15f, 30f, 50f, 75f, 110f };

            pullSpeedIndex = (pullSpeedIndex + 1) % Levels.Length;
            Parrot.client.Mods.Movement.pullSpeed = values[pullSpeedIndex];

            var button = GetIndex("Wall Walk Speed");
            if (button != null)
                button.overlapText = $"Wall Walk Speed [{Levels[pullSpeedIndex]}]";
        }

        public static void ChangeAntiReportSensitivity()
        {
            float[] values = new float[] { 0.06f, 0.12f, 0.2f, 0.35f, 0.6f };

            antiReportIndex = (antiReportIndex + 1) % Levels.Length;
            Parrot.client.Mods.Safety.TriggerRange = values[antiReportIndex];

            var button = GetIndex("Anti Report Sensitivity");
            if (button != null)
                button.overlapText = $"Anti Report Sensitivity [{Levels[antiReportIndex]}]";
        }

        public static void ChangeTagAuraDistance()
        {
            float[] values = new float[] { 1.5f, 2.5f, 4f, 6f, 9f };

            tagAuraIndex = (tagAuraIndex + 1) % Levels.Length;
            Parrot.client.Mods.Advantage.tagAuraDistance = values[tagAuraIndex];

            var button = GetIndex("Tag Aura Distance");
            if (button != null)
                button.overlapText = $"Tag Aura Distance [{Levels[tagAuraIndex]}]";
        }
    }
}
