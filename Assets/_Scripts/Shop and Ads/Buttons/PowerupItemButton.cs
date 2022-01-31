using TMPro;
using UnityEngine;

namespace Gismo.Core.Shop.UI
{
    public class PowerupItemButton : MonoBehaviour
    {
        private PowerUpItem item;
        private StyleShopController shopController;

        [SerializeField] private TextMeshProUGUI nameText;

        [SerializeField] private TextMeshProUGUI costText;

        [SerializeField] private TextMeshProUGUI countText;

        [SerializeField] private UnityEngine.UI.Button buyButton;

        [SerializeField] private UnityEngine.UI.Image image;

        public void Initalize(PowerUpItem item, StyleShopController controller)
        {
            this.item = item;
            shopController = controller;

            image.sprite = item.icon;

            Draw();
        }

        public void Draw()
        {
            nameText.text = item.name;

            if (item.powerUpsGotten == 0)
            {
                countText.text = "";
            }
            else
            {
                countText.text = $"x{item.powerUpsGotten}";
            }

            costText.text = $"{item.cost} Style {Tools.StaticFunctions.Pluraise(item.cost, "Point")}";
            buyButton.interactable = true;
            if (item.limitedDateAvalibility)
            {
                buyButton.gameObject.SetActive(item.dateRange.CurrentDayWithinRange());
            }
            else
            {
                buyButton.gameObject.SetActive(true);
            }
        }


        public void TryBuyItem()
        {
            if (shopController.BuyItem(item))
            {
                item.powerUpsGotten++;
                Audio.AudioManager.Play("Purchase");

                shopController.SaveUnlockedItems();
                Draw();
            }
        }
    }
}
