using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    // animation IDs
    private int _animIDBlock;
    private int _animIDAttack1;
    private int _animIDAttack2;
    private int _animIDSlash1;
    private int _animIDSlash2;
    private int _animIDSlash3;
    private int _animIDBuff;

    private Animator _animator;
    private bool _hasAnimator;

    [SerializeField] VFXController fXController;

    private void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);
        AssignAnimationIDs();
    }
    private void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsTag("BlockMovement") || stateInfo.IsTag("Jump")) return;
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (_hasAnimator)
            {
                _animator.SetTrigger(_animIDBuff);
            }
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (_hasAnimator)
            {
                _animator.SetTrigger(_animIDAttack1);
            }
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (_hasAnimator)
            {
                _animator.SetTrigger(_animIDAttack2);
            }
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (_hasAnimator)
            {
                _animator.SetTrigger(_animIDSlash1);

                fXController.Play(2, transform.GetChild(1));
            }
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (_hasAnimator)
            {
                _animator.SetTrigger(_animIDSlash2);

                fXController.Play(3, transform.GetChild(1));
            }
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            if (_hasAnimator)
            {
                _animator.SetTrigger(_animIDSlash3);
            }
        }
    }
    private void AssignAnimationIDs()
    {
        _animIDBlock = Animator.StringToHash("Block");
        _animIDAttack1 = Animator.StringToHash("Attack1");
        _animIDAttack2 = Animator.StringToHash("Attack2");
        _animIDSlash1 = Animator.StringToHash("Slash1");
        _animIDSlash2 = Animator.StringToHash("Slash2");
        _animIDSlash3 = Animator.StringToHash("Slash3");
        _animIDBuff = Animator.StringToHash("Buff");
    }

}
