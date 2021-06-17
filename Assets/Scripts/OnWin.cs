using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnWin : MonoBehaviour
{
    public UnityEvent onWin;

    void Start()
    {
        gameObject.SetActive(false);
        GameManager.s_instance.OnWin.AddListener(Win);
    }

    private void Win()
    {
        onWin.Invoke();
    }
}
