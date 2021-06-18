using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sans : MonoBehaviour
{
    public float ringTimer;
    private float currentRingTimer;
    public float callTimer;
    private float currentCallTimer;
    private bool calling = false;
    
    // Start is called before the first frame update
    void Start()
    {
        ResetObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (calling)
            currentCallTimer -= Time.deltaTime;
        else
            currentRingTimer -= Time.deltaTime;

        if (currentRingTimer <= 0 || currentCallTimer <= 0)
        {
            ResetObject();
            gameObject.SetActive(false);
        }
    }

    private void ResetObject()
    {
        currentRingTimer = ringTimer;
        currentCallTimer = callTimer;
    }

    public void Accept()
    {
        calling = true;
    }
}
