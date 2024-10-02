using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save : MonoBehaviour
{
    private SaveCheck saveCheck;
    private PlayerStatus playerStatus;
    private void Awake()
    {
        TryGetComponent(out saveCheck);
        playerStatus = FindObjectOfType<PlayerStatus>();
    }
    public void TrySave()
    {
        playerStatus.Save();
        saveCheck.OpenSvaeCheck();
    }
}
