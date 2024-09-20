using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PotionSlot : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    private Item item;

    public int itemCount; // 획득한 아이템의 개수

    [SerializeField] private Image itemImage; // 아이템의 이미지
    [SerializeField] private TMP_Text textCount;
    [SerializeField] private GameObject CountImage;

    private Inventory inventory;

    private void Start()
    {
        inventory = FindAnyObjectByType<Inventory>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UseItem();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            ClearSlot(); // 포션 슬롯 해제
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();
        }
    }
    public void FreshCount()
    {
        if (item == null) return;

        itemCount = inventory.getItemCount(item);

        if (itemCount == 0)
        {
            ClearSlot();
            return;
        }

        textCount.text = itemCount.ToString();
    }
    public void UseItem()
    {
        if (item == null)
        {
            return;
        }
        //todo: 포션 사용
        inventory.UseItem(item);
        itemCount = inventory.getItemCount(item);

        if (itemCount == 0)
        {
            ClearSlot();
        }
        else
        {
            textCount.text = itemCount.ToString();
        }
    }
    private void ChangeSlot()
    {
        if(DragSlot.instance.dragSlot.item.itemType.Equals(Item.ItemType.Used))
        {
            SetSlot(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);
            DragSlot.instance.dragSlot = null;
        }
    }
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetImageAlpha(0f);

        textCount.text = "0";
        CountImage.SetActive(false);
    }
    private void SetSlot(Item _item, int _count = 1)
    {
        this.item = _item;
        this.itemCount = _count;
        itemImage.sprite = _item.itemImage;

        CountImage.SetActive(true);
        textCount.text = this.itemCount.ToString();

        SetImageAlpha(1f); // 이미지 표시 (불투명)
    }

    /// <summary>
    /// 이미지의 투명도 조절
    /// </summary>
    /// <param name="alpha">투명도</param>
    private void SetImageAlpha(float alpha)
    {
        Color color = itemImage.color;  // 현재 이미지의 색상 값을 가져옴
        color.a = alpha;  // 알파값 설정
        itemImage.color = color;  // 변경된 색상 다시 설정
    }
}
