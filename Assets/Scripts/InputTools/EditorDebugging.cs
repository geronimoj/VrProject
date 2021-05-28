using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EditorDebugging : MonoBehaviour
{
    public GameObject[] UI = new GameObject[0];

    public GameObject eventSystem = null;

    void Start()
    {
        if (UI != null)
        {
            foreach(GameObject ui in UI)
            {
                GraphicRaycaster gr = ui.GetComponent<GraphicRaycaster>();
                OVRRaycaster ovR = ui.GetComponent<OVRRaycaster>();

#if UNITY_EDITOR
                if (gr)
                    gr.enabled = true;

                if (ovR)
                    ovR.enabled = false;
#else
                if (gr)
                    gr.enabled = false;

                if (ovR)
                    ovR.enabled = true;
#endif
            }
        }

        if (eventSystem)
        {
            StandaloneInputModule sim = eventSystem.GetComponent<StandaloneInputModule>();
            OVRInputModule oim = eventSystem.GetComponent<OVRInputModule>();
#if UNITY_EDITOR
            if (sim)
                sim.enabled = true;

            if (oim)
                oim.enabled = false;
#else
            if (sim)
                sim.enabled = false;

            if (oim)
                oim.enabled = true;
#endif
        }
    }
}
