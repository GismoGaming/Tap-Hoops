using TMPro;
using UnityEngine;

namespace Gismo.Core.Shop.UI
{
    public class HoopFXItemButton : MonoBehaviour
    {
        private HoopFXItem item;
        private StyleShopController shopController;

        [SerializeField] private TextMeshProUGUI nameText;

        [SerializeField] private TextMeshProUGUI costText;

        [SerializeField] private UnityEngine.UI.Button buyButton;
        [SerializeField] private UnityEngine.UI.Button equipButton;

        public void Initalize(HoopFXItem item, StyleShopController controller)
        {
            this.item = item;
            shopController = controller;

            Draw();
        }

        public void DeEquip()
        {
            item.selected = false;
            Draw();
        }

        public void Draw()
        {
            nameText.text = item.name;

            costText.text = $"{item.cost} Style {Tools.StaticFunctions.Pluraise(item.cost, "Point")}";

            buyButton.gameObject.SetActive(!item.unlocked);
            equipButton.gameObject.SetActive(item.unlocked);

            equipButton.interactable = !item.selected;
        }


        public void TryBuyItem()
        {
            if (shopController.BuyItem(item))
            {
                Audio.AudioManager.Play("Purchase");
                Draw();
            }
        }

        public void EquipItem(bool doAudio)
        {
            shopController.EquipItem(item);
            Draw();
            if (doAudio)
                Audio.AudioManager.Play("Equip");
        }
    }
}