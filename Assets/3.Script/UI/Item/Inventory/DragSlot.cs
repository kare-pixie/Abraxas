using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    static public DragSlot instance = null;

    public ItemSlot dragSlot;
    public SkillSlot dragSkillSlot;

    [SerializeField] private Image image; // 아이템 이미지

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void DragSetImage(Image _itemImage)
    {
        image.sprite = _itemImage.sprite;
        SetImageAlpha(1f);
    }

    /// <summary>
    /// 이미지의 투명도 조절
    /// </summary>
    /// <param name="alpha">투명도</param>
    public void SetImageAlpha(float alpha)
    {
        Color color = image.color;  // 현재 이미지의 색상 값을 가져옴
        color.a = alpha;  // 알파값 설정
        image.color = color;  // 변경된 색상 다시 설정
    }
}
