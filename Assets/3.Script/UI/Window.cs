using UnityEngine;
using UnityEngine.EventSystems;

public class Window : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private RectTransform rectTransform; // UI�� RectTransform ����
    private Vector2 originalPosition;    // �巡�� ���� ������ ���� ��ġ
    private Vector2 mouseOffset;         // ���콺�� UI�� �Ÿ� ������
    private void Awake()
    {
        // RectTransform ������Ʈ ��������
        rectTransform = transform.parent.GetComponent<RectTransform>();
    }

    // ���콺�� ���� �� ȣ��Ǵ� �޼���
    public void OnPointerDown(PointerEventData eventData)
    {
        // ���콺�� ��ġ�� UI ��ġ ���� �������� ���
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out mouseOffset);

        // UI�� ���� ��ġ ����
        originalPosition = rectTransform.anchoredPosition;

        // �� ������Ʈ�� �θ� �������� ���� ������ �ڽ����� �̵����� ���� ���� ǥ��
        transform.SetAsLastSibling();
    }

    // �巡�� ���� �� ȣ��Ǵ� �޼���
    public void OnDrag(PointerEventData eventData)
    {
        // �巡���ϴ� ���� ���콺 ��ġ�� ���� UI�� anchoredPosition�� ������Ʈ
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            rectTransform.anchoredPosition = localPointerPosition - mouseOffset;
        }
    }
}
