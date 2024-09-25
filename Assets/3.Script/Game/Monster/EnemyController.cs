using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxCollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    public void OnDamage(int damage)
    {
        if(curHealth - damage <= 0)
        {
            curHealth = 0;
            //todo: 사망로그
            Debug.Log(curHealth);
            return;
        }
        curHealth -= damage;
        Debug.Log(curHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Weapon")) //todo: 버그있음..
        {
            Weapon weapon = other.GetComponent<Weapon>();
            if(weapon.isUse)
            {
                OnDamage(other.GetComponent<Weapon>().damage);
            }
        }
    }
}
