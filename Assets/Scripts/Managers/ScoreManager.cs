using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages the players scores
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager s_instance = null;
    /// <summary>
    /// List of scores
    /// </summary>
    private readonly List<float> _scores = new List<float>();
    /// <summary>
    /// The max number of scores
    /// </summary>
    [Tooltip("The max number of scores")]
    [SerializeField]
    private uint _maxScores = 10;
    /// <summary>
    /// Load the scores to memory
    /// </summary>
    private void Awake() => Load();
    /// <summary>
    /// Sets the insance of the ScoreManager
    /// </summary>
    private void Start() => s_instance = this;
    /// <summary>
    /// Saves the scores to player prefs
    /// </summary>
    public void Save()
    {   //Loop over the stored scores
        for (int i = 0; i < _scores.Count; i++)
            PlayerPrefs.SetFloat("Score" + i, _scores[i]);
        //Save the changes to disk
        PlayerPrefs.Save();
        Debug.Log("Scores saved to disk!");
    }
    /// <summary>
    /// Loads the scores to player prefs
    /// </summary>
    public void Load()
    {   //Loop over the stored scores
        int i = 0;
        string key = "Score" + i;
        while (PlayerPrefs.HasKey(key))
        {   //Add the score. Default the value to 0 if it can't be found
            _scores.Add(PlayerPrefs.GetFloat(key, 0));
            //Update the key
            key = "Score" + i;
        }

        Debug.Log("Scores Loaded! " + i + "Scores were loaded");
    }
    /// <summary>
    /// Saves a score to the scores. This does not save the scores to memory
    /// </summary>
    /// <param name="score">The score to save</param>
    public void SaveScore(float score)
    {   //Add the score
        _scores.Add(score);
        //Sort the scores by size. This is techniqually in-optimal
        _scores.Sort(SortScores);
        //Remove the last score until we meet the minimum number of scores
        while(_scores.Count > _maxScores)
            //Remove the last score
            _scores.RemoveAt(_scores.Count - 1);
        Debug.Log("Scores saved locally.");
    }
    /// <summary>
    /// Sorts the scores such that the heighest is first
    /// </summary>
    /// <param name="a">Score A</param>
    /// <param name="b">Score B</param>
    /// <returns>Return -1 if a > b or 1 if b > a</returns>
    private int SortScores(float a, float b)
    {
        if (a > b)
            return -1;

        if (b > a)
            return 1;

        return 0;
    }
    /// <summary>
    /// Gets the scores
    /// </summary>
    /// <returns>The scores as an array</returns>
    public float[] GetScores() => _scores.ToArray();
    /// <summary>
    /// Save the scores before the program closes
    /// </summary>
    private void OnApplicationQuit()
    {
        Save();
    }
}
