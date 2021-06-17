using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleEvents : MonoBehaviour
{
    public List<int> currentNumbers;
    public float maxTimer;
    private float currentTimer;
    public int[] sans;
    public GameObject sansPrefab;


    // Start is called before the first frame update
    void Start()
    {
        currentTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTimer > 0)
            currentTimer -= Time.deltaTime;
        else if (currentTimer < 0)
            currentTimer = 0;
    }

    public void AddNumber(int n)
    {
        currentTimer = maxTimer;
        currentNumbers.Add(n);
    }
}
