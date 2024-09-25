using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    [Header("VFX ���ӽð�")]
    [SerializeField] private float mStartTime;
    [SerializeField] private float mLifeTime;

    private Coroutine mCoEnable; // ��Ȱ��ȭ �ڷ�ƾ
    private Coroutine mCoDisable; // ��Ȱ��ȭ �ڷ�ƾ

    [SerializeField] private GameObject attack1;
    [SerializeField] private GameObject attack2;
    [SerializeField] private GameObject slash1;
    [SerializeField] private GameObject slash2;
    [SerializeField] private GameObject slash3;

    [SerializeField] private Weapon weapon;

    private void Awake()
    {
        weapon = FindObjectOfType<Weapon>();
    }
    public void Play(string anim, Transform targetTransform = null)
    {
        if (targetTransform != null)
        {
            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
        }

        if (mCoEnable != null)
            StopCoroutine(mCoDisable);

        mCoEnable = StartCoroutine(CoEnable(anim));

        if (mCoDisable != null)
            StopCoroutine(mCoDisable);

        mCoDisable = StartCoroutine(CoDisable(anim));
    }

    private IEnumerator CoDisable(string anim)
    {
        yield return new WaitForSeconds(mLifeTime);
        weapon.SkillNoUse();
        switch (anim)
        {
            //case "Attack1":
            //    attack1.SetActive(false); break;
            //case "Attack2":
            //    attack2.SetActive(false); break;
            case "Slash1":
                slash1.SetActive(false); break;
            case "Slash2":
                slash2.SetActive(false); break;
            //case "Slash3":
            //    slash3.SetActive(false); break;
        }
    }
    private IEnumerator CoEnable(string anim)
    {
        weapon.SkillUse(anim);
        yield return new WaitForSeconds(mStartTime);
        switch (anim)
        {
            //case "Attack1":
            //    attack1.SetActive(true); break;
            //case "Attack2":
            //    attack2.SetActive(true); break;
            case "Slash1":
                slash1.SetActive(true); break;
            case "Slash2":
                slash2.SetActive(true); break;
                //case "Slash3":
                //    slash3.SetActive(true); break;
        }
    }
}
