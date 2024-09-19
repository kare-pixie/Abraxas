using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private bool inventoryActivated = false;
    public List<HaveItem> items; // 플레이어가 소유한 아이템들을 저장하는 리스트

    public PotionSlot potionSlot1; // 하단바 포션 슬롯
    public PotionSlot potionSlot2; // 하단바 포션 슬롯

    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject slotParent; // 슬롯이 배치된 부모 Transform (슬롯들의 부모 오브젝트)

    private Slot[] slots; // 각 슬롯을 관리하는 Slot 배열

    private void Awake()
    {
        slots = slotParent.GetComponentsInChildren<Slot>();
        FreshSlot();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            TryOpenInventory();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            potionSlot1.UseItem();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            potionSlot2.UseItem();
        }
    }

    public int getItemCount(Item _item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Item != null && slots[i].Item.itemName == _item.itemName)
            {
                return slots[i].itemCount;
            }
        }
        return 0;
    }

    public void TryOpenInventory()
    {
        inventoryActivated = !inventoryActivated;

        if (inventoryActivated)
            OpenInventory();
        else
            CloseInventory();
    }

    private void OpenInventory()
    {
        inventory.SetActive(true);

        // 이 오브젝트를 부모 계층에서 가장 마지막 자식으로 이동시켜 가장 위에 표시
        inventory.transform.SetAsLastSibling();
    }
    private void CloseInventory()
    {
        DragSlot.instance.SetImageAlpha(0f);

        inventory.SetActive(false);
    }

    public void AcquireItem(Item _item, int _count = 1)
    {
        if(Item.ItemType.Equipment != _item.itemType)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Item != null && slots[i].Item.itemName == _item.itemName)
                {
                    slots[i].SetSlotCount(_count);
                    return;
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }
        Debug.Log("슬롯이 가득 차 있습니다.");
    }

    public void UseItem(Item _item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Item != null && slots[i].Item.itemName == _item.itemName)
            {
                slots[i].SetSlotCount(-1);
                return;
            }
        }
    }

    /// <summary>
    /// 인벤토리의 슬롯을 새로 고침하는 메서드
    /// </summary>
    public void FreshSlot()
    {
        if (items == null) return;

        int i = 0;
        // 아이템과 슬롯이 있는 만큼 슬롯에 아이템을 할당
        for (; i < items.Count && i < slots.Length; i++)
        {
            slots[i].itemCount = items[i].itemCount;
            slots[i].Item = items[i].item;
        }

        // 남은 슬롯이 있으면 빈 슬롯으로 설정
        for (; i < slots.Length; i++)
        {
            slots[i].itemCount = 0;
            slots[i].Item = null;
        }
    }
}