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
            }
        }
        else
        {
            ItemInfoDisappear();
        }
    }
    private void OnDrawGizmos()
    {
        // 구체 캐스트 색상 설정
        Gizmos.color = Color.yellow;

        // 캐릭터가 바라보는 방향으로 구체를 그리기 위한 위치와 방향 계산
        Vector3 direction = transform.TransformDirection(Vector3.forward);

        // SphereCast 범위를 시각화
        Gizmos.DrawWireSphere(transform.position + direction * range, sphereRadius);
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
