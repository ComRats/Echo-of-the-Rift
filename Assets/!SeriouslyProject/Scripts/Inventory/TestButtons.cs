using UnityEngine;

public class TestButtons : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            inventoryManager.AddItem("axe", 1);
            inventoryManager.AddItem("testAmulet", 1);
            inventoryManager.AddItem("rustChestplate", 1);
            inventoryManager.AddItem("brokeHelmet", 1);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            inventoryManager.AddItem("pig", 1);
            inventoryManager.AddItem("karas", 1);
        }
    }

}