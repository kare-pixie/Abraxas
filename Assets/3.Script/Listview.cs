using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Listview : MonoBehaviour
{
    [SerializeField] private Listview listview;
    [SerializeField] private GameObject text;
    
    public void Click()
    {
        GameObject newtext = Instantiate(text, transform);
    }
}
