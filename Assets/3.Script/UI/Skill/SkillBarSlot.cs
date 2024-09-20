using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBarSlot : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public Skill skill;

    [SerializeField] private Image skillImage; // ��ų �̹���

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UseSkill();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            ClearSlot(); // ���� ����
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSkillSlot != null)
        {
            ChangeSlot();
        }
    }

    public void UseSkill()
    {
        if (skill == null)
        {
            return;
        }
        //todo: ��ų���
    }
    private void ChangeSlot()
    {
        SetSlot(DragSlot.instance.dragSkillSlot);
        DragSlot.instance.dragSkillSlot = null;
    }
    private void ClearSlot()
    {
        skill = null;
        skillImage.sprite = null;
        SetImageAlpha(0f);
    }
    private void SetSlot(SkillSlot _skill)
    {
        this.skillImage.sprite = _skill.skill.skillImage;

        SetImageAlpha(1f); // �̹��� ǥ�� (������)
    }

    /// <summary>
    /// �̹����� ���� ����
    /// </summary>
    /// <param name="alpha">����</param>
    private void SetImageAlpha(float alpha)
    {
        Color color = skillImage.color;  // ���� �̹����� ���� ���� ������
        color.a = alpha;  // ���İ� ����
        skillImage.color = color;  // ����� ���� �ٽ� ����
    }
}
