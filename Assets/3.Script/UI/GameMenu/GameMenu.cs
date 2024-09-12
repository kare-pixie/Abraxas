using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    private bool ganeMenuActivated = false;
    [SerializeField] private GameObject ganeMenu;
    public void TryOpenGameMenu()
    {
        ganeMenuActivated = !ganeMenuActivated;

        if (ganeMenuActivated)
            OpenEquip();
        else
            CloseEquip();
    }

    private void OpenEquip()
    {
        ganeMenu.SetActive(true);

        // 이 오브젝트를 부모 계층에서 가장 마지막 자식으로 이동시켜 가장 위에 표시
        ganeMenu.transform.SetAsLastSibling();
    }
    private void CloseEquip()
    {
        ganeMenu.SetActive(false);
    }
}
