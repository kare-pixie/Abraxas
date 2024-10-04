using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    private Animator animator;
    private bool hasAnimator;

    public SkillBarSlot[] SkillSlot;

    [SerializeField] private SaveSkillSlot saveSkillSlot;

    private void Start()
    {
        hasAnimator = TryGetComponent(out animator);
        LoadSkillSlot();
    }

    private void LoadSkillSlot()
    {
        if (saveSkillSlot != null)
        {
            for (int i = 0; i < saveSkillSlot.skill.Length; i++)
            {
                if (saveSkillSlot.skill[i] != null)
                {
                    SkillSlot[i].SetSlot(saveSkillSlot.skill[i]);
                }
            }
        }
    }
    public void SaveSkillSlot()
    {
        if (saveSkillSlot != null)
        {
            for (int i = 0; i < saveSkillSlot.skill.Length; i++)
            {
                saveSkillSlot.skill[i] = SkillSlot[i].skill;
            }
        }
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
            SkillSlot[0].UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SkillSlot[1].UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            SkillSlot[2].UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            SkillSlot[3].UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SkillSlot[4].UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            SkillSlot[5].UseSkill();
        }
    }
}
