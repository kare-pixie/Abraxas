using UnityEngine;
[CreateAssetMenu(fileName = "New Status", menuName = "Status/status")]

public class Status : ScriptableObject
{
    public float maxHp;
    public float curHp;
    public float maxMp;
    public float curMp;
    public float damage;
    public float def;
    public float exp;
    public Vector3 location;
    public Quaternion rotation;
}