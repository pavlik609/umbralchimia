using DG.Tweening;
using DG.Tweening.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager _gm;
    public GameObject gameobject_mat_in_hand;
    public GameObject current_arrow;
    public GameObject[] all_arrows = new GameObject[99];
    public GameObject tooltip;
    public GameObject game;
    public GameObject clues;
    public GameObject new_element;
    public GameObject arrow_prefab;
    public GameObject element_allreadyunlocked;
    private Transform tooltip_content;
    public Text tooltip_text;
    public Text element_unlocked_text;
    public Material[] materials;
    public Sprite[] Material_imgs;
    public RecipieTemplate[] recipies;
    public int gridSize;
    public int gridProgression;
    public int holding_index;
    public int crafting_size = 9;
    public bool[] unlocked_materials;
    public bool has_material_in_hand;
    private bool first_tooltip;
    private bool corutine_alpha_already_running = false;
    [HideInInspector] public Material material_in_hand;
    [HideInInspector] public int[] all_arrow_destinations = new int[99];
    [HideInInspector] public Material[] table = new Material[99];
    [HideInInspector] public bool setting_movement;
    [HideInInspector] public bool first_setting_movement;
    [HideInInspector] public bool use_tooltip;
    [HideInInspector] public bool right_click;
    [HideInInspector] public Vector3 setting_movement_origin;
    [HideInInspector] public string tt_text;
    // Start is called before the first frame update
    void Start()
    {
        use_tooltip = false;
        unlocked_materials = new bool[35];
        tooltip_content = tooltip.transform.Find("Content");
        for (int i = 0; i < 99; i++)
        {
            all_arrow_destinations[i] = 999;
        }
        table = new Material[99];
        element_unlocked_text = new_element.transform.Find("Element_BG").transform.Find("text_label").GetComponent<Text>();
        _gm = this;
        gridSize = 3;
        gridProgression = 0;
        material_in_hand = null;
        has_material_in_hand = false;
        setting_movement = false;
        setting_movement_origin = Vector3.zero;
        first_setting_movement = false;
        first_tooltip = false;
    }
    // Update is called once per frame
    void Update()
    {
        tooltip_text.text = tt_text;
        tooltip.transform.position = Input.mousePosition;
        if (use_tooltip == true && has_material_in_hand == false)
        {
            if(first_tooltip == false)
            {
                tooltip_content.GetComponentInChildren<RectTransform>().DOScaleY(1, 0.5f).SetEase(Ease.OutExpo);
                first_tooltip = true;
            }
        }
        else
        {
            if (first_tooltip == true)
            {
                tooltip_content.GetComponentInChildren<RectTransform>().DOScaleY(0, 0.5f).SetEase(Ease.OutExpo);
                first_tooltip = false;
            }
        } 
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
        if(right_click == true && has_material_in_hand == true)
        {
            material_in_hand = null;
            has_material_in_hand = false;
            Destroy(gameobject_mat_in_hand);
        }
        if (material_in_hand != null)
        {
            if (has_material_in_hand == false)
            {
                gameobject_mat_in_hand = Instantiate(material_in_hand.prefab);
                gameobject_mat_in_hand.transform.SetParent(GameObject.Find("Game").GetComponent<Transform>());
                gameobject_mat_in_hand.transform.SetAsLastSibling();
                has_material_in_hand = true;
            }
            gameobject_mat_in_hand.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (setting_movement == true && first_setting_movement == false && all_arrows[holding_index] == null)
        {
            current_arrow = Instantiate(arrow_prefab, setting_movement_origin,Quaternion.identity);
            all_arrows[holding_index] = current_arrow;
            current_arrow.GetComponent<RectTransform>().pivot = new Vector3(0, 0.5f, 0);
            current_arrow.transform.SetParent(GameObject.Find("Game").GetComponent<Transform>());
            current_arrow.transform.SetAsLastSibling();
            current_arrow.transform.position = setting_movement_origin;
            first_setting_movement = true;
        }else if(setting_movement == true && first_setting_movement == false && all_arrows[holding_index] != null)
        {
            current_arrow = all_arrows[holding_index];
            first_setting_movement = true;
        }
        else if (setting_movement == true && first_setting_movement == true)
        {
            RectTransform rt = current_arrow.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(Vector3.Distance(current_arrow.transform.position,Input.mousePosition), 3);
            Transform head = current_arrow.transform.Find("arrow_head");
            current_arrow.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2(current_arrow.transform.position.y - Input.mousePosition.y, current_arrow.transform.position.x - Input.mousePosition.x) * Mathf.Rad2Deg+180));
            head.position = Input.mousePosition;
        }
        use_tooltip = false;
    }//TOOLTIP CHANGE 4.56
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
            if (tables_same && unlocked_materials[recipies[i].reward_idx] == false) { unlocked_materials[recipies[i].reward_idx] = true;
                new_element.transform.DOScaleX(1, 1).SetEase(Ease.OutExpo);
                element_unlocked_text.text = materials[recipies[i].reward_idx + 1]._name;
                new_element.transform.Find("Element_Img").GetComponent<Image>().sprite = Material_imgs[recipies[i].reward_idx + 1];
                StartCoroutine(DelayedNewElement_Corutine()); }
            else if (unlocked_materials[recipies[i].reward_idx] == true) {
                element_allreadyunlocked.GetComponent<CanvasGroup>().alpha = 1;
                if (!corutine_alpha_already_running)
                {
                    StartCoroutine(FadeUI_Corutine());
                }
            } 
        }
    }
    public IEnumerator FadeUI_Corutine()
    {
        corutine_alpha_already_running = true;
        for (int i = 0; i < 11; i++)
        {
            yield return new WaitForSeconds(0.05f);
            element_allreadyunlocked.GetComponent<CanvasGroup>().alpha = 1-i*0.1f;
        }
        corutine_alpha_already_running = false;
    }
    public IEnumerator DelayedNewElement_Corutine()
    {
        yield return new WaitForSeconds(1f);
        new_element.transform.DOScaleX(0, 1).SetEase(Ease.InExpo);
    }
    public void MoveToClues()
    {
        game.transform.DOMoveX(920, 1, false).SetEase(Ease.InOutSine);
        clues.transform.DOMoveX(920, 1, false).SetEase(Ease.InOutSine);
    }
    public void MoveToStation()
    {
        game.transform.DOMoveX(0, 1, false).SetEase(Ease.InOutSine);
        clues.transform.DOMoveX(0, 1, false).SetEase(Ease.InOutSine);
    }
    public void ScaleClue(GameObject clue)
    {
        clue.transform.DOScaleY(1, 0.75f).SetEase(Ease.OutExpo);
    }
}
