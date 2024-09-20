using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PotionSlot : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    private Item item;

    public int itemCount; // ȹ���� �������� ����

    [SerializeField] private Image itemImage; // �������� �̹���
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
            ClearSlot(); // ���� ���� ����
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
        //todo: ���� ���
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

        SetImageAlpha(1f); // �̹��� ǥ�� (������)
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
