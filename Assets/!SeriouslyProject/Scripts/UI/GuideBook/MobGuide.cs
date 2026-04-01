using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using EchoRift.EchoRiftSaveLoadSystem;
using System;

namespace EchoRift.UI
{
    [Serializable]
    public class BestiaryData
    {
        public List<string> mobNames = new List<string>();
    }

    public class MobGuide : MonoBehaviour
    {
        [SerializeField] private List<Mob> mobs = new List<Mob>();
        [SerializeField] private List<MobUI> mobsUI = new List<MobUI>();
        [SerializeField] private GameObject mobPrefab;
        [SerializeField] private Image mobImage;
        [SerializeField] private TextMeshProUGUI mobLongDescription;

        private void Awake()
        {
            LoadBestiary();
        }

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

            if (mobs == null || mobs.Count == 0) return;
            if (mobPrefab == null) return;

            foreach (Mob mob in mobs)
            {
                if (mob == null) continue; // guard against null entries
                GameObject newMob = Instantiate(mobPrefab, transform);
                MobUI mobUI = newMob.GetComponent<MobUI>();

                if (mobUI == null) continue;

                if (mobUI.nickname != null)
                    mobUI.nickname.text = mob.nickname;
                if (mobUI.shortDescription != null)
                    mobUI.shortDescription.text = mob.shortDescription;

                if (mobUI.button != null)
                {
                    mobUI.button.onClick.RemoveAllListeners();
                    mobUI.button.onClick.AddListener(() => ShowMob(mob));
                }

                mobsUI.Add(mobUI);
            }
        }

        private void MobGuideIsEmpy()
        {
            if (mobs.Count == 0)
            {
                if (mobImage != null)
                {
                    Color c = mobImage.color;
                    c.a = 0f;
                    mobImage.color = c;
                }
                if (mobLongDescription != null)
                    mobLongDescription.text = "";
            }
            else
            {
                if (mobImage != null)
                {
                    Color c = mobImage.color;
                    c.a = 1f;
                    mobImage.color = c;
                }
            }
        }

        public void AddMob(Mob mob)
        {
            if (mob == null) return;
            if (mobs.Any(m => m != null && m.name == mob.name)) return;

            mobs.Add(mob);
            UpdateMobsGrid();
            SaveBestiary();
        }

        private void ShowMob(Mob mob)
        {
            if (mobs.Count > 0)
            {
                if (mobLongDescription != null)
                    mobLongDescription.text = mob.longDescription;
                if (mobImage != null)
                    mobImage.sprite = mob.sprite;
            }
        }

        private void LoadBestiary()
        {
            var data = EchoRiftSaveLoadSystem.SaveLoadSystem.Load<BestiaryData>("BestiaryData", SaveFileNames.GAME_DIRECTORY);
            if (data != null && data.mobNames != null)
            {
                mobs.Clear();

                var allMobs = Resources.LoadAll<Mob>("Mobs");
                foreach (var name in data.mobNames)
                {
                    var mob = allMobs.FirstOrDefault(x => x.name == name);
                    if (mob != null && !mobs.Any(x => x != null && x.name == name))
                    {
                        mobs.Add(mob);
                    }
                    else
                    {
                        Debug.LogWarning($"[MobGuide] Could not find Mob with name: {name}");
                    }
                }
                UpdateMobsGrid();
            }
        }

        private void SaveBestiary()
        {
            var data = new BestiaryData();
            data.mobNames = mobs.Where(m => m != null).Select(m => m.name).ToList();
            EchoRiftSaveLoadSystem.SaveLoadSystem.Save("BestiaryData", data, SaveFileNames.GAME_DIRECTORY);
        }
    }
}
