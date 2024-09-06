using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static bool inventoryActivated = false;

    [SerializeField] private GameObject inventoryBase;
    [SerializeField] private GameObject slotParent; // 슬롯이 배치된 부모 Transform (슬롯들의 부모 오브젝트)

    private Slot[] slots; // 각 슬롯을 관리하는 Slot 배열

    void Awake()
    {
        slots = slotParent.GetComponentsInChildren<Slot>();
        //FreshSlot();
    }

    private void Update()
    {
        TryOpenInventory();
    }

    private void TryOpenInventory()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            inventoryActivated = !inventoryActivated;

            if (inventoryActivated)
                OpenInventory();
            else
                CloseInventory();
        }
    }

    private void OpenInventory()
    {
        inventoryBase.SetActive(true);
    }
    private void CloseInventory()
    {
        inventoryBase.SetActive(false);
    }

    public void AcquireItem(Item _item, int _count = 1)
    {
        if(Item.ItemType.Equipment != _item.itemType)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item.itemName != null && slots[i].item.itemName == _item.itemName)
                {
                    slots[i].SetSlotCount(_count);
                    return;
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item.itemName == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }
    }
}