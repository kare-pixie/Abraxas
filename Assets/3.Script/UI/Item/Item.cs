using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/item")]
public class Item : ScriptableObject
{
    public string itemName; // 아이템의 이름
    public ItemType itemType; // 아이템의 유형
    public Sprite itemImage; // 아이템의 이미지
    public GameObject itemPrefab; // 아이템의 프리팹
    public Item usedItem;
    public enum ItemType
    {
        Ingredient,
        Used,
        Equipment,
        ETC
    }
}
