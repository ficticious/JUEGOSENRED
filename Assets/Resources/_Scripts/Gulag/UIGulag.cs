using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGulag : MonoBehaviour
{
    public static UIGulag Instance;

    public GameObject gulagPanel;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowGulagUI(bool state)
    {
        gulagPanel.SetActive(state);
    }
}
