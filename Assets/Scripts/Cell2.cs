using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Cell2 : MonoBehaviour
{
    // Cellとの差別化のため、ちと変な名前にしてみた
    public Text textForCell2;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        textForCell2.text = "クリックしたよー";
    }
}
