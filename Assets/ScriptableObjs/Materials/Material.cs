using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Material", menuName = "Assets/CreateMaterial", order = 1)]
public class Material : ScriptableObject
{
    public GameObject prefab;
    public string _name;
}
