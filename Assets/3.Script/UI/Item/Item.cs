using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/item")]
public class Item : ScriptableObject
{
    public string itemName; // �������� �̸�
    public ItemType itemType; // �������� ����
    public Sprite itemImage; // �������� �̹���
    public GameObject itemPrefab; // �������� ������
    public Item usedItem;
    public enum ItemType
    {
        Ingredient,
        Used,
        Equipment,
        ETC
    }
}
