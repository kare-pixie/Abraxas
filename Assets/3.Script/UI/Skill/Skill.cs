using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/skill")]
public class Skill : ScriptableObject
{
    public Sprite skillImage; // ��ų �̹���
    public string skillName; // ��ų �̸�
    public string skillExplain; //��ų ����
    public int skillLevel; // ��ų ����
    public string anim; // �ִϸ��̼� �̸�
}
