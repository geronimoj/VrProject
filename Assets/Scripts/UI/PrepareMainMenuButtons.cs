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
            if (m_exit)
                m_exit.onClick.AddListener(_manager.ExitApp);

        if (_wave)
        {
            if (m_tutorial)
                m_tutorial.onClick.AddListener(() => _wave.LoadLevel("TutorialLevel"));

            if (m_play)
                m_play.onClick.AddListener(() => _wave.LoadLevel("Level1"));
        }
    }
}
