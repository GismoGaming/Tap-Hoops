using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gismo.Tools.UI
{
    class UITabingSystem : MonoBehaviour
    {
        [SerializeField] private List<GameObject> tabs;
        [SerializeField] private List<Button> tabButtons;

        private int activeTab = -1;

        private void Awake()
        {
            foreach(GameObject g in tabs)
            {
                g.SetActive(false);
            }

            foreach(Button b in tabButtons)
            {
                b.interactable = true;
            }

            GoToTab(0);
        }

        public void GoToTab(int id)
        {
            if (activeTab != -1)
            {
                tabs[activeTab].SetActive(false);
                tabButtons[activeTab].interactable = true;
            }

            tabs[id].SetActive(true);
            tabButtons[id].interactable = false;

            activeTab = id;
        }
    }
}
