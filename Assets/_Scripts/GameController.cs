using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Gismo.Core.Shop;

namespace Gismo.Core
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;

        bool countDownClockRunning;

        private float currentCountDownTimeMax;
        private float currentCountDownTime;

        private int basketCount;

        [SerializeField] private BallMovement playerBall;

        [SerializeField] private TextMeshProUGUI countDownTimer;
        [SerializeField] private TextMeshProUGUI basketCountText;
        [SerializeField] private TextMeshProUGUI stylePointCountText;
        [SerializeField] private TextMeshProUGUI stylePointCountTextSub;

        [SerializeField] private Image countDownBar;

        [SerializeField] private GameObject largeUpgradeShopButton;
        [SerializeField] private GameObject shortUpgradeShopButton;
        [SerializeField] private GameObject startText;

        [Header("Style Point Settings")]
        [SerializeField] private float maxForCloseCallPoints;

        [SerializeField] private int hoopsPerStylePoint;

        [SerializeField] private GameObject stylePointPopup;

        [SerializeField] private RectTransform mainCanvas;

        private List<PowerUpItem> usedPowerups;
        enum StylePointDescription { No_Ground, No_Input, Close_Call, Upward_Dunk}

        [System.Serializable]
        public struct StylePointInfoPopup
        {
            public int points;
            public string popupText;
        }

        [SerializeField] private Tools.VisualDictionary<StylePointDescription, StylePointInfoPopup> stylePointLookupTable;

        [Header("Countdown Time")]

        [SerializeField] private Vector2 basketBounds;
        [SerializeField] private Vector2 countDownTimeBounds;

        [SerializeField] private GameObject shopUI;
        [SerializeField] private GameObject mainUI;

        [SerializeField] private int maxBaskets;
        [SerializeField] private float maxBasketsTime;

        private List<StylePointDescription> stylePointsToDisplay;

        [SerializeField] private HoopController hoopController;

        [SerializeField] private Transform powerupBanner;
        [SerializeField] private GameObject powerupBannerItemPrefab;
        [SerializeField] private GameObject powerupBannerRoot;
        [SerializeField] private StyleShopController shopController;

        private void Awake()
        {
            if (!instance)
                instance = this;

            shortUpgradeShopButton.SetActive(false);
            largeUpgradeShopButton.SetActive(true);

            startText.SetActive(true);
        }

        private void Update()
        {
            if(countDownClockRunning)
            {
                if(currentCountDownTime >= 0f)
                {
                    currentCountDownTime -= Time.deltaTime;
                }
                else
                {
                    currentCountDownTime = 0f;

                    if (!Statics.developerMode)
                        OnSessionEnd();

                    Statics.stylePoints += Mathf.CeilToInt(basketCount / hoopsPerStylePoint);
                }
                UpdateMainHud();
            }
        }

        [ContextMenu("End Session")]
        public void OnSessionEnd(bool showAd = true)
        {
            countDownClockRunning = false;

            Statics.gameRunning = false;
            PlayerPrefs.SetInt("Style Points", Statics.stylePoints);
            PlayerPrefs.Save();

            shopUI.SetActive(true);
            mainUI.SetActive(false);

            shortUpgradeShopButton.SetActive(false);
            largeUpgradeShopButton.SetActive(true);

            startText.SetActive(true);

            foreach(PowerUpItem i in usedPowerups)
            {
                i.OnDeActivated?.Invoke();
            }

            if (showAd)
            {
                Ads.AdManager.instance.ShowAd(Ads.AdType.Skippable);
            }
        }

        public void AddUsedPowerup(PowerUpItem i)
        {
            usedPowerups.Add(i);
        }

        void UpdateMainHud()
        {
            countDownBar.fillAmount = currentCountDownTime / currentCountDownTimeMax;

            countDownTimer.text = $"{(int)currentCountDownTime} {Tools.StaticFunctions.Pluraise((int)currentCountDownTime, "Second")} Left";
        }

        public void UpdateScoreHud()
        {
            if (basketCount == 0)
            {
                basketCountText.text = "";
            }
            else
            {
                basketCountText.text = $"{basketCount} {Tools.StaticFunctions.Pluraise(basketCount,"Basket")} Made";
            }

            stylePointCountText.text = $"{Statics.stylePoints}";
            stylePointCountTextSub.text = $"{Statics.stylePoints}";
        }

        public void ResetGame()
        {
            shopUI.SetActive(false);
            mainUI.SetActive(true);
            playerBall.ArrowVis(true);

            hoopController.currentHoopCount = 0;

            currentCountDownTime = Statics.startingCountdownTime;

            currentCountDownTimeMax = currentCountDownTime;

            playerBall.ResetBall();

            Statics.ballHasTouchedGround = false;
            Statics.playerAddedForce = false;

            Statics.gameRunning = true;

            countDownClockRunning = false;
            basketCount = 0;

            usedPowerups = new List<PowerUpItem>();

            foreach (Transform t in powerupBanner)
                Destroy(t.gameObject);

            bool hasPowerups = false;
            foreach(PowerUpItem item in shopController.powerupItems)
            {
                if(item.powerUpsGotten != 0)
                {
                    hasPowerups = true;
                    Instantiate(powerupBannerItemPrefab, powerupBanner).GetComponent<PowerupBannerButton>().SetupItemButton(item);
                }
            }

            powerupBannerRoot.SetActive(hasPowerups);

            UpdateMainHud();
            UpdateScoreHud();

            countDownTimer.text = "Score a basket to start!";
        }

        public void RestartCountDown(float newTime)
        {
            currentCountDownTime = newTime;

            currentCountDownTimeMax = currentCountDownTime;

            countDownClockRunning = true;

            Statics.ballHasTouchedGround = false;
            Statics.playerAddedForce = false;
        }

        public void OnScore(ScoreStat scoreStat)
        {
            if (!Statics.gameRunning)
                return;

            stylePointsToDisplay = new List<StylePointDescription>();

            if (!countDownClockRunning)
            {
                countDownClockRunning = true;
                largeUpgradeShopButton.SetActive(false);
                shortUpgradeShopButton.SetActive(true);
                startText.SetActive(false);

                powerupBannerRoot.SetActive(false);
            }
            basketCount++;

            if(!Statics.ballHasTouchedGround)
            {
                stylePointsToDisplay.Add(StylePointDescription.No_Ground);
            }

            if(!Statics.playerAddedForce)
            {
                stylePointsToDisplay.Add(StylePointDescription.No_Input);
            }

            // Close Call Check
            if (currentCountDownTime < maxForCloseCallPoints)
            {
                stylePointsToDisplay.Add(StylePointDescription.Close_Call);
            }

            switch (scoreStat)
            {
                case ScoreStat.Upwards:
                    stylePointsToDisplay.Add(StylePointDescription.Upward_Dunk);
                    break;
                case ScoreStat.Downwards:
                    break;
            }

            UpdateScoreHud();

            RestartCountDown(GetCountDownTime());

            DoStylePointPopup();
        }

        private float GetCountDownTime()
        {
            if (basketCount >= maxBaskets)
            {
                return maxBasketsTime;
            }
            else
            {
                return Tools.StaticFunctions.ReMap(Mathf.Clamp(basketCount, basketBounds.x, basketBounds.y), basketBounds, countDownTimeBounds);
            } 
        }

        private void DoStylePointPopup()
        {
            StartCoroutine(DoPopup());
        }

        IEnumerator DoPopup()
        {
            foreach (StylePointDescription description in stylePointsToDisplay)
            {
                yield return new WaitForSeconds(Random.Range(.2f, .7f));

                StylePointInfoPopup info = stylePointLookupTable.GetItem(description);

                Statics.stylePoints += info.points;
                UpdateScoreHud();

                ShowPopup(info.popupText, mainCanvas);
            }
        }

        void ShowPopup(string text, Transform parent)
        {
            GameObject g = Instantiate(stylePointPopup, Vector3.zero, Quaternion.identity, parent);

            g.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(Random.Range(-Screen.width / 4f, Screen.width / 4f), Random.Range(-Screen.height / 4f, Screen.height / 4f));
            g.GetComponent<RectTransform>().sizeDelta *= (1 + Tools.ScreenBoundaries.GetMinDimensionFromRefrence());

            g.GetComponentInChildren<TextMeshProUGUI>().text = text;

            g.LeanMoveLocalY(g.transform.localPosition.y + Tools.ScreenBoundaries.GetWorldHeight() / 8f, 3f).setEaseOutBounce();
            g.LeanScale(Vector3.zero, 2f).setEaseInElastic().setOnComplete(() => Destroy(g));
        }
    }

    public enum ScoreStat { Downwards, Upwards}
}
