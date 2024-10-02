using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveCheck : MonoBehaviour
{
    [SerializeField] private GameObject saveCheck;

    public void CloseSaveCheck()
    {
        saveCheck.SetActive(false);
    }
    public void OpenSvaeCheck()
    {
        saveCheck.SetActive(true);
    }
}
