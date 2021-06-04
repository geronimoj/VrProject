using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Wave : MonoBehaviour
{
    public static bool paused = false;

    public float spawnTime;

    public string path = "";
    public string formation = "";

    [SerializeField]
    private bool sequential = false;

    [SerializeField]
    float spacing = 1;

    bool spawned = false;

    float timer = 0;

    [HideInInspector]
    public BezPath pathComp;
    [HideInInspector]
    public Formation formComp;

    GameObject FormationManager;
    GameObject PathManager;

    [SerializeField]
    Vector3 position;

        [SerializeField]
    public bool visible = true;

    public void Start()
    {
        if (!PathManager)
            PathManager = GameObject.Find("PathManager");
        LookUpPath();

        if (!FormationManager)
            FormationManager = GameObject.Find("FormationManager");
        LookUpFormation();
    }

    public void Update()
    {
        if (Application.isPlaying)
        {   //Don't increment the timers if the waves are paused.
            if (paused)
                return;
            timer += Time.deltaTime;
            if (!spawned && timer > spawnTime)
                Spawn();
        }
        else
        {
            LookUpFormation();
            LookUpPath();
        }
    }

    private void LookUpFormation()
    {
        formComp = null;
        Formation[] formationList = FormationManager.GetComponents<Formation>();
        for (int i = 0; i < formationList.Length; i++)
        {
            if (formationList[i].formation == formation)
            {
                formComp = formationList[i];
                break;
            }
        }

        if (!formComp)
            Debug.LogError("Formation not found!");
    }

    private void LookUpPath()
    {
        pathComp = null;
        BezPath[] pathList = PathManager.GetComponents<BezPath>();
        for (int i = 0; i < pathList.Length; i++)
        {
            if (pathList[i].pathName == path)
            {
                pathComp = pathList[i];
                break;
            }
        }

        if (!pathComp)
            Debug.LogError("Path not found!");
    }

    public void Spawn()
    {
        List<GameObject> spawnList = formComp.Spawn(position);

        float delay = 0;
        foreach (GameObject spawn in spawnList)
        {
            BezRunner runner = spawn.GetComponent<BezRunner>();
            runner.path = pathComp;

            if (sequential)
            {
                SpawnDelay d = spawn.AddComponent<SpawnDelay>();
                d.delay = delay;
                delay += spacing;
            }
        }

        spawned = true;
    }
}
