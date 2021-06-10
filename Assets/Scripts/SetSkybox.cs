using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Sets the skybox on start
/// </summary>
public class SetSkybox : MonoBehaviour
{
    /// <summary>
    /// The material for the skybox
    /// </summary>
    public Material skybox;
    /// <summary>
    /// Sets the skybox
    /// </summary>
    void Start()
    {
        RenderSettings.skybox = skybox;
    }
    //When in the editor, keeps setting the skybox
#if UNITY_EDITOR
    /// <summary>
    /// Continually sets the skybox for debugging purposes
    /// </summary>
    void Update()
    {
        RenderSettings.skybox = skybox;
    }
#endif
}
