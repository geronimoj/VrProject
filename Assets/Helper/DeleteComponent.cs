// DeleteComponent.cs
using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class DeleteComponent : MonoBehaviour
{
    public Component componentReference = null;
    void Start()
    {
        if (componentReference != null)
            DestroyImmediate(componentReference);
        DestroyImmediate(this);
    }
}