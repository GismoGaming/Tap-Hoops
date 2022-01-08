using System;
using UnityEngine;

namespace Gismo.Tools
{
    public class ScreenBoundaries
    {
        private static ScreenBounds bounds;

        private static bool boundsCalculated;

        private static Camera mainCamera;

        private const int REFWIDTH = 1440;
        private const int REFHEIGHT = 2960;

        public static void Initalize(bool forceRecalculate = false)
        {
            if(boundsCalculated && !forceRecalculate)
            {
                return;
            }

            mainCamera = Camera.main;

            boundsCalculated = true;

            bounds.bottomLeft = mainCamera.ViewportToWorldPoint(Vector2.zero);
            bounds.bottomRight = mainCamera.ViewportToWorldPoint(Vector2.right);

            bounds.topleft = mainCamera.ViewportToWorldPoint(Vector2.up);
            bounds.topRight = mainCamera.ViewportToWorldPoint(Vector2.one);
        }

        public static float GetMaxDimension()
        {
            if (Screen.width > Screen.height)
                return Screen.width;
            else
                return Screen.height;
        }

        public static float GetMinDimension()
        {
            if (Screen.width > Screen.height)
                return Screen.height;
            else
                return Screen.width;
        }

        public static float GetMaxDimensionFromRefrence()
        {
            if (Screen.width > Screen.height)
                return Screen.width / REFWIDTH;
            else
                return Screen.height / REFHEIGHT;
        }

        public static float GetMinDimensionFromRefrence()
        {
            if (Screen.width > Screen.height)
                return Screen.height / REFHEIGHT;
            else
                return Screen.width / REFHEIGHT;
        }

        public static float GetWorldHeight()
        {
            return bounds.topleft.y - bounds.bottomLeft.y;
        }

        public static float GetWorldWidth()
        {
            return bounds.topRight.x - bounds.topleft.x;
        }

        public static bool ObjectInBounds(Transform t)
        {
            Initalize();
            Vector2 point = mainCamera.WorldToViewportPoint(t.position);

            return StaticFunctions.WithinBounds(point.x, Vector2.up) && StaticFunctions.WithinBounds(point.y, Vector2.up);
        }

        public static Vector2 GetPoint(Vector2 point, bool ignoreOutOfBounds = false)
        {
            Initalize();

            if(!ignoreOutOfBounds)
            {
                if (point.x > 1f || point.x < 0f || point.y > 1f || point.y < 0f)
                {
                    Debug.LogError($"Point {point} is out of bounds\nPoint must be between (0f,1f) on the x and y axis. If you want allow out of bounds, set \"ignoreOutOfBounds\" to false");
                }
            }

            return mainCamera.ViewportToWorldPoint(point);
        }

        public static Vector3 GetPoint(Vector2 point, float z, bool ignoreOutOfBounds = false)
        {
            return new Vector3(GetPoint(point, ignoreOutOfBounds).x, GetPoint(point,ignoreOutOfBounds).y, z);
        }

        public static Vector2 GetPoint(PresetPoints point)
        {
            Initalize();

            return point switch
            {
                PresetPoints.BottomLeft => bounds.bottomLeft,
                PresetPoints.BottomRight => bounds.bottomRight,
                PresetPoints.TopRight => bounds.topRight,
                _ => bounds.topleft,
            };
        }

        public static Vector3 GetPoint(PresetPoints point, float z)
        {
            return new Vector3(GetPoint(point).x, GetPoint(point).y, z);
        }
    }

    public enum PresetPoints
    {
        BottomLeft,
        BottomRight,
        TopRight,
        TopLeft
    }

    struct ScreenBounds
    {
        public Vector2 bottomLeft;
        public Vector2 bottomRight;

        public Vector2 topRight;
        public Vector2 topleft;
    }
}
