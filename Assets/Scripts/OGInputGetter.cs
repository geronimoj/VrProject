using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGInputGetter : MonoBehaviour
{
    void Update()
    {
        
    }
    /// <summary>
    /// Returns true the frame the input was pressed
    /// </summary>
    /// <param name="input">The input to get</param>
    /// <returns>Returns true if the button was pressed</returns>
    public static bool GetDown(OculusInputs input)
    {
        switch (input)
        {
            default:
                return false;
            case OculusInputs.ReturnButton:

                break;
        }

        return false;
    }
    /// <summary>
    /// Returns true the frame the input was released
    /// </summary>
    /// <param name="input">The input to get</param>
    /// <returns>Returns true if the button was released</returns>
    public static bool GetUp(OculusInputs input)
    {
        return false;
    }
    /// <summary>
    /// Returns true if the input is bring pressed
    /// </summary>
    /// <param name="input">The input to get</param>
    /// <returns>Returns true if the button is being pressed</returns>
    public static bool Get(OculusInputs input)
    {
        return false;
    }
    /// <summary>
    /// Returns the value of the input is bring pressed
    /// </summary>
    /// <param name="input">The input to get</param>
    /// <returns>Returns 0 - 1 range based on how hard the input is pressed</returns>
    public static float GetValue(OculusInputs input)
    {
        return 0;
    }
    /// <summary>
    /// Returns the touch pad as a vector of magnitude between 0 - 1 depending on the touch position
    /// </summary>
    /// <returns>Returns Vector2.zero if no input is present</returns>
    public static Vector2 GetTouchPad()
    {
        return new Vector2();
    }
    /// <summary>
    /// Returns the value of the input being pressed in range 0 or 1
    /// </summary>
    /// <param name="input">The input to get</param>
    /// <returns>Rerturns 0 or 1 depending on if the input is pressed or not</returns>
    public static int GetValueRaw(OculusInputs input)
    {
        return 0;
    }
    /// <summary>
    /// Returns the touch pad as a vector of magnitude of 1
    /// </summary>
    /// <returns>Returns Vector2.zero if no input is present</returns>
    public static Vector2 GetTouchPadRaw()
    {
        return new Vector2();
    }
    /// <summary>
    /// The valid inputs to get
    /// </summary>
    public enum OculusInputs
    {   //The trigger
        BackTrigger = 0,
        //The button on the touch pad
        FaceButton,
        //The button with the arrow on it
        ReturnButton
    }
}
