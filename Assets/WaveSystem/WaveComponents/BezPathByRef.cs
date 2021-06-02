using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class BezPathByRef : MonoBehaviour
{
    [SerializeField]
    string path = "";
    [HideInInspector]
    public BezPath pathRef;

    [SerializeField]
    public bool visible = true;

    GameObject PathManager;

    public void Start()
    {
        if (!PathManager)
            PathManager = GameObject.Find("PathManager");
        LookUpPath();
    }

    public void Update()
    {
        if (!PathManager)
            PathManager = GameObject.Find("PathManager");
        LookUpPath();
    }

    public BezPath LookUpPath()
    {
        if (!PathManager)
            PathManager = GameObject.Find("PathManager");

        pathRef = null;
        BezPath[] pathList = PathManager.GetComponents<BezPath>();
        for (int i = 0; i < pathList.Length; i++)
        {
            if (pathList[i].pathName == path)
            {
                pathRef = pathList[i];
                return pathRef;
            }
        }
        return null;
    }
}
