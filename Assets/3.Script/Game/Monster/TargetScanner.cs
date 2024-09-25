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
    /// �� ��ĳ���� �Ķ���Ϳ� ���� �÷��̾ ���̴��� Ȯ���մϴ�.
    /// </summary>
    /// <param name="detector">Ž���� ���۵Ǵ� Ʈ������ (�� ĳ������ ��ġ)</param>
    /// <param name="useHeightDifference">���� ���̸� maxHeightDifference ���� ������, �������� ����</param>
    /// <returns>�÷��̾ ���̸� PlayerController, ������ ������ null ��ȯ</returns>
    public PlayerController Detect(Transform detector, bool useHeightDifference = true)
    {
        //�÷��̾ �������� �ʾҰų� ���� ���̸�, Ÿ������ �������� ����
        if (PlayerController.instance == null || PlayerController.instance.respawning)
            return null;

        Vector3 eyePos = detector.position + Vector3.up * heightOffset;
        Vector3 toPlayer = PlayerController.instance.transform.position - eyePos;
        Vector3 toPlayerTop = PlayerController.instance.transform.position + Vector3.up * 1.5f - eyePos;

        if (useHeightDifference && Mathf.Abs(toPlayer.y + heightOffset) > maxHeightDifference)
        { //Ÿ���� �ʹ� ���ų� ������ ������ �õ����� ����
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
