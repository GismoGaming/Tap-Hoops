using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gismo.Ads
{
    public class AdToStylePointButton : MonoBehaviour
    {
        [SerializeField] private AdManager adManager;

        [SerializeField] private Transform root;
        [SerializeField] private GameObject popupPrefab;

        int stylePointsGet;

        [SerializeField] private Vector2Int stylePointGets;

        [SerializeField] TMPro.TextMeshProUGUI pointText;

        [SerializeField] private UnityEngine.UI.Button button;

        private bool canShowAds;

        private void Awake()
        {
            stylePointsGet = Random.Range(stylePointGets.x, stylePointGets.y);
            canShowAds = true;
            UpdateText();
        }

        void UpdateText()
        {
            pointText.text = $"+ {stylePointsGet}";
        }

        public void ShowStylePointAd()
        {
            if (!canShowAds)
                return;
            switch(adManager.ShowAd(AdType.Rewarded))
            {
                case 0:
                    ShowPopup($"Thanks for watching the ad! Enjoy your {stylePointsGet} style points!");

                    Core.Statics.stylePoints += stylePointsGet;
                    break;
                case -1:
                    ShowPopup($"Thank you for purchasing no ads! Enjoy your {stylePointsGet} style point!");

                    Core.Statics.stylePoints += stylePointsGet;
                    break;
                case -5:
                    ShowPopup($"Ads aren't working, have {stylePointsGet / 4} style points for your hassle");

                    Core.Statics.stylePoints += stylePointsGet / 4;
                    break;
            }

            stylePointsGet = Random.Range(stylePointGets.x, stylePointGets.y);
            PlayerPrefs.SetInt("Style Points", Core.Statics.stylePoints);

            button.interactable = false;

            StartCoroutine(WaitToUnlock());

            UpdateText();
        }

        IEnumerator WaitToUnlock()
        {
            canShowAds = false;
            yield return new WaitForSeconds(10f);
            canShowAds = true;

            button.interactable = true;
        }

        void ShowPopup(string text)
        {
            GameObject g = Instantiate(popupPrefab, Vector3.zero, Quaternion.identity, root);
            g.transform.localPosition = Vector3.zero;
            g.transform.localScale = Vector3.zero;

            g.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text;

            g.GetComponent<RectTransform>().sizeDelta *= (1 + Tools.ScreenBoundaries.GetMinDimensionFromRefrence());

            LTSeq seq = LeanTween.sequence();
            seq.append(g.LeanScale(Vector3.one, 2f).setEaseOutBounce());
            seq.append(3f);
            seq.append(g.LeanScale(Vector3.zero, 2f).setEaseOutBounce().setOnComplete(() => Destroy(g)));
        }
    }
}
