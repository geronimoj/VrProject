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

    private Cubemap _default = null;
    private bool topChanged = false;
    private static bool s_inCheeseMode = false;

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
        //If we are in cheese mode, swap to it, otherwise revert it to the previous value
        Cubemap c = s_inCheeseMode ? _default : m_cheese;
        //If we are in cheese mode, we need to invert changeTop so we replace the correct value
        if (s_inCheeseMode)
            changeTop = !changeTop;

        if (changeTop)
        {
            _default = (Cubemap)_skyboxMat.GetTexture("_CubeTop");
            _skyboxMat.SetTexture("_CubeTop", c);
        }
        else
        {
            _default = (Cubemap)_skyboxMat.GetTexture("_CubeBot");
            _skyboxMat.SetTexture("_CubeBot", c);
        }

        s_inCheeseMode = !s_inCheeseMode;
        topChanged = changeTop;
        //Swap the skybox
        _skybox.Toggle();
        //Remove us from the button so you can't remove it
        button.onClick.RemoveListener(OnPressButton);
    }
    /// <summary>
    /// Removes the cheese mode from the skybox
    /// </summary>
    private void OnApplicationQuit()
    {   //When the application is exited, remove the cheese material
        if (topChanged)
            _skyboxMat.SetTexture("_CubeTop", _default);
        else
            _skyboxMat.SetTexture("_CubeBot", _default);
    }
    /// <summary>
    /// Removes pizza time
    /// </summary>
    private void OnDestroy()
    {   //If this is destroyed. Fix the skybox.
        OnApplicationQuit();
    }
}
