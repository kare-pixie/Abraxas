using UnityEngine;
using UnityEngine.EventSystems;

public class Window : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private RectTransform rectTransform; // UI의 RectTransform 참조
    private Vector2 originalPosition;    // 드래그 시작 시점의 원래 위치
    private Vector2 mouseOffset;         // 마우스와 UI의 거리 오프셋
    private void Awake()
    {
        // RectTransform 컴포넌트 가져오기
        rectTransform = transform.parent.GetComponent<RectTransform>();
    }

    // 마우스를 누를 때 호출되는 메서드
    public void OnPointerDown(PointerEventData eventData)
    {
        // 마우스의 위치와 UI 위치 간의 오프셋을 계산
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out mouseOffset);

        // UI의 원래 위치 저장
        originalPosition = rectTransform.anchoredPosition;

        // 이 오브젝트를 부모 계층에서 가장 마지막 자식으로 이동시켜 가장 위에 표시
        transform.SetAsLastSibling();
    }

    // 드래그 중일 때 호출되는 메서드
    public void OnDrag(PointerEventData eventData)
    {
        // 드래그하는 동안 마우스 위치에 따라 UI의 anchoredPosition을 업데이트
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            rectTransform.anchoredPosition = localPointerPosition - mouseOffset;
        }
    }
}
