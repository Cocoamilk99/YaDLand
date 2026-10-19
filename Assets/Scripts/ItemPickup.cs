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
        {
            // เรียกผีโผล่ก่อนซ่อนไอเทม
            HunterTrigger ht = GetComponent<HunterTrigger>();
            if (ht != null)
                ht.SpawnHunter();

            // ซ่อนแทนการลบ เพื่อให้รีเซ็ตแล้วกลับมาได้
            inventory.UnregisterNearby(this);
            gameObject.SetActive(false);
        }
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