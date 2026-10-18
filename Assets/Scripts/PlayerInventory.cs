
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory UI")]
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Image iconPrefab;

    [Header("Pickup Prompt")]
    [SerializeField] private TMP_Text pickupPrompt;

    private readonly HashSet<string> collectedItems =
        new HashSet<string>();

    private readonly HashSet<ItemPickup> nearbyItems =
        new HashSet<ItemPickup>();

    private readonly Dictionary<string, Sprite> collectedItemIcons =
    new Dictionary<string, Sprite>();

    [Header("Door Interaction")]
    [SerializeField] private float doorInteractionRange = 2.5f;

    [SerializeField] private Image keyPromptIcon;
    private void Start()
    {
        UpdatePrompt();
    }



    private void Update()
    {
        UpdatePrompt();

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            DoorTeleport door = GetNearestDoor();

            if (door != null)
            {
                door.TryUse(this);
            }
            else
            {
                TryPickupNearestItem();
            }
        }
    }



    public void RegisterNearby(ItemPickup item)
    {
        if (item != null)
            nearbyItems.Add(item);

        UpdatePrompt();
    }

    public void UnregisterNearby(ItemPickup item)
    {
        nearbyItems.Remove(item);
        UpdatePrompt();
    }

    private ItemPickup GetNearestItem()
    {
        ItemPickup nearestItem = null;
        float nearestDistance = float.MaxValue;

        foreach (ItemPickup item in nearbyItems)
        {
            if (item == null)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                item.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestItem = item;
            }
        }

        return nearestItem;
    }



    private void UpdatePrompt()
    {
        if (pickupPrompt == null)
            return;

        DoorTeleport door = GetNearestDoor();

        // อยู่ใกล้ประตู
        if (door != null && !door.IsUsed())
        {
            pickupPrompt.gameObject.SetActive(true);
            pickupPrompt.text = "กด E เพื่อเปิดด้วย "
                + door.GetRequiredKeyID();

            if (keyPromptIcon != null)
            {
                Sprite icon = door.GetKeyIcon();
                keyPromptIcon.sprite = icon;
                keyPromptIcon.enabled = icon != null;
            }

            return;
        }

        ItemPickup nearestItem = GetNearestItem();

        // อยู่ใกล้ไอเทมที่เก็บได้
        if (nearestItem != null)
        {
            pickupPrompt.gameObject.SetActive(true);
            pickupPrompt.text = "กด E เพื่อเก็บ";

            if (keyPromptIcon != null)
            {
                Sprite icon = nearestItem.GetItemIcon();
                keyPromptIcon.sprite = icon;
                keyPromptIcon.enabled = icon != null;
            }

            return;
        }

        // ไม่อยู่ใกล้ประตูหรือไอเทม
        pickupPrompt.gameObject.SetActive(false);

        if (keyPromptIcon != null)
            keyPromptIcon.enabled = false;
    }



    private void TryPickupNearestItem()
    {
        ItemPickup nearestItem = GetNearestItem();

        if (nearestItem != null)
            nearestItem.Pickup(this);
    }

    public bool ReceiveItem(string itemID, Sprite icon)
    {
        if (string.IsNullOrWhiteSpace(itemID))
            return false;

        if (!collectedItems.Add(itemID))
        {
            Debug.Log("มีไอเทมนี้อยู่แล้ว: " + itemID);
            return false;
        }
        collectedItemIcons[itemID] = icon;

        if (icon != null &&
            iconContainer != null &&
            iconPrefab != null)
        {
            Image newIcon = Instantiate(
                iconPrefab,
                iconContainer
            );

            newIcon.sprite = icon;
            newIcon.preserveAspect = true;
            newIcon.enabled = true;
        }
        else
        {
            Debug.LogWarning(
                "ยังไม่ได้ตั้งค่าไอคอนหรือ UI ของ Inventory"
            );
        }

        Debug.Log("เก็บไอเทมสำเร็จ: " + itemID);
        return true;
    }

    public bool HasItem(string itemID)
    {
        return collectedItems.Contains(itemID);
    }


    private DoorTeleport GetNearestDoor()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            doorInteractionRange
        );

        DoorTeleport nearestDoor = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            DoorTeleport door =
                hit.GetComponentInParent<DoorTeleport>();

            if (door == null)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                door.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestDoor = door;
            }
        }

        return nearestDoor;
    }
    public Sprite GetItemIcon(string itemID)
    {
        if (collectedItemIcons.TryGetValue(itemID, out Sprite icon))
            return icon;

        return null;
    }
}
