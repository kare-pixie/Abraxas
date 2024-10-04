using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionController : MonoBehaviour
{
    [SerializeField] private float range; // ���� ������ �ִ� �Ÿ�
    private bool pickupActivated = false; // ���� ������ �� true

    private RaycastHit hitInfo; // �浹ü ���� ����

    [SerializeField] private LayerMask layerMask; // ������ ���̾�� �����ϵ��� ���̾� ����ũ�� ����
    [SerializeField] private TMP_Text actionText; // �ʿ��� ������Ʈ
    [SerializeField] private Inventory inventory;

    private float sphereRadius = 1.0f; // ������ �Ⱦ� ��ü ĳ��Ʈ�� �ݰ�

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
        // �������� ����
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
        // ��ü ĳ��Ʈ ���� ����
        Gizmos.color = Color.yellow;

        // ĳ���Ͱ� �ٶ󺸴� �������� ��ü�� �׸��� ���� ��ġ�� ���� ���
        Vector3 direction = transform.TransformDirection(Vector3.forward);

        // SphereCast ������ �ð�ȭ
        Gizmos.DrawWireSphere(transform.position + direction * range, sphereRadius);
    }

    private void ItemInfoAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " ȹ�� " + "<color=yellow>" + "(E)" + "</color>";
    }

    private void ItemInfoDisappear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
    }
}
