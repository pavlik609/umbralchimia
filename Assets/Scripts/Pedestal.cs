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
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(LateStart(0.01f));
    }
    public IEnumerator LateStart(float sec)
    {
        yield return new WaitForSeconds(sec);
        button = GetComponent<Button>();
        button.onClick.AddListener(delegate { Clicked(); } );
        index = transform.GetSiblingIndex();
    }

    // Update is called once per frame
    void Update()
    {
        index = transform.GetSiblingIndex();
        if (rectTransform.rect.xMax + rectTransform.position.x >= Input.mousePosition.x && Input.mousePosition.x >= rectTransform.rect.xMin + rectTransform.position.x)
        {
            if (rectTransform.rect.yMax + rectTransform.position.y >= Input.mousePosition.y && Input.mousePosition.y >= rectTransform.rect.yMin + rectTransform.position.y)
            {
                if (GameManager._gm.right_click)
                {
                    Destroy(holding);
                    holding = null;
                    Destroy(GameManager._gm.all_arrows[index]);
                    GameManager._gm.table[index] = null;
                    GameManager._gm.all_arrows[index] = null;
                    GameManager._gm.current_arrow = null;
                    GameManager._gm.setting_movement = false;
                    GameManager._gm.first_setting_movement = false;
                    GameManager._gm.holding_index = -1;
                }
            }
        }
    }
    public void Clicked()
    {
        if (holding == null && GameManager._gm.gameobject_mat_in_hand != null)
        {
            holding = GameManager._gm.gameobject_mat_in_hand;
            GameManager._gm.table[index] = GameManager._gm.material_in_hand;
            GameManager._gm.gameobject_mat_in_hand = null;
            GameManager._gm.material_in_hand = null;
            GameManager._gm.has_material_in_hand = false;
            holding.transform.SetParent(transform);
            holding.transform.localPosition = new Vector3(0, 0, 0);
        }else if(holding != null && GameManager._gm.gameobject_mat_in_hand == null && GameManager._gm.setting_movement == false)
        {
            GameManager._gm.setting_movement = true;
            GameManager._gm.setting_movement_origin = transform.localPosition;
            GameManager._gm.holding_index = index;
        }
        else if (holding != null && GameManager._gm.gameobject_mat_in_hand == null && GameManager._gm.setting_movement == true && GameManager._gm.holding_index == index)
        {
            GameManager._gm.all_arrows[index] = null;
            Destroy(GameManager._gm.current_arrow);
            GameManager._gm.current_arrow = null;
            GameManager._gm.setting_movement = false;
            GameManager._gm.first_setting_movement = false;
            GameManager._gm.holding_index = -1;
        }
        else if(GameManager._gm.gameobject_mat_in_hand == null && GameManager._gm.setting_movement == true && GameManager._gm.holding_index != index)
        {
            GameManager._gm.current_arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(Vector3.Distance(GameManager._gm.current_arrow.transform.position, transform.position), 3);
            GameManager._gm.current_arrow.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2(GameManager._gm.current_arrow.transform.position.y - transform.position.y, GameManager._gm.current_arrow.transform.position.x - transform.position.x) * Mathf.Rad2Deg + 180));
            GameManager._gm.all_arrow_destinations[GameManager._gm.holding_index] = index;
            GameManager._gm.setting_movement = false;
            GameManager._gm.first_setting_movement = false;
            GameManager._gm.holding_index = -1;
        }
    }
}
