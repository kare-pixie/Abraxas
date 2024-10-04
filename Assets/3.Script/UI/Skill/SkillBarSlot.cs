using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBarSlot : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public Skill skill { get; private set; }
    
    private int animID;
    private string anim;
    private string skillName;
    private int skillMana;

    [SerializeField] private Animator animator;
    [SerializeField] private VFXController fXController;
    [SerializeField] private GameObject character;

    [SerializeField] private Image skillImage; // 스킬 이미지

    private PlayerStatus playerStatus;

    private void Awake()
    {
        playerStatus = FindObjectOfType<PlayerStatus>();
    }
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
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (skill == null || stateInfo.IsTag("BlockMovement") || stateInfo.IsTag("Jump")) return;
        if (playerStatus.CheckManaZero(skillMana))
        {
            UIManager.instance.ManaZeroLog();
            return;
        }
        animator.SetTrigger(animID);
        fXController.Play(anim, character.transform.GetChild(1));
        UIManager.instance.SkillLog(skillName, skillMana);
    }
    private void ChangeSlot()
    {
        SetSlot(DragSlot.instance.dragSkillSlot.skill);
        DragSlot.instance.dragSkillSlot = null;
    }
    private void ClearSlot()
    {
        skill = null;
        skillImage.sprite = null;
        SetImageAlpha(0f);
    }
    public void SetSlot(Skill _slot)
    {
        skill = _slot;
        skillImage.sprite = _slot.skillImage;
        anim = _slot.anim;
        skillName = _slot.skillName;
        skillMana = _slot.mana;
        animID = Animator.StringToHash(_slot.anim);

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
