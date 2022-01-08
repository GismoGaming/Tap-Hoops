using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gismo.Effects
{
    public class BackgroundFiller : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void UpdateBG(Core.Shop.BGItem item)
        {
            spriteRenderer.sprite = item.bgSprite;
            spriteRenderer.drawMode = item.drawMode;
            spriteRenderer.color = item.bgColor;
            spriteRenderer.size = Vector2.one;
            transform.localScale = Vector2.one;

            LeanTween.cancel(spriteRenderer.gameObject);

            if (item.drawMode == SpriteDrawMode.Simple)
            {
                transform.localScale = new Vector2(Tools.ScreenBoundaries.GetWorldWidth(), Tools.ScreenBoundaries.GetWorldHeight()) * item.bgSprite.pixelsPerUnit/4;
            }
            else
            {
                spriteRenderer.size = new Vector2(Tools.ScreenBoundaries.GetWorldWidth(), Tools.ScreenBoundaries.GetWorldHeight());
            }

            if (item.doColorChange)
            {
                gameObject.LeanColor(item.bgColorSecondary, item.colorChangeTime).setLoopPingPong();
            }
        }
    }
}
