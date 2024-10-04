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
    [SerializeField] private TMP_Text textLevel;

    [SerializeField] private GameObject gameOver;
    public bool isGameOver { get; private set; }

    private PlayerStatus PlayerStatus;
    private Inventory inventory;
    private int level;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            PlayerStatus = FindObjectOfType<PlayerStatus>();
            inventory = FindObjectOfType<Inventory>();
            level = 0;
            isGameOver = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        gameOver.SetActive(true);
    }

    private void AddLog(string str)
    {
        TMP_Text log = Instantiate(logText);
        log.text = str;
        log.transform.SetParent(logGroup);
    }
    public void SkillLog(string skillName, int skillMana)
    {
        AddLog($"{skillName}�� ����߽��ϴ�.");
        PlayerStatus.AddCurMp(-skillMana);
    }

    public void ManaZeroLog()
    {
        AddLog($"MP�� ���� ��ų�� ����� �� �����ϴ�.");
    }
    public void ItemLog(string item, int itemCount = 1)
    {
        AddLog($"{item} {itemCount}���� ȹ���߽��ϴ�.");
        inventory.RefreshPotion();
    }
    public void ExpLog(int exp)
    {
        AddLog($"{exp} ����ġ�� ȹ���߽��ϴ�.");
        PlayerStatus.AddExp(exp);
    }
    public void EnemyLog(string name)
    {
        AddLog($"{name}�� ����Ʈ�Ƚ��ϴ�.");
    }
    public void UseLog(string item)
    {
        switch(item)
        {
            case "HP ����":
                PlayerStatus.AddCurHP(50); 
                break;
            case "MP ����": 
                PlayerStatus.AddCurMp(50);
                break;
        }
        AddLog($"{item}�� ����߽��ϴ�.");
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
        level = (int)(exp / 100) + 1;

        textLevel.text = level.ToString();

        sliderEXP.value = exp / 100;
        sliderStatusEXP.value = exp / 100;
        textExp.text = $"{exp}%";
        textStatusExp.text = $"Lv{level} exp {exp}%";
    }
}
