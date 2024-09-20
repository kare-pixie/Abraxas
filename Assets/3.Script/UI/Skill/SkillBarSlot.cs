using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBarSlot : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public Skill skill;

    [SerializeField] private Image skillImage; // 스킬 이미지

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UseSkill();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            ClearSlot(); // 슬롯 해제
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
        //todo: 스킬사용
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

        SetImageAlpha(1f); // 이미지 표시 (불투명)
    }

    /// <summary>
    /// 이미지의 투명도 조절
    /// </summary>
    /// <param name="alpha">투명도</param>
    private void SetImageAlpha(float alpha)
    {
        Color color = skillImage.color;  // 현재 이미지의 색상 값을 가져옴
        color.a = alpha;  // 알파값 설정
        skillImage.color = color;  // 변경된 색상 다시 설정
    }
}
