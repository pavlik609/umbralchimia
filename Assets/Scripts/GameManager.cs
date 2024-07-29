using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.CustomPlugins;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager _gm;
    public GameObject gameobject_mat_in_hand;
    public GameObject current_arrow;
    [HideInInspector] public GameObject[] all_arrows = new GameObject[99];
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
    public bool[] clue_conditions;
    public bool has_material_in_hand;
    public AudioClip snd_unlocked;
    public Sprite[] vial_anims;
    private bool first_tooltip;
    private bool corutine_alpha_already_running = false;
    private AudioSource src;
     public Material material_in_hand;
     public int[] all_arrow_destinations = new int[99];
     public Material[] table = new Material[99];
    [HideInInspector] public bool setting_movement;
    [HideInInspector] public bool first_setting_movement;
    [HideInInspector] public bool use_tooltip;
    [HideInInspector] public bool right_click;
    [HideInInspector] public Vector3 setting_movement_origin;
    [HideInInspector] public string tt_text;
    // Start is called before the first frame update
    void Start()
    {
        clue_conditions = new bool[7];
        src = GetComponent<AudioSource>();
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
        tooltip.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        tooltip.transform.position = new Vector3(tooltip.transform.position.x, tooltip.transform.position.y, 0);
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
                gameobject_mat_in_hand.transform.localScale = Vector3.one;
                gameobject_mat_in_hand.transform.SetAsLastSibling();
                has_material_in_hand = true;
            }
            gameobject_mat_in_hand.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gameobject_mat_in_hand.transform.position = new Vector3(gameobject_mat_in_hand.transform.position.x, gameobject_mat_in_hand.transform.position.y, 0);
        }
        if (setting_movement == true && first_setting_movement == false && all_arrows[holding_index] == null)
        {
            current_arrow = Instantiate(arrow_prefab, setting_movement_origin,Quaternion.identity);
            all_arrows[holding_index] = current_arrow;
            current_arrow.GetComponent<RectTransform>().pivot = new Vector3(0, 0.5f, 0);
            current_arrow.transform.SetParent(GameObject.Find("Game").GetComponent<Transform>());
            current_arrow.transform.localScale = Vector3.one;
            current_arrow.transform.SetAsLastSibling();
            current_arrow.transform.SetSiblingIndex(current_arrow.transform.GetSiblingIndex() - 5);
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
            rt.sizeDelta = new Vector2(Vector3.Distance(Camera.main.WorldToScreenPoint(setting_movement_origin), Input.mousePosition), 3);
            Transform head = current_arrow.transform.Find("arrow_head");
            current_arrow.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2(current_arrow.transform.position.y - Camera.main.ScreenToWorldPoint(Input.mousePosition).y, current_arrow.transform.position.x - Camera.main.ScreenToWorldPoint(Input.mousePosition).x) * Mathf.Rad2Deg+180));
            head.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            head.position = new Vector3(head.position.x, head.position.y, 0);
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
            for (int j = 0; j < Mathf.Pow(gridSize,2); j++){
                if (loc_recip[j] != table[j]) { tables_same = false;  }
                if (loc_arr[j] != all_arrow_destinations[j]) { tables_same = false;  }
            }
            if (tables_same == true && !unlocked_materials[recipies[i].reward_idx]) { unlocked_materials[recipies[i].reward_idx] = true;
                new_element.transform.DOScaleX(1, 1).SetEase(Ease.OutExpo);
                src.PlayOneShot(snd_unlocked, 0.1f);
                element_unlocked_text.text = materials[recipies[i].reward_idx + 1]._name;
                new_element.transform.Find("Element_Img").GetComponent<Image>().sprite = Material_imgs[recipies[i].reward_idx + 1];
                StartCoroutine(DelayedNewElement_Corutine()); }
            else if (tables_same && unlocked_materials[recipies[i].reward_idx] == true) {
                element_allreadyunlocked.GetComponent<CanvasGroup>().alpha = 1;
                if (!corutine_alpha_already_running)
                {
                    DOTween.Kill(element_allreadyunlocked.GetComponent<CanvasGroup>());
                    element_allreadyunlocked.GetComponent<CanvasGroup>().alpha = 1;
                    element_allreadyunlocked.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.InSine);
                }
            } 
        }
    }
    public IEnumerator DelayedNewElement_Corutine()
    {
        yield return new WaitForSeconds(1f);
        new_element.transform.DOScaleX(0, 1).SetEase(Ease.InExpo);
    }
    public IEnumerator DelayedHideCandle_Corutine(bool condition,GameObject gamobj)
    {
        yield return new WaitForSeconds(0.75f);
        gamobj.SetActive(condition);
    }
    public void MoveToClues()
    {
        game.transform.DOMoveX(game.transform.position.x+12.3f, 1, false).SetEase(Ease.InOutSine);
        clues.transform.DOMoveX(clues.transform.position.x + 12.3f, 1, false).SetEase(Ease.InOutSine);
    }
    public void MoveToStation()
    {
        game.transform.DOMoveX(game.transform.position.x - 12.3f, 1, false).SetEase(Ease.InOutSine);
        clues.transform.DOMoveX(clues.transform.position.x - 12.3f, 1, false).SetEase(Ease.InOutSine);
    }
    public void ScaleClue(GameObject clue)
    {
        clue.transform.DOScaleY(1, 0.75f).SetEase(Ease.OutExpo);
        StartCoroutine(DelayedHideCandle_Corutine(true,clue.transform.Find("candle").transform.Find("Light2D").gameObject));
    }
    public void ScaleDownClue(GameObject clue)
    {
        clue.transform.DOScaleY(0, 0.75f).SetEase(Ease.OutExpo);
        StartCoroutine(DelayedHideCandle_Corutine(false, clue.transform.Find("candle").transform.Find("Light2D").gameObject));
    }
    public void ScaleX(GameObject gam)
    {
        gam.transform.DOScaleX(1, 0.75f).SetEase(Ease.OutExpo);
    }
    public void ScaleDownX(GameObject gam)
    {
        gam.transform.DOScaleX(0, 0.75f).SetEase(Ease.OutExpo);
    }
    public AudioClip randAudio(AudioClip[] clips)
    {
        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }
    public void playRandSFX(AudioClip[] clips)
    {
        MenuManager._mm.src.PlayOneShot(randAudio(clips),MenuManager._mm.master_volume* MenuManager._mm.sfx_volume);
    }
}
