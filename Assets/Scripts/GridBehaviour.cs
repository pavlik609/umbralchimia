using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridBehaviour : MonoBehaviour
{
    public GridLayoutGroup layout;
    public Transform[] children;
    public Transform[] children_current;
    void Start()
    {
        layout = GetComponent<GridLayoutGroup>();
        children = GetComponentsInChildren<Transform>();
        foreach (Transform t in children)
        {
            if (t.gameObject.name != "BasePedestal" && t != transform)
            {
                t.gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        children_current = GetComponentsInChildren<Transform>();
        layout.constraintCount = GameManager._gm.gridSize;
        foreach (Transform t in children)
        {
            CheckForUpgrade(t, 1);
            CheckForUpgrade(t, 2);
            CheckForUpgrade(t, 3);
            CheckForUpgrade(t, 4);
        }
    }

    private void CheckForUpgrade(Transform t, int num)
    {
        if (t.gameObject.name == "Upgrade" + num + "Pedestal" && GameManager._gm.gridProgression >= num)
        {
            t.gameObject.SetActive(true);
        }
    }
}
