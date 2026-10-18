
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private string itemID = "RedKey";
    [SerializeField] private Sprite itemIcon;

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory =
            other.GetComponentInParent<PlayerInventory>();

        if (inventory != null)
            inventory.RegisterNearby(this);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerInventory inventory =
            other.GetComponentInParent<PlayerInventory>();

        if (inventory != null)
            inventory.UnregisterNearby(this);
    }

    public void Pickup(PlayerInventory inventory)
    {
        if (inventory == null)
            return;

        if (inventory.ReceiveItem(itemID, itemIcon))
            Destroy(gameObject);
    }
    public string GetItemID()
    {
        return itemID;
    }

    public Sprite GetItemIcon()
    {
        return itemIcon;
    }
}
