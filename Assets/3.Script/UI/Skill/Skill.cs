using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/skill")]
public class Skill : ScriptableObject
{
    public Sprite skillImage; // 스킬 이미지
    public string skillName; // 스킬 이름
    public string skillExplain; //스킬 설명
    public int skillLevel; // 스킬 레벨
    public string anim; // 애니메이션 이름
}
