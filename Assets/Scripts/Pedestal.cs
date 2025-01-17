using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pedestal : MonoBehaviour
{
    public GameObject holding;
    public int index;
    private Button button;
    private RectTransform rectTransform;
    private bool once;
    // Start is called before the first frame update
    void Start()
    {
        once = false;
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
        //StartCoroutine(LateStart(0.01f));
    }
    public IEnumerator LateStart(float sec)
    {
        yield return new WaitForSeconds(sec);
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { Clicked(); } );
        index = transform.GetSiblingIndex();
    }

    // Update is called once per frame
    void OnMouseOver()
    {
        if (GameManager._gm.right_click)
        {
            Destroy(holding);
            holding = null;
            Destroy(GameManager._gm.all_arrows[index]);
            GameManager._gm.all_arrow_destinations[index] = 999;
            GameManager._gm.table[index] = null;
            GameManager._gm.all_arrows[index] = null;
            GameManager._gm.current_arrow = null;
            GameManager._gm.setting_movement = false;
            GameManager._gm.first_setting_movement = false;
            GameManager._gm.holding_index = -1;
        }
    }
    void Update()
    {
        if (!once)
        {
            button.onClick.AddListener(() => { Clicked(); });
            once = true;
        }
        index = transform.GetSiblingIndex();
        Vector2 screenmouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        index = transform.GetSiblingIndex();
        if(transform.Find("Lighting_img") != null)
        {
            transform.Find("Lighting_img").position = Camera.main.ScreenToWorldPoint(transform.position);
        }
        if (GameManager._gm.resetGridChildren)
        {
            Destroy(holding);
            holding = null;
        }
    }
    public void Clicked()
    {
        if (holding == null && GameManager._gm.gameobject_mat_in_hand != null)
        {
            holding = GameManager._gm.gameobject_mat_in_hand;
            GameManager._gm.table[index] = GameManager._gm.material_in_hand;
            GameManager._gm.gameobject_mat_in_hand = null;
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                GameManager._gm.material_in_hand = null;
            }
            GameManager._gm.has_material_in_hand = false;
            holding.transform.SetParent(transform);
            holding.transform.localPosition = new Vector3(0, 0, 0);
        }else if(holding != null && GameManager._gm.gameobject_mat_in_hand == null && GameManager._gm.setting_movement == false)
        {
            GameManager._gm.setting_movement = true;
            GameManager._gm.setting_movement_origin = transform.position;
            GameManager._gm.holding_index = index;
        }
        else if (holding != null && GameManager._gm.gameobject_mat_in_hand == null && GameManager._gm.setting_movement == true && GameManager._gm.holding_index == index)
        {
            GameManager._gm.all_arrows[index] = null;
            Destroy(GameManager._gm.current_arrow);
            GameManager._gm.all_arrow_destinations[index] = 999;
            GameManager._gm.current_arrow = null;
            GameManager._gm.setting_movement = false;
            GameManager._gm.first_setting_movement = false;
            GameManager._gm.holding_index = -1;
        }
        else if(GameManager._gm.gameobject_mat_in_hand == null && GameManager._gm.setting_movement == true && GameManager._gm.holding_index != index)
        {
            GameManager._gm.current_arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(Vector3.Distance(Camera.main.WorldToScreenPoint(GameManager._gm.current_arrow.transform.position), Camera.main.WorldToScreenPoint(transform.position)), 3);
            GameManager._gm.current_arrow.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2(GameManager._gm.current_arrow.transform.position.y - transform.position.y, GameManager._gm.current_arrow.transform.position.x - transform.position.x) * Mathf.Rad2Deg + 180));
            GameManager._gm.all_arrow_destinations[GameManager._gm.holding_index] = index;
            GameManager._gm.current_arrow.transform.Find("arrow_head").position = transform.position;
            GameManager._gm.setting_movement = false;
            GameManager._gm.first_setting_movement = false;
            GameManager._gm.holding_index = -1;
        }
    }
}
