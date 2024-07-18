using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
[CreateAssetMenu(fileName = "Recipie", menuName = "Assets/CreateRecipie", order = 1)]
public class RecipieTemplate : ScriptableObject
{
    public string row1;
    public string row2;
    public string row3;
    public string row4;

    public string Arrrow1;
    public string Arrrow2;
    public string Arrrow3;
    public string Arrow4;
    public int reward_idx;
    public Material StringToMaterial(string str)
    {
        switch(str)
        {
            case "Pe1":
                return GameManager._gm.materials[0];
            case "null":
                return null;
            default:
                return GameManager._gm.materials[0];
        }
    }
    public Material[] Parse()
    {
        string s_all = row1 + row2 + row3 + row4;
        string[] s_arr = s_all.Split(',');
        Material[] formatted = new Material[99];
        int persist_i = 0;
        foreach(string str in s_arr)
        {
            formatted[persist_i] = StringToMaterial(str);
            persist_i++;
        }
        return formatted;
    }
    public int[] ParseArrow()
    {
        string s_all = Arrrow1 + Arrrow2 + Arrrow3 + Arrow4;
        string[] s_arr = s_all.Split(',');
        int[] formatted = new int[99];
        int persist_i = 0;
        foreach (string str in s_arr)
        {
            int res;
            if (Int32.TryParse(str, out res) == false)
            {
                formatted[persist_i] = 999;
            }
            else
            {
                formatted[persist_i] = res;
            }
            persist_i++;
        }
        return formatted;
    }
}
