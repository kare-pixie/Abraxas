using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skills : MonoBehaviour
{
    private bool skillActivated = false;

    [SerializeField] private GameObject skills;
    [SerializeField] private List<Skill> skillList;
    [SerializeField] private SkillSlot skillSlot;
    [SerializeField] private GameObject content;
    private void Awake()
    {
        for (int i = 0; i < skillList.Count; i++)
        {
            SkillSlot p = Instantiate(skillSlot);
            p.skill = skillList[i];
            p.transform.SetParent(content.transform);
            p.Init();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TryOpenSkills();
        }
    }
    public void TryOpenSkills()
    {
        skillActivated = !skillActivated;

        if (skillActivated)
            OpenSkills();
        else
            CloseSkills();
    }

    private void OpenSkills()
    {
        skills.SetActive(true);

        // 이 오브젝트를 부모 계층에서 가장 마지막 자식으로 이동시켜 가장 위에 표시
        skills.transform.SetAsLastSibling();
    }
    private void CloseSkills()
    {
        skills.SetActive(false);
    }
}
