using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private float maxHp;
    private float curHp;
    private float maxMp;
    private float curMp;
    private float exp;

    private Material mat;
    private Animator animator;
    private int animIDDie;

    [SerializeField] private Status status;

    public void SetHp(float max, float cur)
    {
        maxHp = max;
        curHp = cur;
        UIManager.instance.SetHP(maxHp, curHp);
    }
    public void AddMaxHp(float hp)
    {
        maxHp += hp;
        curHp += hp;
        UIManager.instance.SetHP(maxHp, curHp);
    }
    public void AddCurHP(float hp)
    {
        if(curHp + hp > maxHp)
        {
            curHp = maxHp;
        }
        else
        {
            curHp += hp;
        }
        UIManager.instance.SetHP(maxHp, curHp);
    }
    public void SetMp(float max, float cur)
    {
        maxMp = max;
        curMp = cur;
        UIManager.instance.SetMP(maxMp, curMp);
    }
    public void AddMaxMp(float mp)
    {
        maxMp += mp;
        curMp += mp;
        UIManager.instance.SetMP(maxMp, curMp);
    }
    public void AddCurMp(float mp)
    {
        if (curMp + mp > maxMp)
        {
            curMp = maxMp;
        }
        else
        {
            curMp += mp;
        }
        UIManager.instance.SetMP(maxMp, curMp);
    }
    public void SetExp(float exp)
    {
        this.exp = exp;
        UIManager.instance.SetEXP(this.exp);
    }
    public void AddExp(float exp)
    {
        this.exp += exp;
        UIManager.instance.SetEXP(this.exp);
    }

    private void Awake()
    {
        TryGetComponent(out animator);
        mat = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material;

        animIDDie = Animator.StringToHash("Death1");
    }
    private void Start()
    {
        SetHp(status.maxHp, status.curHp);
        SetMp(status.maxMp, status.curMp);
        SetExp(status.exp);
        transform.position = status.location;
        transform.rotation = status.rotation;
    }

    private IEnumerator OnDamage(float sec)
    {
        yield return new WaitForSeconds(sec);
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHp > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            animator.SetTrigger(animIDDie);
            mat.color = Color.gray;
        }
        if (curHp <= 0)
        {
            UIManager.instance.GameOver();
        }
    }
    public void TakeDamage(int damage, float sec)
    {
        if (curHp <= 0 || damage <= 0) // ¹«Àû
            return;

        if (curHp - damage <= 0)
        {
            curHp = 0;
            UIManager.instance.SetHP(maxHp, curHp);
        }
        else
        {
            curHp -= damage;
            UIManager.instance.SetHP(maxHp, curHp);
        }
        StartCoroutine(OnDamage(sec));
    }

    public bool CheckManaZero(int skillMana)
    {
        if (curMp - skillMana <= 0)
        {
            return true;
        }
        return false;
    }
    public void SaveStatus()
    {
        status.maxHp = maxHp;
        status.curHp = curHp;
        status.maxMp = maxMp;
        status.curMp = curMp;
        status.exp = exp;
        status.location = transform.position;
        status.rotation = transform.rotation;
    }
}
