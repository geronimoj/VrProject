using UnityEngine;

[ExecuteInEditMode]
public class Manager : MonoBehaviour
{
    [HideInInspector]
    public Vector3 storedPos;

    private void OnEnable()
    {
        hideFlags = HideFlags.HideInInspector;
    }
}
