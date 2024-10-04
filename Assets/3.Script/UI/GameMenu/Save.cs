using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save : MonoBehaviour
{
    private SaveCheck saveCheck;
    private PlayerStatus playerStatus;
    private Inventory inventory;
    private AttackController attackController;
    private void Awake()
    {
        TryGetComponent(out saveCheck);
        playerStatus = FindObjectOfType<PlayerStatus>();
        inventory = FindObjectOfType<Inventory>();
        attackController = FindObjectOfType<AttackController>();
    }
    public void TrySave()
    {
        playerStatus.SaveStatus();
        saveCheck.OpenSvaeCheck();
        inventory.SaveInventory();
        attackController.SaveSkillSlot();
    }
}
