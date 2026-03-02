using FightSystem.Data;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

namespace EchoRift
{
    public class Player : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public CameraSettings cameraSettings;
        public Movement movement;
        public ChangeNameDialogueActor dialogActor;
        public DialogueSystemEvents dialogueEvents;
        public PlayerSaver playerSaver;

        public static FightResult Result = FightResult.None;
        public Vector3 startPosition;

        private void Awake()
        {
            if (startPosition == Vector3.zero)
            {
                startPosition = transform.position;
            }
        }

        public void SetListenerToEvents(UnityAction<Transform> _onStartConversation, UnityAction<Transform> _onEndConversation)
        {
            dialogueEvents.conversationEvents.onConversationStart.AddListener(_onStartConversation);
            dialogueEvents.conversationEvents.onConversationEnd.AddListener(_onEndConversation);
        }

        public void Hide()
        {
            //Debug.LogWarning("Hide player");

            spriteRenderer.enabled = false;
            movement.enabled = false;
            cameraSettings.enabled = false;
        }

        public void Show()
        {
            //Debug.LogWarning("Show player");

            spriteRenderer.enabled = true;
            movement.enabled = true;
            cameraSettings.enabled = true;
        }

        [System.Serializable]
        public class PlayerSaver : EntityStats
        {
            [SerializeField] private string spritePath;

            public void LoadFrom(CharacterData data)
            {
                Name = data.Name;
                Description = data.Description;

                spritePath = data.Sprite != null ? $"CharacterData/{data.Sprite.name}" : null;

                Damage = data.Damage;
                Priority = data.Priority;
                MaxMana = data.MaxMana;
                Mana = data.Mana;
                MaxHealth = data.MaxHealth;
                Health = data.Health;
                Heal = data.Heal;
                Armor = data.Armor;
                Lucky = data.Lucky;
                CreteDamage = data.CreteDamage;

                Level = data.Level;
                CurrentXP = data.CurrentXP;
                MaxXP = data.MaxXP;
                XpReward = data.XpReward;

                DamagePerLevel = data.DamagePerLevel;
                MaxHealthPerLevel = data.MaxHealthPerLevel;
                HealPerLevel = data.HealPerLevel;
                ArmorPerLevel = data.ArmorPerLevel;
                MaxManaPerLevel = data.MaxManaPerLevel;
                XpRewardPerLevel = data.XpRewardPerLevel;

                if (data.Sprite != null)
                    spritePath = $"CharacterData/{data.Sprite.name}";
            }

            public Sprite GetSprite()
            {
                return spritePath != null ? Resources.Load<Sprite>(spritePath) : null;
            }
        }

    }
}