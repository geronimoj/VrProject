using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TransitionSkybox))]
public class CheeseButton : MonoBehaviour
{
    /// <summary>
    /// The cubemap for the cheese
    /// </summary>
    [Tooltip("The cheese cubemap")]
    [SerializeField]
    private Cubemap m_cheese = null;

    private TransitionSkybox _skybox = null;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
