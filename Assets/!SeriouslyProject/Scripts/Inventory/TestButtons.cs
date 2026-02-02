using UnityEngine;

public class TestButtons : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            inventoryManager.AddItem("pig", 1);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            inventoryManager.AddItem("axe", 1);
        }
    }
}