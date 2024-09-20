using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    [Header("VFX 지속시간")]
    [SerializeField] private float mStartTime;
    [SerializeField] private float mLifeTime;

    private Coroutine mCoEnable; // 비활성화 코루틴
    private Coroutine mCoDisable; // 비활성화 코루틴

    [SerializeField] private GameObject attack1;
    [SerializeField] private GameObject attack2;
    [SerializeField] private GameObject slash1;
    [SerializeField] private GameObject slash2;
    [SerializeField] private GameObject slash3;

    public void Play(string ani, Transform targetTransform = null)
    {
        if (targetTransform != null)
        {
            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
        }

        if (mCoEnable != null)
            StopCoroutine(mCoDisable);

        mCoEnable = StartCoroutine(CoEnable(ani));

        if (mCoDisable != null)
            StopCoroutine(mCoDisable);

        mCoDisable = StartCoroutine(CoDisable(ani));
    }

    private IEnumerator CoDisable(string ani)
    {
        yield return new WaitForSeconds(mLifeTime);
        switch (ani)
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
    private IEnumerator CoEnable(string ani)
    {
        yield return new WaitForSeconds(mStartTime);
        switch (ani)
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
