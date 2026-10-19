using UnityEngine;
using UnityEngine.SceneManagement; // จำเป็นต้องใช้สำหรับการโหลด Scene

public class DoorTeleport : MonoBehaviour
{
    [Header("Key Settings")]
    [SerializeField] private string requiredKeyID = "RedKey";

    [Header("Door Destination (สำหรับประตูเทเลพอร์ตปกติ)")]
    [SerializeField] private Transform destination;
    [SerializeField] private bool changeRotation = true;

    [Header("End Game Settings (ติ๊กถูกถ้าเป็นประตูจบเกม)")]
    [SerializeField] private bool isEndGameDoor = false;
    [SerializeField] private string endingSceneName = "EndingScene"; // ชื่อ Scene ฉากจบ

    [Header("Door Lock")]
    [SerializeField] private GameObject doorLock;

    [Header("Key Icon")]
    [SerializeField] private Sprite keyIcon;

    private bool isUsed = false;

    public Sprite GetKeyIcon() { return keyIcon; }
    public bool IsUsed() { return isUsed; }

    public void TryUse(PlayerInventory inventory)
    {
        if (inventory == null) return;

        // ตรวจสอบว่าผู้เล่นมีกุญแจหรือไม่
        if (!inventory.HasItem(requiredKeyID))
        {
            Debug.Log("You need a key to open this door!");
            return;
        }

        isUsed = true;
        Debug.Log("Door opened!");

        // ลบแม่กุญแจ
        if (doorLock != null)
        {
            Destroy(doorLock);
        }

        // --- ถ้าเป็นประตูจบเกม ให้โหลดไปหน้าคัดซีน ---
        if (isEndGameDoor)
        {
            SceneManager.LoadScene(endingSceneName);
            return;
        }

        // --- ถ้าเป็นประตูเทเลพอร์ตปกติ ---
        if (destination == null)
        {
            Debug.LogWarning("Door destination is not assigned!", this);
            return;
        }

        // ข้อควรระวัง: โค้ดเดิมคุณย้ายตำแหน่งประตู (transform.position) 
        // หากต้องการย้ายผู้เล่น ต้องอ้างอิง inventory.transform.position แทน
        transform.position = destination.position; 
        if (changeRotation)
        {
            transform.rotation = destination.rotation;
        }
    }

    public string GetRequiredKeyID() { return requiredKeyID; }

    public Sprite GetRequiredKeyIcon()
    {
        ItemPickup[] items = FindObjectsByType<ItemPickup>(FindObjectsInactive.Exclude);
        foreach (ItemPickup item in items)
        {
            if (item != null && item.GetItemID() == requiredKeyID)
            {
                return item.GetItemIcon();
            }
        }
        return null;
    }

    public Sprite GetRequiredKeyIcon(PlayerInventory inventory)
    {
        if (inventory == null) return null;
        return inventory.GetItemIcon(requiredKeyID);
    }
}