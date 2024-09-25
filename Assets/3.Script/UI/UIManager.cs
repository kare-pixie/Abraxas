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
        AddLog($"{_skillName}를 사용했습니다.");
    }
    public void ItemLog(string _item, int _itemCount = 1)
    {
        AddLog($"{_item}을 {_itemCount}개 획득했습니다.");
    }
    public void UseLog(string _item)
    {
        AddLog($"{_item}을 사용했습니다."); //Todo: 몇 회복했는지 적어두기
    }
}
