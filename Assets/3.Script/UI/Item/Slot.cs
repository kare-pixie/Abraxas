using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(item != null)
            {
                if(item.itemType == Item.ItemType.Ingredient)
                {
                    // ���� ���� ����
                }
                else
                {
                    // ���
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