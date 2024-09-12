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

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // �⺻ Item Ŭ���� �ʵ带 ǥ��
        Item item = (Item)target;

        // �⺻ �ν����� �ʵ� ǥ��
        item.itemName = EditorGUILayout.TextField("Item Name", item.itemName);
        item.itemType = (Item.ItemType)EditorGUILayout.EnumPopup("Item Type", item.itemType);
        item.itemImage = (Sprite)EditorGUILayout.ObjectField("Item Image", item.itemImage, typeof(Sprite), false);
        item.itemPrefab = (GameObject)EditorGUILayout.ObjectField("Item Prefab", item.itemPrefab, typeof(GameObject), false);

        // itemType�� Ingredient�� ���� usedItem �ʵ� ����
        if (item.itemType == Item.ItemType.Ingredient)
        {
            item.usedItem = (Item)EditorGUILayout.ObjectField("Used Item", item.usedItem, typeof(Item), false);
        }

        // ���� ���� ����
        if (GUI.changed)
        {
            EditorUtility.SetDirty(item);
        }
    }
}