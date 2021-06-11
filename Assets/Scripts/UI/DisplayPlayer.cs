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
    /// The health bar of the player
    /// </summary>
    public BarUI m_healthBar = null;
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

        if (_player)
        {
            _player.OnScoreChange.AddListener(UpdateUI);
            _player.OnTakeDamage.AddListener(UpdateUI);
        }
        else
            Debug.LogError("Could not find Player in scene. Make sure player exists before this script starts");
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
            m_text.text = prefix + _player.CurrentScore + suffix;
        //Update the health bar
        if (m_healthBar)
            m_healthBar.Percentage = _player.CurrentHealth / _player.health;
    }

    private void OnDestroy()
    {   //Remove this from OnScoreChange
        _player.OnScoreChange.RemoveListener(UpdateUI);
        _player.OnTakeDamage.RemoveListener(UpdateUI);
    }
}
