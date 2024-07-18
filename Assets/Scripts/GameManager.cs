using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager _gm;
    public GameObject gameobject_mat_in_hand;
    public GameObject current_arrow;
    public GameObject[] all_arrows = new GameObject[99];
    public GameObject arrow_prefab;
    public int[] all_arrow_destinations = new int[99];
    public Material material_in_hand;
    public Material[] table = new Material[99];
    public Material[] materials;
    public bool[] unlocked_materials;
    public RecipieTemplate[] recipies;
    public bool has_material_in_hand;
    public bool setting_movement;
    public bool first_setting_movement;
    public int gridSize;
    public int gridProgression;
    public int holding_index;
    public bool right_click;
    public int crafting_size = 9;
    public Vector3 setting_movement_origin;
    // Start is called before the first frame update
    void Start()
    {
        unlocked_materials = new bool[35];
        for(int i = 0; i < 99; i++)
        {
            all_arrow_destinations[i] = 999;
        }
        table = new Material[99];
        _gm = this;
        gridSize = 3;
        gridProgression = 0;
        material_in_hand = null;
        has_material_in_hand = false;
        setting_movement = false;
        setting_movement_origin = Vector3.zero;
        first_setting_movement = false;
    }
    // Update is called once per frame
    void Update()
    {
        switch (gridProgression)
        {
            case 0:
                crafting_size = 9;
                break;
            case 1:
                crafting_size = 12;
                break;
            case 2:
                crafting_size = 16;
                break;
            case 3:
                crafting_size = 20;
                break;
            case 4:
                crafting_size = 24;
                break;
            default:
                crafting_size = 9;
                break;
        }
        right_click = Input.GetMouseButtonDown(1);
        if (material_in_hand != null)
        {
            if (has_material_in_hand == false)
            {
                gameobject_mat_in_hand = Instantiate(material_in_hand.prefab);
                gameobject_mat_in_hand.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>());
                gameobject_mat_in_hand.transform.SetAsLastSibling();
                has_material_in_hand = true;
            }
            gameobject_mat_in_hand.transform.position = Input.mousePosition;
        }
        if (setting_movement == true && first_setting_movement == false && all_arrows[holding_index] == null)
        {
            current_arrow = Instantiate(arrow_prefab, setting_movement_origin,Quaternion.identity);
            all_arrows[holding_index] = current_arrow;
            current_arrow.GetComponent<RectTransform>().pivot = new Vector3(0, 0.5f, 0);
            current_arrow.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>());
            current_arrow.transform.localPosition = setting_movement_origin;
            current_arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(Vector3.Distance(current_arrow.transform.position, Input.mousePosition), 3);
            current_arrow.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2(current_arrow.transform.position.y - Input.mousePosition.y, current_arrow.transform.position.x - Input.mousePosition.x) * Mathf.Rad2Deg + 180));
            first_setting_movement = true;
        }else if(setting_movement == true && first_setting_movement == false && all_arrows[holding_index] != null)
        {
            current_arrow = all_arrows[holding_index];
            first_setting_movement = true;
        }
        else if (setting_movement == true && first_setting_movement == true)
        {
            current_arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(Vector3.Distance(current_arrow.transform.position,Input.mousePosition), 3);
            current_arrow.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2(current_arrow.transform.position.y - Input.mousePosition.y, current_arrow.transform.position.x - Input.mousePosition.x) * Mathf.Rad2Deg+180));
        }
    }
    public void Transmute()
    {
        for(int i = 0; i < recipies.Length; i++)
        {
            bool tables_same = true;
            Material[] loc_recip = recipies[i].Parse();
            int[] loc_arr = recipies[i].ParseArrow();
            for (int j = 0; j < 10; j++){
                if (loc_recip[j] != table[j]) { tables_same = false; }
                if (loc_arr[j] != all_arrow_destinations[j]) { tables_same = false; }
            }
            if (tables_same) { unlocked_materials[recipies[i].reward_idx] = true; } 
        }
    }
}
