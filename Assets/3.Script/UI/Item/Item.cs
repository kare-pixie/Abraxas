using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public enum ItemType
    {
        Matter,
        Equipment
    }
    public string itemName;
    public Sprite itemImage;
    public ItemType itemType;
}