using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    static public UIManager instance = null;
    [SerializeField] private TMP_Text logText;
    [SerializeField] private Transform logGroup;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void AddLog(string _str)
    {
        TMP_Text log = Instantiate(logText);
        log.text = _str;
        log.transform.SetParent(logGroup);
    }
    public void SkillLog(string _skillName)
    {
        AddLog($"{_skillName}�� ����߽��ϴ�.");
    }
    public void ItemLog(string _item, int _itemCount = 1)
    {
        AddLog($"{_item}�� {_itemCount}�� ȹ���߽��ϴ�.");
    }
    public void UseLog(string _item)
    {
        AddLog($"{_item}�� ����߽��ϴ�."); //Todo: �� ȸ���ߴ��� ����α�
    }
}
