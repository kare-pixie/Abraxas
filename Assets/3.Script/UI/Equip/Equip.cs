using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Equip : MonoBehaviour
{
    private float hp_cur;
    private float mp_cur;

    private float hp_max;
    private float mp_max;
    private float damage;
    private float def;
    private int exp;

    [SerializeField] TMP_Text status_hp_txt;
    [SerializeField] TMP_Text equip_hp_txt;

    [SerializeField] TMP_Text status_mp_txt;
    [SerializeField] TMP_Text equip_mp_txt;

    [SerializeField] TMP_Text damage_txt;
    [SerializeField] TMP_Text def_txt;

    [SerializeField] TMP_Text status_exp_txt;
    [SerializeField] TMP_Text equip_exp_txt;

    public static bool EquipActivated = false;

    [SerializeField] private GameObject equip;

    public void InitStatus(float _hp, float _mp, float _damage, float _def, int _exp)
    {
        hp_max = _hp;
        mp_max = _mp;
        damage = _damage;
        def = _def;
        exp = _exp;
    }
    public void TryOpenEquip()
    {
        EquipActivated = !EquipActivated;

        if (EquipActivated)
            OpenEquip();
        else
            CloseEquip();
    }

    private void OpenEquip()
    {
        equip.SetActive(true);
    }
    private void CloseEquip()
    {
        equip.SetActive(false);
    }
}
