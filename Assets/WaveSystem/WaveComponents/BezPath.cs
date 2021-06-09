using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
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
    float loopDelay = 0.1f;

    [SerializeField]
    public float[] timerList = new float[0];

    [SerializeField]
    public UnityEvent[] eventList = new UnityEvent[0];
    private float TotalTime => type == PathType.Standard? nodes[nodes.Count-1].t : nodes[nodes.Count - 1].t + loopDelay;

    [SerializeField]
    PathType type = PathType.Standard;
    public void OnValidate()
    {
        BakePath();
    }

    Node[] FourPointsStandard(float t)
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

    Node[] FourPointsLoop(float t)
    {
        Node[] fourPoints = new Node[4];                                            //Prep 4 node array

        int CLI = GetLoopCenterLeftIndex(t);                                        //Get the index of the first n in nodes where t>n.t

        for (int i = 0; i < 4; i++)                                                 //For the 4 values of the array
        {
            int nLIndex = CLI - 1 + i;                                              //Store non-looped index; Need this, as we need to adjust timeval for looped nodes

            int lIndex = (nLIndex + nodes.Count) % nodes.Count;                     //Should give [CL-1,CL,CL+1,CL+2] looped

            int cycleNum = Mathf.FloorToInt((float)nLIndex / nodes.Count);                          //Get the number of cycles deep this node is

            float chrono = nodes[lIndex].t + (TotalTime*cycleNum);                   //Calculate the time for use

            fourPoints[i] = new Node(nodes[lIndex].geoPos, chrono);                 //Compile the point into the point array
        }

        return fourPoints;                                                          //Return fourPoints
    }
    List<Vector3> UsePoints(float t)
    {
        Node[] fourPoints;

        float transT = 0;

        List<Vector3> usePoints = new List<Vector3>();                                              //Prep for the 3 transition points  

        switch (type)
        {
            case PathType.Standard:
                fourPoints = FourPointsStandard(t);
                break;

            case PathType.OddLoop:
                /* To manintain bezier continuity in a loop, the 3 points must be continuous across the ends.*/
                fourPoints = FourPointsLoop(t);
                break;
            case PathType.LinearLoop:
                fourPoints = FourPointsLoop(t);
                break;
            default:
                fourPoints = new Node[0];
                break;
        }
        transT = Mathf.InverseLerp(fourPoints[1].t, fourPoints[2].t, t);

        usePoints.Add(Vector3.Lerp(fourPoints[0].geoPos, fourPoints[1].geoPos, transT));
        usePoints.Add(Vector3.Lerp(fourPoints[1].geoPos, fourPoints[2].geoPos, transT));
        usePoints.Add(Vector3.Lerp(fourPoints[2].geoPos, fourPoints[3].geoPos, transT));
        return usePoints;                                                                           //Report usePoints
    }

    float GetNormalizedPathTime(float t)
    {
        if (type == PathType.Standard)
            return Mathf.InverseLerp(0, TotalTime, t % TotalTime);
        else
            if (type == PathType.OddLoop)
            return (1 + Mathf.Sin(2 * Mathf.PI * t / TotalTime)) / 2;
        else
            if (type == PathType.LinearLoop)
            return 0;
        else
            return 0;
    }

    int GetCenterLeftIndex(float time)
    {
        int index = 0;                                      //Initialize index
        while (time >= nodes[index].t)                          //As long as the gametime is greater than the current index time
        {
            index++;                                        //Add one to the index
            if (index == nodes.Count)
                return nodes.Count-1;
            if (time < nodes[index].t)
                return index-1;
        }

        return -1;                                   
    }

    int GetLoopCenterLeftIndex(float time)
    {
        float timeInLoop = time % TotalTime;
        int index = 0;                                      //Initialize index
        while (index != nodes.Count && timeInLoop >= nodes[index].t)
            index++;

        return index-1;
    }

    public void BakePath()
    {
        if (nodes.Count == 0)
            return;
        float totalTime = TotalTime;
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
        float modTime = type == PathType.Standard ? time : time % TotalTime;

        while (points[i].w<modTime && i<points.Length-1)
            i++;
            

        float t = Mathf.InverseLerp(points[i - 1].w, points[i].w, modTime);

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
        return n.t / TotalTime;
    }

    public Vector3 CalcPos(float time)
    {
        List<Vector3> usePoints = UsePoints(time);                                  //Get the smoothed control points

        float nPathTime = GetNormalizedPathTime(time);                     //Get the normalized time across the current base scope

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
        public Node(Vector3 geo, float chrono)
        {
            geoPos = geo;
            points.w = chrono;
        }

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
    LinearLoop,
    OddLoop
}