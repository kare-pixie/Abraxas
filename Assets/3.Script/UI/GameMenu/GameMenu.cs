using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    private bool ganeMenuActivated = false;
    [SerializeField] private GameObject ganeMenu;
    private void Update()
    {
        if (UIManager.instance != null && UIManager.instance.isGameOver) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TryOpenGameMenu();
        }
    }
    public void TryOpenGameMenu()
    {
        ganeMenuActivated = !ganeMenuActivated;

        if (ganeMenuActivated)
            OpenGameMenu();
        else
            CloseGameMenu();
    }

    private void OpenGameMenu()
    {
        ganeMenu.SetActive(true);

        // 이 오브젝트를 부모 계층에서 가장 마지막 자식으로 이동시켜 가장 위에 표시
        ganeMenu.transform.SetAsLastSibling();
    }
    private void CloseGameMenu()
    {
        ganeMenu.SetActive(false);
    }
}
