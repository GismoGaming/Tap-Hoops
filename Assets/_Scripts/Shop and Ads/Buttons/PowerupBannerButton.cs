using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace Gismo.Core.Shop
{
    public class PowerupBannerButton : MonoBehaviour
    {
        [SerializeField] private PowerUpItem item;

        [SerializeField] private Image iconImage;

        public void SetupItemButton(PowerUpItem newItem)
        {
            item = newItem;

            item.activated = false;
            iconImage.sprite = item.icon;
        }

        public void UsePowerup()
        {
            item.activated = true;

            item.OnActivated?.Invoke();

            item.powerUpsGotten--;

            GameController.instance.AddUsedPowerup(item);

            GetComponent<Button>().interactable = false;
        }
    }
}
