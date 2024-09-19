using UnityEngine;
[CreateAssetMenu(fileName = "New Status", menuName = "Status/status")]

public class Status : ScriptableObject
{
    public float hp;
    public float mp;
    public float damage;
    public float def;
    public int exp;
}