using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static bool inventoryActivated = false;
    public List<Item> items; // �÷��̾ ������ �����۵��� �����ϴ� ����Ʈ

    [SerializeField] private GameObject inventoryBase;
    [SerializeField] private GameObject slotParent; // ������ ��ġ�� �θ� Transform (���Ե��� �θ� ������Ʈ)

    private Slot[] slots; // �� ������ �����ϴ� Slot �迭

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

    ///// <summary>
    ///// �κ��丮�� ������ ���� ��ħ�ϴ� �޼���
    ///// </summary>
    //public void FreshSlot()
    //{
    //    int i = 0;
    //    // �����۰� ������ �ִ� ��ŭ ���Կ� �������� �Ҵ�
    //    for (; i < items.Count && i < slots.Length; i++) 
    //    {
    //        slots[i].item = items[i];
    //    }

    //    // ���� ������ ������ �� �������� ����
    //    for (; i < slots.Length; i++)
    //    {
    //        slots[i].item = null;
    //    }
    //}
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