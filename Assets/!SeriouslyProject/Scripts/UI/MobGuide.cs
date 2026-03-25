using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EchoRift.UI
{
    public class MobGuide : MonoBehaviour
    {
        [SerializeField] private List<Mob> mobs;
        [SerializeField] private GameObject mobPrefab;
        [SerializeField] private Image mobImage;
        [SerializeField] private TextMeshProUGUI mobLongDescription;

        private Mob firstMob;

        public void UpdateMobs()
        {
            CreateMobs();
            ShowFirstMob(firstMob);
        }

        private void CreateMobs()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            firstMob = mobs.First();

            foreach (Mob mob in mobs)
            {
                Instantiate(mobPrefab);
                MobUI mobUI = mobPrefab.GetComponent<MobUI>();
                mobUI.nickname.text = mob.nickname;
                mobUI.shortDescription.text = mob.shortDescription;
            }
        }

        private void ShowFirstMob(Mob mob)
        {
            mobLongDescription.text = mob.longDescription;
            mobImage.sprite = mob.sprite;
        }
    }

}

