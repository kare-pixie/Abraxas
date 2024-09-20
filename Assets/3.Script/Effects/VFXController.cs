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

    [SerializeField] private GameObject _Attack1;
    [SerializeField] private GameObject _Attack2;
    [SerializeField] private GameObject _Slash1;
    [SerializeField] private GameObject _Slash2;
    [SerializeField] private GameObject _Slash3;

    public void Play(int idx, Transform targetTransform = null)
    {
        if (targetTransform != null)
        {
            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
        }

        if (mCoEnable != null)
            StopCoroutine(mCoDisable);

        mCoEnable = StartCoroutine(CoEnable(idx));

        if (mCoDisable != null)
            StopCoroutine(mCoDisable);

        mCoDisable = StartCoroutine(CoDisable(idx));
    }

    private IEnumerator CoDisable(int idx)
    {
        yield return new WaitForSeconds(mLifeTime);
        switch(idx)
        {
            case 0:
                _Attack1.SetActive(false); break;
            case 1:
                _Attack2.SetActive(false);break;
            case 2:
                _Slash1.SetActive(false);break;
            case 3:
                _Slash2.SetActive(false);break;
            case 4:
                _Slash3.SetActive(false);break;
        }
    }
    private IEnumerator CoEnable(int idx)
    {
        yield return new WaitForSeconds(mStartTime);
        switch (idx)
        {
            case 0:
                _Attack1.SetActive(true); break;
            case 1:
                _Attack2.SetActive(true); break;
            case 2:
                _Slash1.SetActive(true); break;
            case 3:
                _Slash2.SetActive(true); break;
            case 4:
                _Slash3.SetActive(true); break;
        }
        CoDisable(idx);
    }
}
