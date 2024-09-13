using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    private bool ganeMenuActivated = false;
    [SerializeField] private GameObject ganeMenu;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TryOpenGameMenu();
        }
    }
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

        // �� ������Ʈ�� �θ� �������� ���� ������ �ڽ����� �̵����� ���� ���� ǥ��
        ganeMenu.transform.SetAsLastSibling();
    }
    private void CloseEquip()
    {
        ganeMenu.SetActive(false);
    }
}
