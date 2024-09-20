using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    static public DragSlot instance = null;

    public ItemSlot dragSlot;
    public SkillSlot dragSkillSlot;

    [SerializeField] private Image image; // ������ �̹���

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
    /// �̹����� ���� ����
    /// </summary>
    /// <param name="alpha">����</param>
    public void SetImageAlpha(float alpha)
    {
        Color color = image.color;  // ���� �̹����� ���� ���� ������
        color.a = alpha;  // ���İ� ����
        image.color = color;  // ����� ���� �ٽ� ����
    }
}
