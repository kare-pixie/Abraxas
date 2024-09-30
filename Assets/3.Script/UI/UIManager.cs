using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    static public UIManager instance = null;
    [SerializeField] private TMP_Text logText;
    [SerializeField] private Transform logGroup;

    [SerializeField] private Slider sliderHP;
    [SerializeField] private TMP_Text textHp;
    [SerializeField] private Slider sliderStatusHP;
    [SerializeField] private TMP_Text textStatusHp;

    [SerializeField] private Slider sliderMP;
    [SerializeField] private TMP_Text textMp;
    [SerializeField] private Slider sliderStatusMP;
    [SerializeField] private TMP_Text textStatusMp;

    [SerializeField] private Slider sliderEXP;
    [SerializeField] private TMP_Text textExp;
    [SerializeField] private Slider sliderStatusEXP;
    [SerializeField] private TMP_Text textStatusExp;
    private PlayerStatus PlayerStatus;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            PlayerStatus = FindObjectOfType<PlayerStatus>();
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
        switch(_item)
        {
            case "HP 포션":
                PlayerStatus.AddCurHP(50); 
                break;
            case "MP 포션": 
                PlayerStatus.AddCurMP(50);
                break;
        }
        AddLog($"{_item}을 사용했습니다.");
    }

    public void SetHP(float max, float cur)
    {
        sliderHP.value = cur / max;
        sliderStatusHP.value = cur / max;
        textHp.text = $"{cur}/{max}";
        textStatusHp.text = $"{cur}/{max}";
    }
    public void SetMP(float max, float cur)
    {
        sliderMP.value = cur / max;
        sliderStatusMP.value = cur / max;
        textMp.text = $"{cur}/{max}";
        textStatusMp.text = $"{cur}/{max}";
    }
    public void SetEXP(float exp)
    {
        sliderEXP.value = exp / 100;
        sliderStatusEXP.value = exp / 100;
        textExp.text = $"{exp}%";
        textStatusExp.text = $"{exp}%";
    }
}
