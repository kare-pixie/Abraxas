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

        // �� ������Ʈ�� �θ� �������� ���� ������ �ڽ����� �̵����� ���� ���� ǥ��
        ganeMenu.transform.SetAsLastSibling();
    }
    private void CloseGameMenu()
    {
        ganeMenu.SetActive(false);
    }
}
