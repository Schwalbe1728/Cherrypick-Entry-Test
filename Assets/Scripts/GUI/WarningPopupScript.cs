using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningPopupScript : MonoBehaviour
{
    [SerializeField]
    private Text dialogText;

    [SerializeField]
    private GameObject dialogObject;

    [SerializeField]
    private Image panelSeparator;

    private Color separatorColorBackup;

    public void OpenPopup(string message)
    {
        PopupToggle(true);
        dialogText.text = message;
    }

    public void ClosePopup()
    {
        PopupToggle(false);  
    }

	// Use this for initialization
	void Start ()
    {
        if (panelSeparator != null)
        {
            separatorColorBackup = panelSeparator.color;
        }

        dialogObject.SetActive(false);
        ClosePopup();
	}   
    
    private void PopupToggle(bool value)
    {
        dialogObject.SetActive(value);
        panelSeparator.raycastTarget = value;
        panelSeparator.color = (value) ? separatorColorBackup : Color.clear;
    } 
}
