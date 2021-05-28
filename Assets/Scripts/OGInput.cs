using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OGInput
{
    /// <summary>
    /// The input on the oculus
    /// </summary>
    [Tooltip("The input equivalent on the oculus")]
    [SerializeField]
    protected OGInputGetter.OculusInputs _oculusInput = OGInputGetter.OculusInputs.BackTrigger;

    public OGInputGetter.OculusInputs OculusInput => _oculusInput;
#if UNITY_EDITOR
    /// <summary>
    /// The input in editor
    /// </summary>
    [Tooltip("The input equivalent for playing in the inspector")]
    [SerializeField]
    protected KeyCode _editorInput = KeyCode.Mouse0;
#endif
    private float prevInput = 0;
    private float curInput = 0;
    /// <summary>
    /// Returns true when the button is pressed
    /// </summary>
    /// <returns>Returns true when the button is pressed</returns>
    public bool Down()
    {   //If the previous input was nothing but the current input is something
        if (prevInput == 0 && curInput != 0)
            return true;
        return false;
    }
    /// <summary>
    /// Returns true when the button is released
    /// </summary>
    /// <returns>Returns true when the button is released</returns>
    public bool Up()
    {
        if (prevInput != 0 && curInput == 0)
            return true;
        return false;
    }
    /// <summary>
    /// Returns the value of the input
    /// </summary>
    /// <returns>Returns the value of the input</returns>
    public float Get()
    {
        return curInput;
    }
    /// <summary>
    /// Call regularly to update the inputs
    /// </summary>
    public void UpdateInput()
    {
        prevInput = curInput;

#if UNITY_EDITOR
        curInput = Input.GetKeyDown(_editorInput) ? 1 : 0;
        //Get the input from the correct button
        switch(_oculusInput)
        {
            case OGInputGetter.OculusInputs.BackTrigger:
                curInput = Input.GetKey(KeyCode.Joystick1Button15) ? 1 : 0;
                break;
            case OGInputGetter.OculusInputs.FaceButton:
                curInput = Input.GetKey(KeyCode.Joystick1Button9) ? 1 : 0;
                break;
            case OGInputGetter.OculusInputs.ReturnButton:
                curInput = Input.GetKey(KeyCode.Joystick1Button7) ? 1 : 0;
                break;
            case OGInputGetter.OculusInputs.FaceTouch:
                curInput = Input.GetKey(KeyCode.JoystickButton17) ? 1 : 0;
                break;
        }
#endif
    }
}
