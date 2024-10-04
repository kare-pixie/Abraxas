using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Inventory : MonoBehaviour
{
    private bool inventoryActivated = false;
    public List<HaveItem> items; // 플레이어가 소유한 아이템들을 저장하는 리스트

    public PotionSlot potionSlot1; // 하단바 포션 슬롯
    public PotionSlot potionSlot2; // 하단바 포션 슬롯

    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject slotParent; // 슬롯이 배치된 부모 Transform (슬롯들의 부모 오브젝트)

    private ItemSlot[] slots; // 각 슬롯을 관리하는 Slot 배열

    private string saveFilePath;

    private void Awake()
    {
        slots = slotParent.GetComponentsInChildren<ItemSlot>();
        saveFilePath = Path.Combine(Application.dataPath, "inventory.json");
        FreshSlot();
    }
    private void Start()
    {
        RefreshPotion();
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

    public void RefreshPotion()
    {
        potionSlot1.FreshCount();
        potionSlot2.FreshCount();
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

        // 남은 슬롯이 있으면 빈 슬롯으로 설정
        for (; i < slots.Length; i++)
        {
            slots[i].itemCount = 0;
            slots[i].Item = null;
        }
    }

    // JSON 형태로 아이템 리스트 저장
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
            potionSlot1 = potionSlot1.Item,
            potionSlot2 = potionSlot2.Item
        };

        string json = JsonUtility.ToJson(data, true); 
        File.WriteAllText(saveFilePath, json);
        Debug.Log("인벤토리 저장 완료");
    }

    // JSON 파일에서 아이템 리스트 불러오기
    public void LoadInventory()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            InventoryData data = JsonUtility.FromJson<InventoryData>(json);

            items = data.items;
            if(data.potionSlot1 != null)
                potionSlot1.SetSlot(data.potionSlot1);
            if (data.potionSlot2 != null)
                potionSlot2.SetSlot(data.potionSlot2);

            FreshSlot();
            Debug.Log("인벤토리 불러오기 완료");
        }
        else
        {
            Debug.LogWarning("저장된 인벤토리가 없습니다.");
        }
    }

    [System.Serializable]
    public class InventoryData
    {
        public List<HaveItem> items;
        public Item potionSlot1;
        public Item potionSlot2;
    }
}