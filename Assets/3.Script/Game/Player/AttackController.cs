using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    private Animator animator;
    private bool hasAnimator;

    public SkillBarSlot SkillSlot1;
    public SkillBarSlot SkillSlot2;
    public SkillBarSlot SkillSlot3;
    public SkillBarSlot SkillSlot4;
    public SkillBarSlot SkillSlot5;
    public SkillBarSlot SkillSlot6;

    private void Start()
    {
        hasAnimator = TryGetComponent(out animator);
    }
    private void Update()
    {
        if (UIManager.instance != null && UIManager.instance.isGameOver) return;

        hasAnimator = TryGetComponent(out animator);

        if (!hasAnimator) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsTag("BlockMovement") || stateInfo.IsTag("Jump")) return;

        if (Input.GetKeyDown(KeyCode.F1))
        {
            SkillSlot1.UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SkillSlot2.UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            SkillSlot3.UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            SkillSlot4.UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SkillSlot5.UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            SkillSlot6.UseSkill();
        }
    }
}
