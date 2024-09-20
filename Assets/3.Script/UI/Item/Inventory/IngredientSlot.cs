using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Craft.instance.ClearItem();
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null && DragSlot.instance.dragSlot.item.itemType.Equals(Item.ItemType.Ingredient))
        {
            Craft.instance.SetImage(DragSlot.instance.dragSlot.item);
        }
    }
}
