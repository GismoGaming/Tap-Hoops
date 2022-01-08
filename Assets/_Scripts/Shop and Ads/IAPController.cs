using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Gismo.IAP
{
    public class IAPController : MonoBehaviour
    {
        [SerializeField] private Tools.VisualDictionary<string, UnityEngine.Events.UnityEvent> lookupDictionary;
        [SerializeField] private Core.Shop.StyleShopController controller; 

        public void OnPurchaseDone(Product product)
        {
            if (lookupDictionary.ItemExists(product.definition.id))
            {
                Debug.Log($"Bought {product.metadata.localizedTitle}");
                lookupDictionary.GetItem(product.definition.id)?.Invoke();
            }
        }

        public void OnPurchaseFail(Product product, PurchaseFailureReason reason)
        {
            Debug.Log($"Failed to buy {product.metadata.localizedTitle} because of reason: {reason}");
        }

        public void AddStylePoints(int amount)
        {
            Core.Statics.stylePoints += amount;

            Core.GameController.instance.UpdateScoreHud();
        }

        public void PurchaseNoAds()
        {
            controller.PurchaseNoAds();
        }
    }
}
