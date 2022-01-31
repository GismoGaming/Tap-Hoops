using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gismo.Core.Shop
{
    public class StyleShopController : MonoBehaviour
    {
        private const string TRAILITEMS = "Trail Item Unlocks";
        private const string BALLITEMS = "Ball Item Unlocks";
        private const string BGITEMS = "BG Item Unlocks";
        private const string HOOPFXITEMS = "Hoop FX Item Unlocks";
        private const string PURCHASEDPOWERUPS = "Purchased Powerups";

        private const string SELECTEDTRAILITEM = "Selected Trail Item";
        private const string SELECTEDBALLITEM = "Selected Ball Item";
        private const string SELECTEDBGITEM = "Selected BG Item";
        private const string SELECTEDHOOPFXITEM = "Selected Hoop FX Item";
        public const string NOADS = "No Ads";

        private const string NOITEMINDICATOR = "NO_ITEM";

        [SerializeField] private List<TrailItem> trailItems;
        [SerializeField] private List<BallItem> ballItems;
        [SerializeField] private List<BGItem> bgItems;
        [SerializeField] private List<HoopFXItem> hoopFXItems;
        public List<PowerUpItem> powerupItems;

        [SerializeField] private List<string> iapIDs;

        [Header("UI")]
        [SerializeField] private Transform trailItemDrawArea;
        [SerializeField] private Transform ballItemDrawArea;
        [SerializeField] private Transform bgItemDrawArea;
        [SerializeField] private Transform hoopFXItemDrawArea;
        [SerializeField] private Transform stylePointDrawArea;
        [SerializeField] private Transform powerupItemDrawArea;

        [SerializeField] private GameObject trailItemButtonPrefab;
        [SerializeField] private GameObject ballItemButtonPrefab;
        [SerializeField] private GameObject bgItemButtonPrefab;
        [SerializeField] private GameObject hoopFXItemPrefab;
        [SerializeField] private GameObject stylePointButtonPrefab;
        [SerializeField] private GameObject powerupItemPrefab;

        private int selectedTrailID;
        private int selectedBallID;
        private int selectedBGID;
        private int selectedHoopFXID;

        [SerializeField] private UnityEngine.UI.Image DEVbutton;

        [Header("Modifer")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Effects.BackgroundFiller bgFiller; 
        [SerializeField] private Rigidbody2D ballRB; 
        [SerializeField] private Transform hoopFXParent;
        [SerializeField] private Transform trailPrefabParent;

        private UnityEngine.UI.Button noAdsButton;
        private void Awake()
        {
            GetUnlockedItems();

            if (PlayerPrefs.GetInt(NOADS, 0) == 0)
            {
                Statics.noAds = false;
            }
            else
            {
                Statics.noAds = true;
            }

            DEVbutton.color = Statics.noAds == true ? Color.green : Color.red;

            DrawShopUI();
        }

        public void ToggleAdPurchase()
        {
            Statics.noAds = !Statics.noAds;
            int v = 0;

            if (Statics.noAds)
                v = 1;
            PlayerPrefs.SetInt(NOADS, v);
            DEVbutton.color = Statics.noAds == true ? Color.green : Color.red;
            Debug.Log(Statics.noAds);

            noAdsButton.interactable = !Statics.noAds;
        }

        public void PurchaseNoAds()
        {
            Statics.noAds = true;
            DEVbutton.color = Statics.noAds == true ? Color.green : Color.red;
            PlayerPrefs.SetInt(NOADS, 1);

            if (!Statics.noAds)
               noAdsButton.interactable = false;
        }

        void DrawShopUI()
        {
            foreach(TrailItem item in trailItems)
            {
                GameObject newButton = Instantiate(trailItemButtonPrefab, trailItemDrawArea);

                newButton.GetComponent<UI.TrailItemButton>().Initalize(item,this);

                item.buttonUI = newButton;
            }

            foreach (BallItem item in ballItems)
            {
                GameObject newButton = Instantiate(ballItemButtonPrefab, ballItemDrawArea);

                newButton.GetComponent<UI.BallItemButton>().Initalize(item,this);
                item.buttonUI = newButton;
            }

            foreach (BGItem item in bgItems)
            {
                GameObject newButton = Instantiate(bgItemButtonPrefab, bgItemDrawArea);

                newButton.GetComponent<UI.BGItemButton>().Initalize(item, this);
                item.buttonUI = newButton;
            }

            foreach(HoopFXItem item in hoopFXItems)
            {
                GameObject newButton = Instantiate(hoopFXItemPrefab, hoopFXItemDrawArea);

                newButton.GetComponent<UI.HoopFXItemButton>().Initalize(item, this);
                item.buttonUI = newButton;
            }

            foreach(string s in iapIDs)
            {
                IAP.CustomIAPButton newButton = Instantiate(stylePointButtonPrefab, stylePointDrawArea).GetComponent<IAP.CustomIAPButton>();

                newButton.item = s;

                if(s == "29.99adfree")
                {
                    noAdsButton = newButton.GetComponent<UnityEngine.UI.Button>();
                }
            }

            foreach(PowerUpItem item in powerupItems)
            {
                GameObject newButton = Instantiate(powerupItemPrefab, powerupItemDrawArea);
                newButton.GetComponent<UI.PowerupItemButton>().Initalize(item, this);
            }
        }

        public void LoadSelected()
        {
            selectedTrailID = PlayerPrefs.GetInt(SELECTEDTRAILITEM,-4);

            if (GetTrailItem(selectedTrailID) != null)
                GetTrailItem(selectedTrailID).buttonUI.GetComponent<UI.TrailItemButton>().EquipItem(false);
            else
                GetTrailItem(-1).buttonUI.GetComponent<UI.TrailItemButton>().EquipItem(false);

            selectedBallID = PlayerPrefs.GetInt(SELECTEDBALLITEM, -4);

            if (GetBallItem(selectedBallID) != null)
                GetBallItem(selectedBallID).buttonUI.GetComponent<UI.BallItemButton>().EquipItem(false);
            else
                GetBallItem(-1).buttonUI.GetComponent<UI.BallItemButton>().EquipItem(false);

            selectedBGID = PlayerPrefs.GetInt(SELECTEDBGITEM, -4);

            if (GetBGItem(selectedBGID) != null)
                GetBGItem(selectedBGID).buttonUI.GetComponent<UI.BGItemButton>().EquipItem(false);
            else
                GetBGItem(-1).buttonUI.GetComponent<UI.BGItemButton>().EquipItem(false);

            selectedHoopFXID = PlayerPrefs.GetInt(SELECTEDHOOPFXITEM, -4);
            if (GetHoopFXItem(selectedHoopFXID) != null)
                GetHoopFXItem(selectedHoopFXID).buttonUI.GetComponent<UI.HoopFXItemButton>().EquipItem(false);
            else
                GetHoopFXItem(-1).buttonUI.GetComponent<UI.HoopFXItemButton>().EquipItem(false);
        }

        [ContextMenu("Clear Style Points")]
        public void ClearStylePoints()
        {
            Statics.stylePoints = 0;
            PlayerPrefs.SetInt("Style Points", Statics.stylePoints);
        }

        public void UpdateStylePoints()
        {
            PlayerPrefs.SetInt("Style Points", Statics.stylePoints);
        }

        void GetUnlockedItems()
        {
            string storedTrailItem = PlayerPrefs.GetString(TRAILITEMS, NOITEMINDICATOR);

            if (storedTrailItem != NOITEMINDICATOR)
            {
                foreach (string id in storedTrailItem.Split('.'))
                {
                    foreach(TrailItem item in trailItems)
                    {
                        if (int.TryParse(id, out int gid))
                        {
                            if (item.id == gid)
                            {
                                item.unlocked = true;
                                break;
                            }
                        }
                    }
                }
            }

            string storedBallItem = PlayerPrefs.GetString(BALLITEMS, NOITEMINDICATOR);

            if (storedBallItem != NOITEMINDICATOR)
            {
                foreach (string id in storedBallItem.Split('.'))
                {
                    foreach (BallItem item in ballItems)
                    {
                        if (int.TryParse(id, out int gid))
                        {
                            if (item.id == gid)
                            {
                                item.unlocked = true;
                                break;
                            }
                        }
                    }
                }
            }

            string storedBGItem = PlayerPrefs.GetString(BGITEMS, NOITEMINDICATOR);

            if (storedBGItem != NOITEMINDICATOR)
            {
                foreach (string id in storedBGItem.Split('.'))
                {
                    foreach (BGItem item in bgItems)
                    {
                        if (int.TryParse(id, out int gid))
                        {
                            if (item.id == gid)
                            {
                                item.unlocked = true;
                                break;
                            }
                        }
                    }
                }
            }

            string storedHoopFXItem = PlayerPrefs.GetString(HOOPFXITEMS, NOITEMINDICATOR);

            if (storedHoopFXItem != NOITEMINDICATOR)
            {
                foreach (string id in storedHoopFXItem.Split('.'))
                {
                    foreach (HoopFXItem item in hoopFXItems)
                    {
                        if (int.TryParse(id, out int gid))
                        {
                            if (item.id == gid)
                            {
                                item.unlocked = true;
                                break;
                            }
                        }
                    }
                }
            }

            string purchasedPowerups = PlayerPrefs.GetString(PURCHASEDPOWERUPS, NOITEMINDICATOR);
            if (purchasedPowerups != NOITEMINDICATOR)
            {
                Dictionary<int, int> strings = new Dictionary<int, int>();
                foreach (string itemPowerups in purchasedPowerups.Split('.'))
                {
                    if (int.TryParse(itemPowerups.Split('|')[0], out int id) && int.TryParse(itemPowerups.Split('|')[1], out int numberGotten))
                    {
                        strings.Add(id, numberGotten);
                    }
                }
                foreach (PowerUpItem item in powerupItems)
                {
                    item.powerUpsGotten = 0;
                    if(strings.ContainsKey(item.id))
                    {
                        item.powerUpsGotten = strings[item.id];
                    }
                }
            }
        }

        [ContextMenu("Clear Unlocks")]
        public void ClearUnlocks()
        {
            foreach (TrailItem i in trailItems)
            {
                if(i.id != -1)
                    i.unlocked = false;
            }

            foreach (BallItem i in ballItems)
            {
                if (i.id != -1)
                    i.unlocked = false;
            }

            foreach (BGItem i in bgItems)
            {
                if (i.id != -1)
                    i.unlocked = false;
            }

            foreach (HoopFXItem i in hoopFXItems)
            {
                if (i.id != -1)
                    i.unlocked = false;
            }

            foreach (PowerUpItem i in powerupItems)
            {
                if (i.id != -1)
                    i.powerUpsGotten = 0;
            }

            PlayerPrefs.DeleteAll();

            SaveUnlockedItems();
            GetUnlockedItems();
        }


        [ContextMenu("Force Save")]
        public void SaveUnlockedItems()
        {         
            string trailSave = "";
            foreach(TrailItem item in trailItems)
            {
                if(item.unlocked)
                {
                    trailSave += item.id.ToString() + ".";
                }
            }

            trailSave.Trim();

            PlayerPrefs.SetString(TRAILITEMS, trailSave);

            string ballSave = "";
            foreach (BallItem item in ballItems)
            {
                if (item.unlocked)
                {
                    ballSave += item.id.ToString() + ".";
                }
            }

            ballSave.Trim();
            PlayerPrefs.SetString(BALLITEMS, ballSave);

            string bgSave = "";
            foreach (BGItem item in bgItems)
            {
                if (item.unlocked)
                {
                    bgSave += item.id.ToString() + ".";
                }
            }

            bgSave.Trim();
            PlayerPrefs.SetString(BGITEMS, bgSave);

            string hoopFXSave = "";
            foreach (HoopFXItem item in hoopFXItems)
            {
                if (item.unlocked)
                {
                    hoopFXSave += item.id.ToString() + ".";
                }
            }

            hoopFXSave.Trim();
            PlayerPrefs.SetString(HOOPFXITEMS, hoopFXSave);

            string powerupSave = "";

            foreach (PowerUpItem item in powerupItems)
            {
                powerupSave += $"{item.id}|{item.powerUpsGotten}.";
            }
            PlayerPrefs.SetString(PURCHASEDPOWERUPS, powerupSave);
            PlayerPrefs.Save();
        }

        public void EquipItem(TrailItem item)
        {
            if (!item.unlocked)
                return;

            if (GetTrailItem(selectedTrailID) != null)
            {
                GetTrailItem(selectedTrailID).buttonUI.GetComponent<UI.TrailItemButton>().DeEquip();
            }

            selectedTrailID = item.id;
            item.selected = true;

            if (trailPrefabParent.childCount != 0)
            {
                Destroy(trailPrefabParent.GetChild(0).gameObject);
            }

            Instantiate(item.trailPrefab, trailPrefabParent);

            PlayerPrefs.SetInt(SELECTEDTRAILITEM, selectedTrailID);
        }

        public void EquipItem(BallItem item)
        {
            if (!item.unlocked)
                return;

            if (GetBallItem(selectedBallID) != null)
            {
                GetBallItem(selectedBallID).buttonUI.GetComponent<UI.BallItemButton>().DeEquip();
            }

            selectedBallID = item.id;
            item.selected = true;

            spriteRenderer.sprite = item.ballSprite;

            ballRB.mass = item.physics.weight;
            ballRB.angularDrag = item.physics.angularDrag;
            ballRB.drag = item.physics.linearDrag;

            ballRB.sharedMaterial = item.physics.physicsMaterial;

            PlayerPrefs.SetInt(SELECTEDBALLITEM, selectedBallID);
        }

        public void EquipItem(BGItem item)
        {
            if (!item.unlocked)
                return;

            if (GetBGItem(selectedBGID) != null)
            {
                GetBGItem(selectedBGID).buttonUI.GetComponent<UI.BGItemButton>().DeEquip();
            }

            selectedBGID = item.id;
            item.selected = true;

            bgFiller.UpdateBG(item);

            PlayerPrefs.SetInt(SELECTEDBGITEM, selectedBGID);
        }

        public void EquipItem(HoopFXItem item)
        {
            if (!item.unlocked)
                return;

            if (GetHoopFXItem(selectedHoopFXID) != null)
            {
                GetHoopFXItem(selectedHoopFXID).buttonUI.GetComponent<UI.HoopFXItemButton>().DeEquip();
            }

            selectedHoopFXID = item.id;
            item.selected = true;

            if(hoopFXParent.childCount != 0)
            {
                Destroy(hoopFXParent.GetChild(0).gameObject);
            }

            Instantiate(item.hoopFXPrefab, hoopFXParent);

            PlayerPrefs.SetInt(SELECTEDHOOPFXITEM, selectedHoopFXID);
        }


        public bool BuyItem(ShopItem item)
        {
            if (CanBuyItem(item))
            {
                item.unlocked = true;
                Statics.stylePoints -= item.cost;

                SaveUnlockedItems();

                GameController.instance.UpdateScoreHud();
                return true;
            }
            return false;
        }

        private bool CanBuyItem(ShopItem item)
        {
            if (item.unlocked && item.GetType() != typeof(PowerUpItem))
                return false;
            if (item.cost > Statics.stylePoints)
                return false;

            return true;
        }

        private TrailItem GetTrailItem(int id)
        {
            foreach(TrailItem i in trailItems)
            {
                if (i.id == id)
                    return i;
            }
            return null;
        }

        private BallItem GetBallItem(int id)
        {
            foreach (BallItem i in ballItems)
            {
                if (i.id == id)
                    return i;
            }
            return null;
        }

        private BGItem GetBGItem(int id)
        {
            foreach (BGItem i in bgItems)
            {
                if (i.id == id)
                    return i;
            }
            return null;
        }

        private HoopFXItem GetHoopFXItem(int id)
        {
            foreach (HoopFXItem i in hoopFXItems)
            {
                if (i.id == id)
                    return i;
            }
            return null;
        }
    }

    [Serializable]
    public class ShopItem
    {
        public int id;
        public Sprite icon;

        public bool unlocked;

        public bool selected;

        [HideInInspector]
        public GameObject buttonUI;

        public string name;
        public int cost;

        [Header("Avaliblity")]
        public bool limitedDateAvalibility;
        public Date.DateRange dateRange;
    }

    [Serializable]
    public class TrailItem : ShopItem
    {
        [Header("")]
        public GameObject trailPrefab;
    }

    [Serializable]
    public class BallItem : ShopItem
    {
        [Header("")]
        public Sprite ballSprite;
        public BallPhysicsDetails physics;
    }

    [Serializable]
    public struct BallPhysicsDetails
    {
        public float weight;
        public float linearDrag;
        public float angularDrag;

        public PhysicsMaterial2D physicsMaterial;
    }

    [Serializable]
    public class HoopFXItem: ShopItem
    {
        [Header("")]
        public GameObject hoopFXPrefab;
    }

    [Serializable]
    public class BGItem : ShopItem
    {
        [Header("")]
        public Sprite bgSprite;
        public Color bgColor;
        public SpriteDrawMode drawMode;

        [Header("Color Change")]
        public bool doColorChange;
        public Color bgColorSecondary;
        public float colorChangeTime;
    }

    [Serializable]
    public class PowerUpItem : ShopItem
    {
        public int powerUpsGotten;

        public bool activated;

        public UnityEvent OnActivated;
        public UnityEvent OnDeActivated;
    }
}