using UnityEngine;

public class HitReset : MonoBehaviour
{
    public Transform player;            // PlayerArmature
    public Transform playerSpawnPoint;  // PlayerSpawnPoint
    public GameObject hunter;           // IYA02

    public void ResetAfterHit()
    {
        // 1) ล้างกระเป๋า
        PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();
        if (inventory != null)
            inventory.ClearInventory();

        // 2) ไอเทมทุกชิ้นกลับมาให้เก็บใหม่ (รวมตัวที่ถูกซ่อนอยู่)
        ItemPickup[] items = FindObjectsByType<ItemPickup>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (ItemPickup item in items)
        {
            item.gameObject.SetActive(true);

            HunterTrigger ht = item.GetComponent<HunterTrigger>();
            if (ht != null)
                ht.ResetTrigger();
        }
    }
}