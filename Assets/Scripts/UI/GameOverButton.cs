using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverButton : MonoBehaviour
{
    void Start()
    {
        Button b = GetComponent<Button>();

        b.onClick.AddListener(() => GameManager.s_instance.LoadScene("MainMenu"));
    }
}
