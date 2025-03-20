using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Title;
    [SerializeField] TextMeshProUGUI Description;
   
    
    public void ShowPopup(string title, string description)
    {
        Title.text = title;
        Description.text = description+Environment.NewLine;
        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void HidePopup()
    {
        Title.text = "";
        Description.text = "";
        gameObject.SetActive(false);
    }
}