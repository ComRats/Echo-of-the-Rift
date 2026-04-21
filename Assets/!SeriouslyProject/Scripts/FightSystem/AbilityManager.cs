using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using DG.Tweening;

public class AbilityManager : MonoBehaviour
{
    [Title("UI References")]
    [SerializeField] private Transform physicalAbilitiesContainer;
    [SerializeField] private Transform magicAbilitiesContainer;
    [SerializeField] private Transform defenseAbilitiesContainer;
    [SerializeField] private Transform supportAbilitiesContainer;
    [SerializeField] private RectTransform abilitiesPanel;

    [Title("button Prefab")]
    [SerializeField] private GameObject abilityButtonPrefab;

    [Title("Animation Settings")]
    [SerializeField] private float slideInDuration = 0.5f;
    [SerializeField] private float slideInDistance = 240f;
    [SerializeField] private Ease slideInEase = Ease.OutBack;

    [Title("References")]
    [SerializeField] private FightManager fightManager;

    private Dictionary<Button, BattleAbility> buttonAbilityMap = new Dictionary<Button, BattleAbility>();
    private FightSystem.Character.Character currentCharacter;
    private Vector2 originalPanelPosition;

    public void SetupAbilitiesForCharacter(FightSystem.Character.Character character)
    {
        currentCharacter = character;
        ClearAllButtons();

        if (character.AbilitySet == null)
        {
            return;
        }

        // Берём разблокированные индексы из PlayerSaver
        List<int> extraIndices = GlobalLoader.Instance?.playerInstance?.playerSaver?.unlockedAbilityIndices;
        var activeAbilities = character.AbilitySet.GetActiveAbilities(character.Level, extraIndices);

        foreach (var charAbility in activeAbilities)
        {
            CreateAbilityButton(charAbility);
        }

        AnimatePanelSlideIn();
    }

    private void AnimatePanelSlideIn()
    {
        if (abilitiesPanel == null) return;

        if (originalPanelPosition == Vector2.zero)
        {
            originalPanelPosition = abilitiesPanel.anchoredPosition;
        }

        Vector2 targetPosition = originalPanelPosition + Vector2.up * slideInDistance;

        abilitiesPanel.DOAnchorPos(targetPosition, slideInDuration)
            .SetEase(slideInEase);
    }

    private void CreateAbilityButton(CharacterAbility charAbility)
    {
        Transform parentContainer = GetContainerForType(charAbility.abilityType);
        
        if (parentContainer == null || abilityButtonPrefab == null)
        {
            return;
        }

        GameObject buttonObj = Instantiate(abilityButtonPrefab, parentContainer);
        Button button = buttonObj.GetComponent<Button>();
        
        if (button == null)
        {
            Destroy(buttonObj);
            return;
        }

        AbilityButton abilityButton = buttonObj.GetComponent<AbilityButton>();
        if (abilityButton != null)
        {
            abilityButton.Setup(charAbility.ability, currentCharacter, charAbility.abilityIcon);
        }
        else
        {
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = charAbility.ability.AbilityName;
            }

            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null && charAbility.abilityIcon != null)
            {
                buttonImage.sprite = charAbility.abilityIcon;
            }
        }

        var ability = charAbility.ability;
        button.onClick.AddListener(() => OnAbilityButtonClicked(ability));

        buttonAbilityMap[button] = ability;
    }

    private Transform GetContainerForType(AbilityType type)
    {
        switch (type)
        {
            case AbilityType.Physical:
                return physicalAbilitiesContainer;
            case AbilityType.Magic:
                return magicAbilitiesContainer;
            case AbilityType.Defense:
                return defenseAbilitiesContainer;
            case AbilityType.Support:
                return supportAbilitiesContainer;
            default:
                return physicalAbilitiesContainer;
        }
    }

    private void OnAbilityButtonClicked(BattleAbility ability)
    {
        if (currentCharacter == null || !ability.CanUse(currentCharacter))
        {
            return;
        }

        var actionButtons = GetComponent<ActionButtons>();
        if (actionButtons != null)
        {
            actionButtons.SetPendingAbility(ability, currentCharacter);
        }
    }

    private void ClearAllButtons()
    {
        buttonAbilityMap.Clear();
        
        ClearContainer(physicalAbilitiesContainer);
        ClearContainer(magicAbilitiesContainer);
        ClearContainer(defenseAbilitiesContainer);
        ClearContainer(supportAbilitiesContainer);
    }

    private void ClearContainer(Transform container)
    {
        if (container == null) return;

        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }

    public void RefreshAbilities()
    {
        if (currentCharacter != null)
        {
            SetupAbilitiesForCharacter(currentCharacter);
        }
    }
}
