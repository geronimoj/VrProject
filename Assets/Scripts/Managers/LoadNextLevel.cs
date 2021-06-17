using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadNextLevel : MonoBehaviour
{
    /// <summary>
    /// The scene to load as a string
    /// </summary>
    [Tooltip("The name of the scene that represents the next level")]
    public string m_nextLevel;
    /// <summary>
    /// The time to wait
    /// </summary>
    [Tooltip("The time to wait to load the next scene")]
    public float m_loadTime = 5;
    /// <summary>
    /// Sets up the level to load on victory
    /// </summary>
    private void Start()
    {   //When the player wins, load the next level
        GameManager.s_instance.OnWin.AddListener(LoadLevel);
    }
    /// <summary>
    /// Loads the next level after a fixed amount of time
    /// </summary>
    private void LoadLevel()
    {
        StartCoroutine(Load());
    }
    /// <summary>
    /// Loads the next level after a fixed amount of time
    /// </summary>
    /// <returns></returns>
    private IEnumerator Load()
    {   //Wait
        yield return new WaitForSeconds(m_loadTime);
        //Load the next level
        GameManager.s_instance.LoadScene(m_nextLevel);
    }
}
