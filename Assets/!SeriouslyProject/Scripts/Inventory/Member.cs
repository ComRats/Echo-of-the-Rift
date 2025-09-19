using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Member : MonoBehaviour
{
    public int health;
    public int xp;
    public int level;
    public int mana;
    public int maxHealth;
    public int maxMana;


    public Slider healthSlider;
    public Slider manaSlider;
    public Image icon;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI manaText;

    public Member(int health, int xp, int level, int mana, int maxHealth, int maxMana, Slider healthSlider, Slider manaSlider, Image icon,
    TextMeshProUGUI healthText, TextMeshProUGUI levelText, TextMeshProUGUI manaText)
    {
        this.health = health;
        this.xp = xp;
        this.level = level;
        this.mana = mana;
        this.maxHealth = maxHealth;
        this.maxMana = maxMana;

        this.healthSlider = healthSlider;
        this.manaSlider = manaSlider;
        this.icon = icon;
        this.healthText = healthText;
        this.levelText = levelText;
        this.manaText = manaText;

        UpdateUI();
    }

    public void UpdateUI()
    {
        healthSlider.value = health;
        manaSlider.value = mana;

        healthSlider.maxValue = maxHealth;
        manaSlider.maxValue = maxMana;

        healthText.text = $"{health}/{maxHealth}";
        manaText.text = $"{mana}/{maxMana}";
        levelText.text = $"lvl {level}";
    }
    
    private void OnValidate()
    {
        UpdateUI();
    }
}
