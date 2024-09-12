using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Craft : MonoBehaviour
{
    static public Craft instance;

    [SerializeField] private Image ingredientItemImage; // ������ �̹���
    [SerializeField] private Image usedItemImage; // ������ �̹���

    [SerializeField] private TMP_Text ingredientItemCount;
    [SerializeField] private TMP_Text usedItemImageCount;

    private Item ingredientItem;
    private Item usedItem;
    private Inventory inventory;
    private int ingredientCnt;
    private int usedCnt;

    private void Start()
    {
        instance = this;
        inventory = FindAnyObjectByType<Inventory>();
    }

    public void SetImage(Item _item)
    {
        ingredientItem = _item;
        usedItem = _item.usedItem;

        ingredientItemImage.sprite = _item.itemImage;
        usedItemImage.sprite = _item.usedItem.itemImage;

        SetImageAlpha(1f);

        FreshCount();
    }

    public void FreshCount()
    {
        if (ingredientItem == null || usedItem == null) return;

        ingredientCnt = inventory.getItemCount(ingredientItem);
        usedCnt = inventory.getItemCount(usedItem);

        if(ingredientCnt == 0)
        {
            ClearItem();
            return;
        }

        ingredientItemCount.text = ingredientCnt.ToString();
        usedItemImageCount.text = usedCnt.ToString();
    }
    private void ClearItem()
    {
        ingredientItemCount.text = string.Empty;
        usedItemImageCount.text = string.Empty;

        ingredientItem = null;
        usedItem = null;

        ingredientItemImage.sprite = null;
        usedItemImage.sprite = null;

        SetImageAlpha(0f);
    }

    public void TryCraft()
    {
        if (ingredientItem == null || usedItem == null || ingredientCnt == 0)
            return;

        inventory.AcquireItem(usedItem);
        inventory.UseItem(ingredientItem);

        FreshCount();
    }

    /// <summary>
    /// �̹����� ���� ����
    /// </summary>
    /// <param name="alpha">����</param>
    public void SetImageAlpha(float alpha)
    {
        Color color = ingredientItemImage.color;  // ���� �̹����� ���� ���� ������
        color.a = alpha;  // ���İ� ����
        ingredientItemImage.color = color;  // ����� ���� �ٽ� ����
        usedItemImage.color = color;  // ����� ���� �ٽ� ����
    }
}
