using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static bool inventoryActivated = false;
    public List<HaveItem> items; // �÷��̾ ������ �����۵��� �����ϴ� ����Ʈ

    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject slotParent; // ������ ��ġ�� �θ� Transform (���Ե��� �θ� ������Ʈ)

    private Slot[] slots; // �� ������ �����ϴ� Slot �迭

    void Awake()
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
    }
    private void CloseInventory()
    {
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
            slots[i].ItemCount = items[i].itemCount;
            slots[i].Item = items[i].item;
        }

        // ���� ������ ������ �� �������� ����
        for (; i < slots.Length; i++)
        {
            slots[i].ItemCount = 0;
            slots[i].Item = null;
        }
    }
    ///// <summary>
    ///// �κ��丮�� �������� �߰��ϴ� �޼���
    ///// </summary>
    ///// <param name="_item">�߰��� ������</param>
    ///// <param name="_count">�߰��� �������� ����</param>
    //public void AddItem(Item _item, int _count = 1)
    //{
    //    // �������� �߰��� �� �ִ� �� ������ ���� ���� �߰�
    //    if (items.Count < slots.Length)
    //    {
    //        items.Add(_item);
    //        FreshSlot();
    //    }
    //    else
    //    {
    //        print("������ ���� �� �ֽ��ϴ�.");
    //    }
    //}
}