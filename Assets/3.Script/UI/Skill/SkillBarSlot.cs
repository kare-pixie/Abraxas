using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBarSlot : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    private Skill skill;
    
    private int animID;
    private string anim;
    private string skillName;
    private int skillMana;

    [SerializeField] private Animator animator;
    [SerializeField] private VFXController fXController;
    [SerializeField] private GameObject character;

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
        animator.SetTrigger(animID);
        fXController.Play(anim, character.transform.GetChild(1));
        UIManager.instance.SkillLog(skillName, skillMana);
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
    private void SetSlot(SkillSlot _slot)
    {
        skill = _slot.skill;
        skillImage.sprite = _slot.skill.skillImage;
        anim = _slot.skill.anim;
        skillName = _slot.skill.skillName;
        skillMana = _slot.skill.mana;
        animID = Animator.StringToHash(_slot.skill.anim);

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
