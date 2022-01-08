
namespace Gismo.Core
{
    class Statics
    {
        public static float currentTapCooldown;
        public static float currentForce;

        public static float startingCountdownTime;

        public static bool ballHasTouchedGround;
        public static bool playerAddedForce;

        public static bool developerMode;

        public static bool gameRunning;
        public static int stylePoints;

        public static bool noAds;
    }

    [System.Serializable]
    public class MultiTag
    {
        public string[] tags;

        public bool CompareTag(string tag)
        {
            foreach(string t in tags)
            {
                if (t == tag)
                    return true;
            }
            return false;
        }
    }
}
