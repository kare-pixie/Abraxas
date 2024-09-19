using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private bool inventoryActivated = false;
    public List<HaveItem> items; // �÷��̾ ������ �����۵��� �����ϴ� ����Ʈ

    public PotionSlot potionSlot1; // �ϴܹ� ���� ����
    public PotionSlot potionSlot2; // �ϴܹ� ���� ����

    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject slotParent; // ������ ��ġ�� �θ� Transform (���Ե��� �θ� ������Ʈ)

    private Slot[] slots; // �� ������ �����ϴ� Slot �迭

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

        // �� ������Ʈ�� �θ� �������� ���� ������ �ڽ����� �̵����� ���� ���� ǥ��
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
        Debug.Log("������ ���� �� �ֽ��ϴ�.");
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
    /// �κ��丮�� ������ ���� ��ħ�ϴ� �޼���
    /// </summary>
    public void FreshSlot()
    {
        if (items == null) return;

        int i = 0;
        // �����۰� ������ �ִ� ��ŭ ���Կ� �������� �Ҵ�
        for (; i < items.Count && i < slots.Length; i++)
        {
            slots[i].itemCount = items[i].itemCount;
            slots[i].Item = items[i].item;
        }

        // ���� ������ ������ �� �������� ����
        for (; i < slots.Length; i++)
        {
            slots[i].itemCount = 0;
            slots[i].Item = null;
        }
    }
}