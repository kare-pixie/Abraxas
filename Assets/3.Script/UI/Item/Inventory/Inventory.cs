using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Inventory : MonoBehaviour
{
    private bool inventoryActivated = false;
    public List<HaveItem> items; // �÷��̾ ������ �����۵��� �����ϴ� ����Ʈ

    public PotionSlot potionSlot1; // �ϴܹ� ���� ����
    public PotionSlot potionSlot2; // �ϴܹ� ���� ����

    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject slotParent; // ������ ��ġ�� �θ� Transform (���Ե��� �θ� ������Ʈ)

    private ItemSlot[] slots; // �� ������ �����ϴ� Slot �迭

    private string saveFilePath;

    private void Awake()
    {
        slots = slotParent.GetComponentsInChildren<ItemSlot>();
        saveFilePath = Path.Combine(Application.dataPath, "inventory.json");
        FreshSlot();
    }
    private void OnEnable()
    {
        LoadInventory();
    }

    private void Update()
    {
        if (UIManager.instance != null && UIManager.instance.isGameOver) return;

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
            if(items[i] == null)
            {
                slots[i].itemCount = 0;
                slots[i].Item = null;
            }
            else
            {
                slots[i].itemCount = items[i].itemCount;
                slots[i].Item = items[i].item;
            }
        }

        // ���� ������ ������ �� �������� ����
        for (; i < slots.Length; i++)
        {
            slots[i].itemCount = 0;
            slots[i].Item = null;
        }
    }

    // JSON ���·� ������ ����Ʈ ����
    public void SaveInventory()
    {
        items = new List<HaveItem>();
        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i].Item != null)
            {
                HaveItem item = new HaveItem();
                item.item = slots[i].Item;
                item.itemCount = slots[i].itemCount;
                items.Add(item);
            }
            else
            {
                items.Add(null);
            }
        }

        InventoryData data = new InventoryData
        {
            items = items,
            potionSlot1 = potionSlot1,
            potionSlot2 = potionSlot2
        };

        string json = JsonUtility.ToJson(data, true); 
        File.WriteAllText(saveFilePath, json);
        Debug.Log("�κ��丮 ���� �Ϸ�");
    }

    // JSON ���Ͽ��� ������ ����Ʈ �ҷ�����
    public void LoadInventory()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            InventoryData data = JsonUtility.FromJson<InventoryData>(json);

            items = data.items;
            potionSlot1 = data.potionSlot1;
            potionSlot2 = data.potionSlot2;

            FreshSlot();
            Debug.Log("�κ��丮 �ҷ����� �Ϸ�");
        }
        else
        {
            Debug.LogWarning("����� �κ��丮�� �����ϴ�.");
        }
    }

    [System.Serializable]
    public class InventoryData
    {
        public List<HaveItem> items;
        public PotionSlot potionSlot1;
        public PotionSlot potionSlot2;
    }
}