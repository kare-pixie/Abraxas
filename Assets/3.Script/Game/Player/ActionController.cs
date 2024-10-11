using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionController : MonoBehaviour
{
    [SerializeField] private float range; // 습득 가능한 최대 거리
    private bool pickupActivated = false; // 습득 가능할 시 true

    private RaycastHit hitInfo; // 충돌체 정보 저장

    [SerializeField] private LayerMask layerMask; // 아이템 레이어에만 반응하도록 레이어 마스크를 설정
    [SerializeField] private TMP_Text actionText; // 필요한 컴포넌트
    [SerializeField] private Inventory inventory;

    private float sphereRadius = 1.0f; // 아이템 픽업 구체 캐스트의 반경
    private bool isItemDetected = false;

    private void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
    }

    private void Update()
    {
        CheckItem();
        TryAction();
    }

    private void TryAction()
    {
        if (UIManager.instance != null && UIManager.instance.isGameOver) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckItem();
            CanPickUp();
        }
    }

    private void CanPickUp()
    {
        if(pickupActivated)
        {
            if(hitInfo.transform != null)
            {
                inventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                UIManager.instance.ItemLog(hitInfo.transform.GetComponent<ItemPickUp>().item.itemName);
                Destroy(hitInfo.transform.gameObject);
                ItemInfoDisappear();
            }
        }
    }
    private void CheckItem()
    {
        // 아이템을 감지
        if (Physics.SphereCast(transform.position, sphereRadius, transform.TransformDirection(Vector3.forward), out hitInfo, 1f, layerMask))
        {
            if (hitInfo.transform.tag == "Item")
            {
                ItemInfoAppear();
                isItemDetected = true;
            }
            else
            {
                ItemInfoDisappear();
                isItemDetected = false;
            }
        }
        else
        {
            ItemInfoDisappear();
            isItemDetected = false;
        }
    }
    private void OnDrawGizmos()
    {
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        float castDistance = 1f; // SphereCast에서 사용한 거리

        if (isItemDetected)
        {
            Gizmos.color = Color.red; // 충돌 시 빨간색
        }
        else
        {
            Gizmos.color = Color.yellow; // 충돌 없을 시 노란색
        }

        // 시작 위치에 구체 그리기
        Gizmos.DrawWireSphere(transform.position, sphereRadius);

        // 끝 위치에 구체 그리기
        Gizmos.DrawWireSphere(transform.position + direction * castDistance, sphereRadius);

        // 시작점과 끝점을 연결하는 선 그리기
        Gizmos.DrawLine(transform.position, transform.position + direction * castDistance);
    }

    private void ItemInfoAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " 획득 " + "<color=yellow>" + "(E)" + "</color>";
    }

    private void ItemInfoDisappear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
    }
}
