using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Message : MonoBehaviour
{
    public TextMeshProUGUI MyMessage;

    void Start()
    {
        MyMessage.color = Color.black;
        GetComponent<RectTransform>().SetAsFirstSibling();

        //MyMessage.gameObject.SetActive(false);
    }

    
}
