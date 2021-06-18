using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sans : MonoBehaviour
{
    public AudioSource ringtone;
    public AudioSource voice;
    public float maxTimer;
    private float currentTimer;
    // Start is called before the first frame update
    void Start()
    {
        ResetObject();
    }

    // Update is called once per frame
    void Update()
    {
        currentTimer -= Time.deltaTime;

        if (currentTimer <= 0)
            gameObject.SetActive(false);
    }

    private void ResetObject()
    {
        currentTimer = maxTimer;
    }

    public void Decline()
    {
        ringtone.Stop();
        gameObject.SetActive(false);
    }

    public void Accept()
    {
        ringtone.Stop();
        voice.Play();
    }
}
