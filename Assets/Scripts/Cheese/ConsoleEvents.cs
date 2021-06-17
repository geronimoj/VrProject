using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleEvents : MonoBehaviour
{
    public List<int> currentNumbers;
    public float maxTimer;
    private float currentTimer;

    public List<string> combos;
    public List<string> events;


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
        {
            currentNumbers.Clear();
            currentTimer = 0;
        }

        if(currentNumbers.Count == 4)
        {
            CheckNumbers();
        }
    }

    public void AddNumber(int n)
    {
        currentTimer = maxTimer;
        currentNumbers.Add(n);
    }

    private void CheckNumbers()
    {
        string input = "";

        for (int i = 0; i < currentNumbers.Count; i++)
        {
            input += currentNumbers[i].ToString();
        }

        currentNumbers.Clear();

        for (int i = 0; i < combos.Count; i++)
        {
            if(combos[i] == input)
            {
                Debug.Log("Found combo " + input);
                break;
            }
        }

        
    }
}
