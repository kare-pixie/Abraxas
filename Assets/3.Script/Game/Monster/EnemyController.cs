using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public bool isinvincible;

    private Material mat;
    private Animator animator;
    private Inventory inventory;

    private int animIDDie;

    public int damage;
    public bool isSkillUse { get; private set; }
    public string name;

    [SerializeField] private DropItem dropItem;

    private void Awake()
    {
        TryGetComponent(out animator);
        mat = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material;
        isinvincible = false;

        isSkillUse = false;
        animIDDie = Animator.StringToHash("Die");

        inventory = FindObjectOfType<Inventory>();
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
        if (isinvincible || curHealth <= 0) // ¹«Àû
            return;
        
        isinvincible = true;
        StartCoroutine(OnInvincible());

        if (curHealth - damage <= 0)
        {
            curHealth = 0;
            int itemIdx = Random.Range(0, dropItem.items.Count);
            int itemCnt = Random.Range(1, 4);
            inventory.AcquireItem(dropItem.items[itemIdx], itemCnt);

            UIManager.instance.EnemyLog(name);
            UIManager.instance.ExpLog(Random.Range(1, 5));
            UIManager.instance.ItemLog(dropItem.items[itemIdx].itemName, itemCnt);
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
