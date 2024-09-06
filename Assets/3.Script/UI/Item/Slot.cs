using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour
{
    public Item item; // 획득한 아이템
    public int itemCount; // 획득한 아이템의 개수

    [SerializeField] private Image itemImage; // 아이템의 이미지
    [SerializeField] private TMP_Text text_Count;
    [SerializeField] private GameObject CountImage;

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
            text_Count.text = this.itemCount.ToString();
        }
        else
        {
            text_Count.text = "0";
            CountImage.SetActive(false);
        }
        SetImageAlpha(1f); // 이미지 표시 (불투명)
    }
    /// <summary>
    /// 아이템 개수 조정
    /// </summary>
    /// <param name="_count">더할 아이템 개수</param>
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

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

        text_Count.text = "0";
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

    //public Item item
    //{
    //    get { return _item; }
    //    set
    //    {
    //        _item = value;

    //        // 아이템이 있는 경우
    //        if (_item != null)
    //        {
    //            itemImage.sprite = item.itemImage;
    //            SetImageAlpha(1f); // 이미지 표시 (불투명)
    //        }
    //        // 아이템이 없는 경우
    //        else
    //        {
    //            SetImageAlpha(0f); // 이미지 숨기기 (투명)
    //        }
    //    }
    //}
}