using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyOffsetToMat : MonoBehaviour
{
    /// <summary>
    /// The material to cycle
    /// </summary>
    public Material material;
    /// <summary>
    /// The offset for the material
    /// </summary>
    private Vector2 _initOffset;
    /// <summary>
    /// The speed of the cycle
    /// </summary>
    [Tooltip("The speed of the cycling")]
    public float speed;

    void Start()
    {   //Make sure we have a material
        if (!material)
        {
            Renderer r = GetComponent<Renderer>();

            if (r)
                material = r.material;
        }
        //Null catch
        if (material)
            _initOffset = material.GetTextureOffset("_MainTex");
    }

    void Update()
    {   //Null catch
        if (!material)
            return;
        //Move the offset
        _initOffset.x += speed * Time.deltaTime;
        //Apply the change
        material.SetTextureOffset("_MainTex", _initOffset);
    }
}
