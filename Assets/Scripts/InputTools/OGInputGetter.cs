using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;
/// <summary>
/// Acts as a middle ground between the Inputs and code to allow for Different inputs to be used on different devices.
/// </summary>
//Run this script slightly before everything else
[DefaultExecutionOrder(-1)]
public class OGInputGetter : MonoBehaviour
{
    /// <summary>
    /// An instance of the OGInputGetter used by the other static functions for detecting inputs
    /// </summary>
    private static OGInputGetter s_instance = null;

    public OGInput m_trigger = new OGInput();

    public OGInput m_touchPadPress = new OGInput();

    public OGInput m_touchPadTouch = new OGInput();
    /// <summary>
    /// Assign the instance of the OGInputGetter
    /// </summary>
    private void Awake()
    {
        s_instance = this;
    }
    /// <summary>
    /// Updates the Inputs
    /// </summary>
    void Update()
    {   //Get the OGInputs on this
        var fields = TypeHelper.GetFieldsOfType<OGInput>(GetType());
        //Loop over the fields. We know GetFieldsOfType only returned OGInputs
        foreach (var field in fields)
        {   //Cast it to an input
            OGInput input = (OGInput)field.GetValue(this);
            //Update the input
            input.UpdateInput();
        }
    }
    /// <summary>
    /// Returns true the frame the input was pressed
    /// </summary>
    /// <param name="input">The input to get</param>
    /// <returns>Returns true if the button was pressed</returns>
    public static bool GetDown(OculusInputs input)
    {   //Get the local input of the type
        OGInput inputs = GetLocalInput(input);
        //If its not null, return the input
        if (inputs != null)
            return inputs.Down();
        //Return a fail
        return false;
    }
    /// <summary>
    /// Returns true the frame the input was released
    /// </summary>
    /// <param name="input">The input to get</param>
    /// <returns>Returns true if the button was released</returns>
    public static bool GetUp(OculusInputs input)
    {   //Get the local input of the type
        OGInput inputs = GetLocalInput(input);
        //If its not null, return the input
        if (inputs != null)
            return inputs.Up();
        //Return a fail
        return false;
    }
    /// <summary>
    /// Returns true if the input is bring pressed
    /// </summary>
    /// <param name="input">The input to get</param>
    /// <returns>Returns true if the button is being pressed</returns>
    public static bool Get(OculusInputs input)
    {//Get the local input of the type
        OGInput inputs = GetLocalInput(input);
        //If its not null, return the input
        if (inputs != null)
            return inputs.Get() != 0;
        //Return a fail
        return false;
    }
    /// <summary>
    /// Returns the value of the input is bring pressed
    /// </summary>
    /// <param name="input">The input to get</param>
    /// <returns>Returns 0 - 1 range based on how hard the input is pressed</returns>
    public static float GetValue(OculusInputs input)
    {//Get the local input of the type
        OGInput inputs = GetLocalInput(input);
        //If its not null, return the input
        if (inputs != null)
            return inputs.Get();
        //Return 0
        return 0;
    }
    /// <summary>
    /// Returns the touch pad as a vector of magnitude between 0 - 1 depending on the touch position
    /// </summary>
    /// <returns>Returns Vector2.zero if no input is present</returns>
    public static Vector2 GetTouchPad()
    {   //Use OVRInput to get the touchpad
        return OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
    }
    /// <summary>
    /// Returns the value of the input being pressed in range 0 or 1
    /// </summary>
    /// <param name="input">The input to get</param>
    /// <returns>Rerturns 0 or 1 depending on if the input is pressed or not</returns>
    public static int GetValueRaw(OculusInputs input)
    {//Get the local input of the type
        OGInput inputs = GetLocalInput(input);
        //If its not null, return the input
        if (inputs != null)
        {
            if (inputs.Get() > 0)
                return 1;
            else if (inputs.Get() < 0)
                return -1;
        }
        //Return a fail
        return 0;
    }
    /// <summary>
    /// Returns the touch pad as a vector of magnitude of 1
    /// </summary>
    /// <returns>Returns Vector2.zero if no input is present</returns>
    public static Vector2 GetTouchPadRaw()
    {   //Just use GetTouchPad but normalized
        return GetTouchPad().normalized;
    }

    private static OGInput GetLocalInput(OculusInputs input)
    {
        //We use reflection to get all of the OGInputs and use the OGInputs OculusControl to compare against input
        var fields = TypeHelper.GetFieldsOfType<OGInput>(s_instance.GetType());
        //Loop over the inputs
        foreach (var field in fields)
        {   //Cast them to a readable type
            OGInput ogInput = (OGInput)field.GetValue(s_instance);
            //Compare the input with what their input is
            if (ogInput.OculusInput == input)
                //Return the input
                return ogInput;
        }
        //Return a fail
        return null;
    }
    /// <summary>
    /// The valid inputs to get
    /// </summary>
    public enum OculusInputs
    {   //The trigger
        BackTrigger = 0,
        //The button on the touch pad
        FaceButton,
        //When the touchpad is being touched
        FaceTouch,
        //The button with the arrow on it
        ReturnButton,
    }
}
