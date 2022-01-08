using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gismo.Tools
{
    public class DeviceSafeAreaManager : MonoBehaviour
    {
        RectTransform rectTransform;
        Rect safeArea;
        Vector2 minAnchor;
        Vector2 maxAnchor;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            safeArea = Screen.safeArea;

            minAnchor = safeArea.position;
            maxAnchor = minAnchor + safeArea.size;

            minAnchor /= new Vector2(Screen.width, Screen.height);
            maxAnchor /= new Vector2(Screen.width, Screen.height);

            rectTransform.anchorMin = minAnchor;
            rectTransform.anchorMax = maxAnchor;
        }
    }
}