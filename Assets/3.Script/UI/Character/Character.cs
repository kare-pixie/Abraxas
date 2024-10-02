using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Character : MonoBehaviour
{
    private float hp_cur;
    private float mp_cur;

    private float hp_max;
    private float mp_max;
    private float damage;
    private float def;
    private int exp;

    [SerializeField] TMP_Text status_hp_txt;
    [SerializeField] TMP_Text character_hp_txt;

    [SerializeField] TMP_Text status_mp_txt;
    [SerializeField] TMP_Text character_mp_txt;

    [SerializeField] TMP_Text damage_txt;
    [SerializeField] TMP_Text def_txt;

    [SerializeField] TMP_Text status_exp_txt;
    [SerializeField] TMP_Text character_exp_txt;

    private bool characterActivated = false;

    [SerializeField] private GameObject character;

    private void Update()
    {
        if (UIManager.instance.isGameOver) return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            TryOpenCharacter();
        }
    }
    public void InitStatus(float _hp, float _mp, float _damage, float _def, int _exp)
    {
        hp_max = _hp;
        mp_max = _mp;
        damage = _damage;
        def = _def;
        exp = _exp;
    }
    public void TryOpenCharacter()
    {
        characterActivated = !characterActivated;

        if (characterActivated)
            OpenCharacter();
        else
            CloseCharacter();
    }

    private void OpenCharacter()
    {
        character.SetActive(true);

        // �� ������Ʈ�� �θ� �������� ���� ������ �ڽ����� �̵����� ���� ���� ǥ��
        character.transform.SetAsLastSibling();
    }
    private void CloseCharacter()
    {
        character.SetActive(false);
    }
}
