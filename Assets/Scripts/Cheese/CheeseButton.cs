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

    private Material _skyboxMat = null;

    public Button button = null;

    void Start()
    {
        _skybox = GetComponent<TransitionSkybox>();
        _skyboxMat = _skybox.skyboxMaterial;
        //Setup the on click event
        if (button)
            button.onClick.AddListener(OnPressButton);
    }

    private void OnPressButton()
    {   //If we are missing any components, don't do anything
        if (!(m_cheese && _skybox && _skyboxMat))
            return;
        //Make sure the skybox is fulling showing one or the other image
        float offset = _skyboxMat.GetFloat("_Offset");
        //Make sure the offset is finished basically
        if (offset > -0.99 && offset < 0.99)
            return;
        //If true, change top image
        bool changeTop = offset >= 0.99;

        if (changeTop)
            _skyboxMat.SetTexture("_CubeTop", m_cheese);
        else
            _skyboxMat.SetTexture("_CubeBot", m_cheese);
        //Swap the skybox
        _skybox.Toggle();
        //Remove us from the button so you can't remove it
        button.onClick.RemoveListener(OnPressButton);
    }
}
