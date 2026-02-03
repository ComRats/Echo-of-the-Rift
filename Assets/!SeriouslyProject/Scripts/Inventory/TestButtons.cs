using UnityEngine;

public class TestButtons : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            inventoryManager.AddItem("axe", 1);
            inventoryManager.AddItem("pig", 1);
            inventoryManager.AddItem("karas", 1);
            inventoryManager.AddItem("testAmulet", 1);
            inventoryManager.AddItem("testArmor", 1);
            inventoryManager.AddItem("testHelmet", 1);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            inventoryManager.AddItem("pig", 1);
            inventoryManager.AddItem("karas", 1);
        }
    }
}