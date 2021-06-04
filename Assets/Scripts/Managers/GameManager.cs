using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// A list of the enemies currently in the scene. Used to track if we should pause enemy spawning momentarily to avoid overwhelming the player
    /// </summary>
    private static readonly List<GameObject> _enemies = new List<GameObject>();
    /// <summary>
    /// Has the game finished
    /// </summary>
    private static bool _gameIsOver = false;
    /// <summary>
    /// Has the game started
    /// </summary>
    protected static bool _gameHasStarted = false;
    /// <summary>
    /// Returns true if the game has started. This is different from GameIsOver which tracks when the game ends.
    /// </summary>
    public bool GameHasStarted
    {
        get => _gameHasStarted;
        set
        {
            _gameHasStarted = value;
            //Pause or unpause the game depending on the vlaue
            Pause(!value);
        }
    }
    /// <summary>
    /// Returns true if the game has finished. This is different from GameHasStarted which tracks when the game begins
    /// </summary>
    public static bool GameIsOver => _gameIsOver;
    /// <summary>
    /// Pauses waves from spawning and enemies from moving
    /// </summary>
    /// <param name="paused">The paused stat</param>
    public static void Pause(bool paused)
    {
        Wave.paused = paused;
        BezRunner.paused = paused;
    }
    /// <summary>
    /// Pauses new waves from spawning
    /// </summary>
    /// <param name="paused">The pause state</param>
    public static void PauseSpawning(bool paused)
    {
        Wave.paused = paused;
        //Make sure the runners aren't paused
        BezRunner.paused = false;
    }
    /// <summary>
    /// Pauses the game until everything is ready
    /// </summary>
    private void Awake()
    {   //Pause the game while everything loads
        Pause(true);
    }
    /// <summary>
    /// Core game loop
    /// </summary>
    private void Update()
    {   //Wait for the game to start
        if (!_gameHasStarted || _gameIsOver)
            return;
        //Check if the game is over
        _gameIsOver = PlayerShip.PlayerIsDead;
    }
}
