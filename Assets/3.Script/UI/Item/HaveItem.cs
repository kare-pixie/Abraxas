using UnityEngine;
[CreateAssetMenu(fileName = "New Have Item", menuName = "Item/haveItem")]
public class HaveItem : ScriptableObject
{
    public Item item;
    public int itemCount;
}