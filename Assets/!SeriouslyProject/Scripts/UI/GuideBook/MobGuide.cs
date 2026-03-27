using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace EchoRift.UI
{
    public class MobGuide : MonoBehaviour
    {
        [SerializeField] private List<Mob> mobs;
        [SerializeField] private List<MobUI> mobsUI;
        [SerializeField] private GameObject mobPrefab;
        [SerializeField] private Image mobImage;
        [SerializeField] private TextMeshProUGUI mobLongDescription;

        private void Start()
        {
            UpdateMobsGrid();
            if (mobs.Count > 0)
            {
                ShowMob(mobs.First());
            }
        }

        [Button]
        public void UpdateMobsGrid()
        {
            MobGuideIsEmpy();
            CreateMobs();
        }

        private void CreateMobs()
        {
            mobsUI.Clear();

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            if (mobs.Count == 0) return;

            foreach (Mob mob in mobs)
            {
                GameObject newMob = Instantiate(mobPrefab, transform);
                MobUI mobUI = newMob.GetComponent<MobUI>();

                mobUI.nickname.text = mob.nickname;
                mobUI.shortDescription.text = mob.shortDescription;

                mobUI.button.onClick.RemoveAllListeners();
                mobUI.button.onClick.AddListener(() => ShowMob(mob));

                mobsUI.Add(mobUI);
            }
        }

        private void MobGuideIsEmpy()
        {
            if (mobs.Count == 0)
            {
                Color c = mobImage.color;
                c.a = 0f;
                mobImage.color = c;
                mobLongDescription.text = "";
            }
            else
            {
                Color c = mobImage.color;
                c.a = 1f;
                mobImage.color = c;
            }
        }

        public void AddMob(Mob mob)
        {
            if (mob == null || mobs.Contains(mob)) return;
            mobs.Add(mob);
            UpdateMobsGrid();
        }

        private void ShowMob(Mob mob)
        {
            if (mobs.Count > 0)
            {
                mobLongDescription.text = mob.longDescription;
                mobImage.sprite = mob.sprite;
            }
        }
    }

}

