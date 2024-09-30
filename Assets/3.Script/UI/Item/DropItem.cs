using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Drop Item", menuName = "Item/DropItem")]
public class DropItem : ScriptableObject
{
    public List<Item> items;
}
