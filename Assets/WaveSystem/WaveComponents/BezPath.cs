using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezPath : MonoBehaviour
{
    public string pathName = "";

    [SerializeField]
    public List<Node> nodes = new List<Node>(1);

    [HideInInspector]
    public Vector4[] points = new Vector4[100];

    [SerializeField]
    public bool visible = true;

    [SerializeField]
    PathType type = PathType.Standard;
    public void OnValidate()
    {
        BakePath();
    }

    Node[] FourPoints(float t)
    {
        Node[] fourPoints = new Node[4];                                            //Prep 4 node array

        int CLI = GetCenterLeftIndex(t);                                            //Get the index of the first n in nodes where t>n.t

        for (int i = 0; i < 4; i++)                                                 //For the 4 values of the array
        {
            int index = Mathf.Clamp(CLI - 1 + i, 0, nodes.Count - 1);               //Should give [CL-1,CL,CL+1,CL+2] clamped to nodes limits
            fourPoints[i] = nodes[index];                                           //Populate fourPoints with the correct nodes
        }

        return fourPoints;                                                          //Return fourPoints
    }

    List<Vector3> UsePoints(float t)
    {
        Node[] fourPoints = FourPoints(t);                                                          //Get the fourpoints

        Vector4[] fourPointsPos = ToPos(fourPoints);

        List<Vector3> usePoints = new List<Vector3>();                                              //Prep for the 3 transition points

        float transT = Mathf.InverseLerp(fourPoints[1].t, fourPoints[2].t, t);                      //Get the current transition time

        for (int i = 0; i < 3; i++)                                                                 //Populate the usePoint list
            usePoints.Add(Vector3.Lerp(fourPoints[i].geoPos, fourPoints[i + 1].geoPos, transT));    //With the transition interpolations of the fourpoints

        return usePoints;                                                                           //Report usePoints
    }

    float GetNormalizedPathTime(float t)
    {
        return Mathf.InverseLerp(nodes[0].t, nodes[nodes.Count-1].t, t);
    }

    int GetCenterLeftIndex(float t)
    {
        int index = 0;
        while (t > nodes[index].t)
            index++;

        return index-1;
    }

    public void BakePath()
    {
        if (nodes.Count == 0)
            return;
        float totalTime = nodes[nodes.Count - 1].t;
        for (int i = 0; i < points.Length; i++)
        {
            float t = i * totalTime / points.Length;
            Vector3 geoPos = CalcPos(t);
            points[i] = new Vector4(geoPos.x, geoPos.y, geoPos.z, t);
        }
    }

    public Vector3 GetPos(float time)
    {
        int i = 1;
        while (points[i].w<time && i<points.Length-1)
            i++;

        float t = Mathf.InverseLerp(points[i - 1].w, points[i].w, time);

        Vector3 pos = Vector3.Lerp(points[i - 1], points[i], t);

        return pos;
    }

    Vector4[] ToPos(Node[] pointsF)
    {
        Vector4[] pointsT = new Vector4[pointsF.Length];

        for (int i = 0; i < pointsF.Length; i++)
            pointsT[i] = pointsF[i].pos;

        return pointsT;
    }

    public float NormalizedTime(Node n)
    {
        return n.t / nodes[nodes.Count - 1].t;
    }

    public Vector3 CalcPos(float time)
    {
        Vector4[] fourPoints = ToPos(FourPoints(time));

        List<Vector3> usePoints = UsePoints(time);                                  //Get the smoothed control points

        float nPathTime = GetNormalizedPathTime(time);                            //Get the normalized time across the current base scope

        Vector3 pos = Bezier(usePoints, nPathTime);                                //Return the bezier function of those

        return pos;
    }

    private Vector3 Bezier(List<Vector3> inList, float t)
    {
        if (inList.Count == 1)
            return inList[0];

        List<Vector3> leftList = inList.GetRange(0, inList.Count - 1);
        List<Vector3> rightList = inList.GetRange(1, inList.Count - 1);

        Vector3 leftPoint = Bezier(leftList, t);
        Vector3 rightPoint = Bezier(rightList, t);

        return Vector3.Lerp(leftPoint, rightPoint, t);
    }


    public void SetGeoPos(int nodeNum, Vector3 pos)
    {
        nodes[nodeNum].geoPos = pos;
    }

    public Vector3 GetGeoPos(int nodeNum)
    {
        return nodes[nodeNum].geoPos;
    }

    [System.Serializable]
    public class Node
    {
        [SerializeField]
        private Vector4 points = Vector4.zero;

        public Vector4 pos
        {
            get => points;
            set => points = value;
        }

        public Vector3 geoPos
        {
            get => new Vector3(x, y, z);
            set => points = new Vector4 (value.x, value.y, value.z, t);
        }

        public float x
        {
            get => points.x;
        }

        public float y
        {
            get => points.y;
        }

        public float z
        {
            get => points.z;
        }

        public float t
        {
            get => points.w;
        }
    }
}

public enum PathType
{
    Standard,
    Loop,
    Pingpong
}