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

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 Item 클래스 필드를 표시
        Item item = (Item)target;

        // 기본 인스펙터 필드 표시
        item.itemName = EditorGUILayout.TextField("Item Name", item.itemName);
        item.itemType = (Item.ItemType)EditorGUILayout.EnumPopup("Item Type", item.itemType);
        item.itemImage = (Sprite)EditorGUILayout.ObjectField("Item Image", item.itemImage, typeof(Sprite), false);
        item.itemPrefab = (GameObject)EditorGUILayout.ObjectField("Item Prefab", item.itemPrefab, typeof(GameObject), false);

        // itemType이 Ingredient일 때만 usedItem 필드 노출
        if (item.itemType == Item.ItemType.Ingredient)
        {
            item.usedItem = (Item)EditorGUILayout.ObjectField("Used Item", item.usedItem, typeof(Item), false);
        }

        // 변경 사항 저장
        if (GUI.changed)
        {
            EditorUtility.SetDirty(item);
        }
    }
}