using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrepareMainMenuButtons : MonoBehaviour
{
    private GameManager _manager;

    private WaveManager _wave;

    public Button m_exit;

    public Button m_tutorial;

    public Button m_play;

    public Button m_toMainMenu;

    private void Start()
    {
        GameObject managers = GameObject.Find("Managers");
        if (managers)
        {   //Get the gameManager
            if (!_manager)
                _manager = managers.GetComponent<GameManager>();
            //Get the wave
            if (!_wave)
                _wave = managers.GetComponent<WaveManager>();
        }

        if (_manager)
            //Setup the exit button
            if (m_exit)
                m_exit.onClick.AddListener(_manager.ExitApp);

        if (_wave)
        {   //Setup the tutorial button
            if (m_tutorial)
                m_tutorial.onClick.AddListener(() => _wave.LoadLevel("TutorialLevel"));
            //Setup the play button
            if (m_play)
                m_play.onClick.AddListener(() => _wave.LoadLevel("Level1"));
            //Setup the main menu button
            if (m_toMainMenu)
                m_toMainMenu.onClick.AddListener(() => _wave.LoadLevel("MainMenu"));
        }
    }
}
