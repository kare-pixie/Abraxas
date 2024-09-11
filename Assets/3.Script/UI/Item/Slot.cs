using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Item item; // ȹ���� ������

    private int itemCount; // ȹ���� �������� ����

    [SerializeField] private Image itemImage; // �������� �̹���
    [SerializeField] private TMP_Text text_Count;
    [SerializeField] private GameObject CountImage;

    public int ItemCount 
    {
        get { return ItemCount; }
        set { itemCount = value; }
    }
    public Item Item
    {
        get { return item; }
        set
        {
            item = value;

            // �������� �ִ� ���
            if (item != null)
            {
                itemImage.sprite = Item.itemImage;
                SetImageAlpha(1f); // �̹��� ǥ�� (������)
                if (item.itemType != Item.ItemType.Equipment)
                {
                    CountImage.SetActive(true);
                    text_Count.text = itemCount.ToString();
                }
            }
            // �������� ���� ���
            else
            {
                SetImageAlpha(0f); // �̹��� ����� (����)
                CountImage.SetActive(false);
            }
        }
    }

    /// <summary>
    /// ������ ȹ��
    /// </summary>
    /// <param name="_item">ȹ���� ������</param>
    /// <param name="_count">ȹ���� �������� ����</param>
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
        SetImageAlpha(1f); // �̹��� ǥ�� (������)
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(item != null)
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(item != null)
            {
                if(item.itemType == Item.ItemType.Ingredient)
                {
                    //todo: ���� ����
                }
                else
                {
                    Debug.Log(item.itemName + " �� ����߽��ϴ�.");
                    SetSlotCount(-1);
                }
            }
        }
    }

    /// <summary>
    /// ������ ���� ����
    /// </summary>
    /// <param name="_count">���� ������ ����</param>
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
    /// ���� �ʱ�ȭ
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
    /// �̹����� ���� ����
    /// </summary>
    /// <param name="alpha">����</param>
    private void SetImageAlpha(float alpha)
    {
        Color color = itemImage.color;  // ���� �̹����� ���� ���� ������
        color.a = alpha;  // ���İ� ����
        itemImage.color = color;  // ����� ���� �ٽ� ����
    }
}