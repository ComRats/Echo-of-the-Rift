using UnityEngine;

public class TestButtons : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;

#if UNITY_EDITOR

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            inventoryManager.AddItem("axe", 1);
            inventoryManager.AddItem("testAmulet", 1);
            inventoryManager.AddItem("rustChestplate", 1);
            inventoryManager.AddItem("brokeHelmet", 1);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            inventoryManager.AddItem("pig", 1);
            inventoryManager.AddItem("karas", 1);
        }
    }
#endif
}