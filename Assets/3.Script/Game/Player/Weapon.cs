using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damage;
    public bool isUse { get; private set; }

    private void Awake()
    {
        isUse = false;
    }
    public void SkillUse(string skillName)
    {
        isUse = true;
        switch (skillName)
        {
            case "Slash1":
                damage = 10; break;
            case "Slash2":
                damage = 15; break;
        }
    }

    public void SkillNoUse()
    {
        isUse = false;
        damage = 0;
    }
}
