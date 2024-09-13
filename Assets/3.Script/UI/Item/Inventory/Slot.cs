using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Item item; // 획득한 아이템

    public int itemCount; // 획득한 아이템의 개수

    [SerializeField] private Image itemImage; // 아이템의 이미지
    [SerializeField] private TMP_Text textCount;
    [SerializeField] private GameObject CountImage;

    public Item Item
    {
        get { return item; }
        set
        {
            item = value;

            // 아이템이 있는 경우
            if (item != null)
            {
                itemImage.sprite = Item.itemImage;
                SetImageAlpha(1f); // 이미지 표시 (불투명)
                if (item.itemType != Item.ItemType.Equipment)
                {
                    CountImage.SetActive(true);
                    textCount.text = itemCount.ToString();
                }
            }
            // 아이템이 없는 경우
            else
            {
                SetImageAlpha(0f); // 이미지 숨기기 (투명)
                CountImage.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 아이템 획득
    /// </summary>
    /// <param name="_item">획득한 아이템</param>
    /// <param name="_count">획득한 아이템의 개수</param>
    public void AddItem(Item _item, int _count = 1)
    {
        this.item = _item;
        this.itemCount = _count;
        itemImage.sprite = _item.itemImage;

        if(_item.itemType != Item.ItemType.Equipment)
        {
            CountImage.SetActive(true);
            textCount.text = this.itemCount.ToString();
        }
        else
        {
            textCount.text = "0";
            CountImage.SetActive(false);
        }
        SetImageAlpha(1f); // 이미지 표시 (불투명)
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(eventData.button);
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null)
            {
                if (item.itemType == Item.ItemType.Ingredient)
                {
                    Craft.instance.SetImage(item);
                }
                else
                {
                    Debug.Log(item.itemName + "을 사용했습니다.");
                    SetSlotCount(-1);
                    Craft.instance.FreshCount();
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);

            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetImageAlpha(0f);
        DragSlot.instance.dragSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();
        }
    }

    private void ChangeSlot()
    {
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        if(_tempItem != null)
        {
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        }
        else
        {
            DragSlot.instance.dragSlot.ClearSlot();
        }
    }

    /// <summary>
    /// 아이템 개수 조정
    /// </summary>
    /// <param name="_count">더할 아이템 개수</param>
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        textCount.text = itemCount.ToString();

        if(itemCount <= 0)
        {
            ClearSlot();
        }
    }
    /// <summary>
    /// 슬롯 초기화
    /// </summary>
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetImageAlpha(0f);

        textCount.text = "0";
        CountImage.SetActive(false);
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