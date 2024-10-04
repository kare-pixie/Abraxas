using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public SkillBarSlot[] SkillSlot;

    [SerializeField] private SaveSkillSlot saveSkillSlot;

    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.dataPath, "skillSlot.json");
    }


    private void Start()
    {
        LoadSkillSlot();
    }

    private void LoadSkillSlot()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath); // 파일에서 JSON 읽기
            JsonUtility.FromJsonOverwrite(json, saveSkillSlot); // 읽어온 데이터를 객체에 덮어쓰기
        }

        if (saveSkillSlot != null)
        {
            for (int i = 0; i < saveSkillSlot.skill.Length; i++)
            {
                if (saveSkillSlot.skill[i] != null)
                {
                    SkillSlot[i].SetSlot(saveSkillSlot.skill[i]);
                }
            }
        }
    }
    public void SaveSkillSlot()
    {
        if (saveSkillSlot != null)
        {
            for (int i = 0; i < saveSkillSlot.skill.Length; i++)
            {
                saveSkillSlot.skill[i] = SkillSlot[i].skill;
            }
        }

        string json = JsonUtility.ToJson(saveSkillSlot, true);
        File.WriteAllText(saveFilePath, json);
    }

    private void Update()
    {
        if (UIManager.instance != null && UIManager.instance.isGameOver) return;

        if (Input.GetKeyDown(KeyCode.F1))
        {
            SkillSlot[0].UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SkillSlot[1].UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            SkillSlot[2].UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            SkillSlot[3].UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SkillSlot[4].UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            SkillSlot[5].UseSkill();
        }
    }
}
