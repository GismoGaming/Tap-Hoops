using UnityEngine;
namespace Gismo.Tools
{
    class StaticFunctions
    {
        public static bool WithinBounds(float value, Vector2 bounds)
        {
            return value >= bounds.x && value <= bounds.y;
        }

        public static string Pluraise(int value, string text)
        {
            return value == 1 ? text : text + "s";
        }

        public static float ReMap(float value, float minOriginal, float maxOriginal, float minNew, float maxNew)
        {
            return (((value - minOriginal) * (maxNew - minNew)) / (maxOriginal - minOriginal)) + minNew;
        }

        public static float ReMap(float value, Vector2 originalBounds, Vector2 newBounds)
        {
            return ReMap(value, originalBounds.x, originalBounds.y, newBounds.x, newBounds.y);
        }

        public static bool GetRandomBool(float bounds = 0.5f)
        {
            return Random.value < bounds;
        }
    }
}
