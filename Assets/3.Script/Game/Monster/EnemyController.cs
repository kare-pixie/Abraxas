using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public bool isinvincible;

    private Rigidbody rigid;
    private BoxCollider boxCollider;
    private Material mat;
    private Animator animator;

    private int animIDDie;

    public int damage;
    public bool isSkillUse { get; private set; }

    private void Awake()
    {
        TryGetComponent(out animator);
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material;
        isinvincible = false;

        isSkillUse = false;
        animIDDie = Animator.StringToHash("Die");
    }

    private IEnumerator OnDamage()
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            animator.SetTrigger(animIDDie);
            mat.color = Color.gray;
            Destroy(gameObject, 4);
        }
    }

    private IEnumerator OnInvincible()
    {
        yield return new WaitForSeconds(0.5f);
        isinvincible = false;
    }

    private void Damage(int damage)
    {
        if (isinvincible || curHealth <= 0) // 무적
            return;
        
        isinvincible = true;
        StartCoroutine(OnInvincible());

        if (curHealth - damage <= 0)
        {
            curHealth = 0;
            //todo: 사망로그
        }
        else
        {
            curHealth -= damage;
        }
        Debug.Log(curHealth);
        StartCoroutine(OnDamage());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Weapon"))
        {
            Weapon weapon = other.GetComponent<Weapon>();
            if(weapon.isUse)
            {
                Damage(other.GetComponent<Weapon>().damage);
            }
        }
    }

    public void SkillUse(string skillName)
    {
        isSkillUse = true;
        switch (skillName)
        {
            case "Punch":
                damage = 10; break;
            case "Swip":
                damage = 15; break;
        }
    }

    public void SkillNoUse()
    {
        isSkillUse = false;
        damage = 0;
    }
}
