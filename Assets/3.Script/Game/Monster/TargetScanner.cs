using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargetScanner
{
    public float heightOffset = 0.0f;
    public float detectionRadius = 10;
    [Range(0.0f, 360.0f)]
    public float detectionAngle = 270;
    public float maxHeightDifference = 1.0f;
    public LayerMask viewBlockerLayerMask;
    /// <summary>
    /// 이 스캐너의 파라미터에 따라 플레이어가 보이는지 확인합니다.
    /// </summary>
    /// <param name="detector">탐지가 시작되는 트랜스폼 (적 캐릭터의 위치)</param>
    /// <param name="useHeightDifference">높이 차이를 maxHeightDifference 값과 비교할지, 무시할지 여부</param>
    /// <returns>플레이어가 보이면 PlayerController, 보이지 않으면 null 반환</returns>
    public PlayerController Detect(Transform detector, bool useHeightDifference = true)
    {
        //플레이어가 스폰되지 않았거나 스폰 중이면, 타겟으로 설정하지 않음
        if (PlayerController.instance == null || PlayerController.instance.respawning)
            return null;

        Vector3 eyePos = detector.position + Vector3.up * heightOffset;
        Vector3 toPlayer = PlayerController.instance.transform.position - eyePos;
        Vector3 toPlayerTop = PlayerController.instance.transform.position + Vector3.up * 1.5f - eyePos;

        if (useHeightDifference && Mathf.Abs(toPlayer.y + heightOffset) > maxHeightDifference)
        { //타겟이 너무 높거나 낮으면 추적을 시도하지 않음
            return null;
        }

        Vector3 toPlayerFlat = toPlayer;
        toPlayerFlat.y = 0;

        if (toPlayerFlat.sqrMagnitude <= detectionRadius * detectionRadius)
        {
            if (Vector3.Dot(toPlayerFlat.normalized, detector.forward) >
                Mathf.Cos(detectionAngle * 0.5f * Mathf.Deg2Rad))
            {

                bool canSee = false;

                Debug.DrawRay(eyePos, toPlayer, Color.blue);
                Debug.DrawRay(eyePos, toPlayerTop, Color.blue);

                canSee |= !Physics.Raycast(eyePos, toPlayer.normalized, detectionRadius,
                    viewBlockerLayerMask, QueryTriggerInteraction.Ignore);

                canSee |= !Physics.Raycast(eyePos, toPlayerTop.normalized, toPlayerTop.magnitude,
                    viewBlockerLayerMask, QueryTriggerInteraction.Ignore);

                if (canSee)
                    return PlayerController.instance;
            }
        }

        return null;
    }

}
