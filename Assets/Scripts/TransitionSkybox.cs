using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Used for transitioning the skybox from one thing to another
/// </summary>
public class TransitionSkybox : MonoBehaviour
{
    /// <summary>
    /// The skybox material
    /// </summary>
    [Tooltip("The material used for the skybox")]
    public Material skyboxMaterial = null;
    /// <summary>
    /// Is it scrolling up or down
    /// </summary>
    [SerializeField]
    private bool displayTop = false;
    /// <summary>
    /// Should the skybox be instantly set to the value
    /// </summary>
    [SerializeField]
    private bool _setInstantly = false;
    /// <summary>
    /// Timer for the transition
    /// </summary>
    private float t_timer = 0;
    /// <summary>
    /// The duration of the transition
    /// </summary>
    [Tooltip("The duration of the transition")]
    public float _transitionDuration = 0;
    /// <summary>
    /// Sets the timer correct
    /// </summary>
    private void Start()
    {   //-1 to 1 range
        float transitionValue = skyboxMaterial.GetFloat("_Offset");
        //0 - 1 range
        transitionValue = (transitionValue + 1) / 2;
        if (!_setInstantly)
            //Set it to sweep from the other value
            t_timer = displayTop ? _transitionDuration * transitionValue : 0;
        else
            //Set it to already be finished
            t_timer = !displayTop ? _transitionDuration : 0;
    }
    /// <summary>
    /// Update the skybox
    /// </summary>
    void Update()
    {   //Make sure we have a material
        if (!skyboxMaterial)
            return;
        //Change the timer correctly
        if (displayTop)
            t_timer -= Time.deltaTime;
        else
            t_timer += Time.deltaTime;
        //Clamp the timer
        t_timer = Mathf.Clamp(t_timer, 0, _transitionDuration);
        //Get the value of the transition
        float value = Mathf.Clamp01(t_timer / _transitionDuration);
        //0 - 2 range
        value *= 2;
        //-1 - 1 range
        value -= 1;
        //Set the value of the skybox
        skyboxMaterial.SetFloat("_Offset", value);
    }
    /// <summary>
    /// Swap the transition from top to bottom and bottom to top
    /// </summary>
    /// <param name="top">The scroll to the top</param>
    public void Transition(bool top)
    {
        displayTop = top;
    }

    public void Toggle()
    {
        displayTop = !displayTop;
    }
}
