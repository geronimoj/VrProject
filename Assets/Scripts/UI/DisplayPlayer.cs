using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Displays player related information
/// </summary>
public class DisplayPlayer : MonoBehaviour
{
    /// <summary>
    /// The players ship
    /// </summary>
    private PlayerShip _player = null;
    /// <summary>
    /// The text to display the players score
    /// </summary>
    public TextMeshProUGUI m_text = null;
    /// <summary>
    /// The text to appear before the score
    /// </summary>
    public string prefix = "Score: ";
    /// <summary>
    /// The text to appear after the score
    /// </summary>
    public string suffix = "";
    /// <summary>
    /// Sets the Text to display the players score
    /// </summary>
    void Start()
    {   //Attempt to get a reference to the player if we don't
        if (!_player)
            _player = PlayerShip.s_instance;
        //Update the UI
        UpdateUI();
    }
    /// <summary>
    /// Updates the UI
    /// </summary>
    public void UpdateUI()
    {   //If we don't have a player return
        if (!_player)
            return;
        //Set the players text
        if (m_text)
            m_text.text = prefix + _player.m_currentScore + suffix;
    }
}
