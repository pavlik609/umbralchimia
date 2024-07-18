using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

public class ElementButtonActive : MonoBehaviour
{
    public int idx;
    private Transform bt;
    void Start()
    {
        bt = transform.Find("Button");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager._gm.unlocked_materials[0] == true)
        {
            bt.gameObject.SetActive(true);
        }
    }
}
