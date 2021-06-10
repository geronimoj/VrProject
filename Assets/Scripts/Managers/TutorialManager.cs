using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Manages the tutorial
/// </summary>
public class TutorialManager : MonoBehaviour
{
    /// <summary>
    /// A stage in the tutorial
    /// </summary>
    [System.Serializable]
    public struct Stage
    {
        /// <summary>
        /// Called when this part of the tutorial starts
        /// </summary>
        public UnityEvent OnStartStage;
        /// <summary>
        /// Called when this part of the tutoiral ends
        /// </summary>
        public UnityEvent OnEndStage;
        /// <summary>
        /// The number of tasks that need to be completed to continue.
        /// </summary>
        public int _requiredNumberOfCompletedTasks;
    }
    /// <summary>
    /// The stages of the tutorial
    /// </summary>
    public Stage[] _tutorialStage = new Stage[0];
    /// <summary>
    /// The current stage of the tutorial
    /// </summary>
    private int _currentStage = 0;
    /// <summary>
    /// The tasks that have been completed
    /// </summary>
    private int _tasksCompleted = 0;
    /// <summary>
    /// Returns true when the tutorial completes
    /// </summary>
    public bool TutorialFinished => _currentStage >= _tutorialStage.Length;

    private void Start()
    {
        _tasksCompleted = int.MaxValue;
        _currentStage = -1;
    }
    /// <summary>
    /// Checks if the next stage of the tutorial has started
    /// </summary>
    private void Update()
    {   //If the tutorial has finished, don't do anything
        if (TutorialFinished)
            return;

        if (_currentStage < 0 || _tasksCompleted >= _tutorialStage[_currentStage]._requiredNumberOfCompletedTasks)
        {   //Reset the task timer
            _tasksCompleted = 0;
            //Make sure the index is valid
            if (_currentStage >= 0)
                //Call OnStageEnd
                _tutorialStage[_currentStage].OnEndStage.Invoke();
            //Move to the next stage
            _currentStage++;
            //If the tutorial has finished, return
            if (TutorialFinished)
                return;
            //Start the next stage
            _tutorialStage[_currentStage].OnStartStage.Invoke();
        }
    }
    /// <summary>
    /// States that a task has been completed
    /// </summary>
    public void CompleteTask()
    {
        _tasksCompleted++;
    }
}
