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

        // �� ������Ʈ�� �θ� �������� ���� ������ �ڽ����� �̵����� ���� ���� ǥ��
        skills.transform.SetAsLastSibling();
    }
    private void CloseSkills()
    {
        skills.SetActive(false);
    }
}
