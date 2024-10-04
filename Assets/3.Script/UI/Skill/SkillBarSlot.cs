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

    [SerializeField] private Image skillImage; // ��ų �̹���

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
