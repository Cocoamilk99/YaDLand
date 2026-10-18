
using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
    [Header("Key Settings")]
    [SerializeField] private string requiredKeyID = "RedKey";

    [Header("Door Destination")]
    [SerializeField] private Transform destination;

    [Header("Options")]
    [SerializeField] private bool changeRotation = true;

    [Header("Door Lock")]
    [SerializeField] private GameObject doorLock;

    [Header("Key Icon")]
    [SerializeField] private Sprite keyIcon;

    public Sprite GetKeyIcon()
    {
        return keyIcon;
    }

    private bool isUsed = false;

    public bool IsUsed()
    {
        return isUsed;
    }
    public void TryUse(PlayerInventory inventory)
    {
        if (inventory == null)
            return;

        // ตรวจสอบว่าผู้เล่นมีกุญแจหรือไม่
        if (!inventory.HasItem(requiredKeyID))
        {
            Debug.Log("You need a key to open this door!");
            return;
        }

        if (destination == null)
        {
            Debug.LogWarning("Door destination is not assigned!", this);
            return;
        }
        isUsed = true;
        // ย้ายประตูไปยังตำแหน่งใหม่
        transform.position = destination.position;

        if (changeRotation)
        {
            transform.rotation = destination.rotation;
        }

        // ลบตัวล็อคประตู
        if (doorLock != null)
        {
            Destroy(doorLock);
        }

        Debug.Log("Door opened!");


    }
    public string GetRequiredKeyID()
    {
        return requiredKeyID;
    }
    public Sprite GetRequiredKeyIcon()
    {
        ItemPickup[] items = FindObjectsByType<ItemPickup>(FindObjectsInactive.Exclude);


        foreach (ItemPickup item in items)
        {
            if (item != null &&
                item.GetItemID() == requiredKeyID)
            {
                return item.GetItemIcon();
            }
        }

        return null;
    }
    public Sprite GetRequiredKeyIcon(PlayerInventory inventory)
    {
        if (inventory == null)
            return null;

        return inventory.GetItemIcon(requiredKeyID);
    }



}
