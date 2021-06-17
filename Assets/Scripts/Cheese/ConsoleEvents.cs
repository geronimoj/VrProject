using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConsoleEvents : MonoBehaviour
{
    /// <summary>
    /// Called for every successful combo
    /// </summary>
    [Tooltip("Called for every combo.")]
    public UnityEvent OnGetValidCombo;
    /// <summary>
    /// Called when a combo fails
    /// </summary>
    [Tooltip("Called when a combo fails")]
    public UnityEvent OnFailCombo;
    /// <summary>
    /// The current inputs
    /// </summary>
    public List<int> currentNumbers;
    public float maxTimer;
    private float currentTimer;
    /// <summary>
    /// A list of combos with their events
    /// </summary>
    public List<Code> combos;
    /// <summary>
    /// The max combo length
    /// </summary>
    private int _maxCombo = 0;


    // Start is called before the first frame update
    void Start()
    {
        currentTimer = 0;
        //Get the maximum combo size
        for (int i = 0; i < combos.Count; i++)
            //If we have a new max combo, set that to be the combo count
            if (combos[i].combo.Length > _maxCombo)
                _maxCombo = combos[i].combo.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTimer > 0)
            currentTimer -= Time.deltaTime;
        else if (currentTimer < 0)
        {
            currentNumbers.Clear();
            Debug.Log("Timer finished, Combo reset");
            //Combo failed
            OnFailCombo.Invoke();
            currentTimer = 0;
        }

        //We don't need to check the combo every frame, we only need to check if when a new number is added to the input
        //This just saves a bit of run time.
    }
    /// <summary>
    /// Call to add an input to the code
    /// </summary>
    /// <param name="n">The input to add</param>
    public void AddNumber(int n)
    {
        currentTimer = maxTimer;
        currentNumbers.Add(n);
        //Check if a combo has been met
        CheckNumbers();
        //If there are no more combos that could possibly be found, reset the input
        if (currentNumbers.Count >= _maxCombo)
        {   //Clear the input list
            currentNumbers.Clear();
            //Combo failed
            OnFailCombo.Invoke();
            Debug.Log("Combo Invalid");
        }
    }
    /// <summary>
    /// Checks if a combo has been found. If so, invokes that combo
    /// </summary>
    private void CheckNumbers()
    {
        string input = "";
        //Convert the input list to a string for easy comparison
        for (int i = 0; i < currentNumbers.Count; i++)
            input += currentNumbers[i].ToString();
        //Check against the combos
        for (int i = 0; i < combos.Count; i++)
        {   //Got a combo
            if(combos[i].combo == input)
            {
                Debug.Log("Found combo!");
                //Invoke the combo
                combos[i].OnGetCombo.Invoke();
                //They get a valid combo.
                OnGetValidCombo.Invoke();
                //If they get a combo, clear the input
                currentNumbers.Clear();
                break;
            }
        }

        
    }
    /// <summary>
    /// For storing a combo & Event
    /// </summary>
    [System.Serializable]
    public struct Code
    {
        [Tooltip("The code for this event. This is a list of numbers eg: 14223")]
        public string combo;

        [Tooltip("The events that will be played when the combo is unlocked")]
        public UnityEvent OnGetCombo;
    }
}
