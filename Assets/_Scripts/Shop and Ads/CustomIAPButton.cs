using TMPro;
using UnityEngine.UI;
using System.Collections;

using UnityEngine;
using UnityEngine.Purchasing;

namespace Gismo.IAP
{
    public class CustomIAPButton : MonoBehaviour
    {
        public string item;

        [Header("Custom Settings")]

        [SerializeField] private TextMeshProUGUI tText;
        [SerializeField] private TextMeshProUGUI pText;
        void Start()
        {
            Button button = GetComponent<Button>();

            if (button)
            {
                button.onClick.AddListener(PurchaseProduct);
            }

            if (string.IsNullOrEmpty(item))
            {
                Debug.LogError("Item is blank");
            }
            InitText();
        }

        public void InitText()
        {
            GetComponent<Button>().interactable = false;

            foreach (TextMeshProUGUI t in GetComponentsInChildren<TextMeshProUGUI>())
                t.text = "";

            StartCoroutine(WaitTillReady());
        }

        IEnumerator WaitTillReady()
        {
            while (!CodelessIAPStoreListener.initializationComplete)
                yield return new WaitForEndOfFrame();

            Product product = CodelessIAPStoreListener.Instance.GetProduct(item);
            Button b = GetComponent<Button>();
            b.interactable = true;
            if (product != null)
            {
                if (product.availableToPurchase)
                {
                    b.interactable = true;
                    if (product.definition.type == ProductType.NonConsumable)
                    {
                        if(product.hasReceipt)
                            b.interactable = false;
                    }
                }
                else
                {
                    b.interactable = false;
                }

                tText.text = product.metadata.localizedTitle.Replace("(Tap Hoops)", "").Trim(); ;
                pText.text = product.metadata.localizedPriceString;
            }
        }

        void PurchaseProduct()
        {
            CodelessIAPStoreListener.Instance.InitiatePurchase(item);
        }
    }
}