using UnityEngine;

public class TestButtons : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    
    public ItemData porosenok;
    public ItemData axe;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            inventoryManager.AddItem(porosenok, 1);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            inventoryManager.AddItem(axe, 1);
        }
    }
}