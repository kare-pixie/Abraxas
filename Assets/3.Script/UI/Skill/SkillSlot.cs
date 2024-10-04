using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SkillSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Skill skill;
    [SerializeField] private Image skillImage; // 스킬 이미지
    [SerializeField] private TMP_Text textName; // 스킬 이름
    [SerializeField] private TMP_Text textExplain; // 스킬 설명
    [SerializeField] private TMP_Text textLevel; // 스킬 레벨

    private PlayerStatus playerStatus;
    private Animator animator;
    private VFXController fXController;

    public void Init()
    {
        playerStatus = FindObjectOfType<PlayerStatus>();
        animator = playerStatus.gameObject.GetComponent<Animator>();
        fXController = FindObjectOfType<VFXController>();

        skillImage.sprite = skill.skillImage;
        textLevel.text = skill.skillLevel.ToString();
        textName.text = skill.skillName.ToString();
        textExplain.text = skill.skillExplain.ToString();
    }
    public void UseSkill()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (skill == null || stateInfo.IsTag("BlockMovement") || stateInfo.IsTag("Jump")) return;

        if (playerStatus.CheckManaZero())
        {
            UIManager.instance.ManaZeroLog();
            return;
        }
        animator.SetTrigger(Animator.StringToHash(skill.anim));
        fXController.Play(skill.anim, playerStatus.transform.GetChild(1));
        UIManager.instance.SkillLog(skill.skillName, skill.mana);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (skill != null && skill.skillLevel > 0)
        {
            DragSlot.instance.dragSkillSlot = this;
            DragSlot.instance.DragSetImage(skillImage);

            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (skill != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetImageAlpha(0f);
        DragSlot.instance.dragSlot = null;
    }
}
