using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementButtonActive : MonoBehaviour
{
    public bool override_active = false;
    public int idx;
    [SerializeField] private string _name;
    private bool first = false;
    private Transform bt;
    private Image img;
    private RectTransform rectTransform;
    private Transform mat;
    void Start()
    {
        first = false;
        bt = transform.Find("Button");
        mat = bt.transform.Find("mask").transform.Find("mat");
        img = bt.GetComponent<Image>();
        rectTransform = bt.GetComponent<RectTransform>();
    }
    void OnMouseOver()
    {
        GameManager._gm.use_tooltip = true;
        GameManager._gm.tt_text = "Select " + _name + "?";
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 screenmouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (GameManager._gm.unlocked_materials[0] == true || override_active)
        {
            bt.gameObject.SetActive(true);
            if (first == false)
            {
                bt.transform.DOScaleX(1, 0.75f).SetEase(Ease.OutExpo);
                StartCoroutine(SwitchLayering_Corutine());
                first = true;
            }
        }
        bt.GetComponent<Button>().onClick.AddListener(() => { mat.transform.DORotate(new Vector3(0,0,UnityEngine.Random.Range(0,361)),0.5f).SetEase(Ease.OutQuad); });
    }
    public IEnumerator SwitchLayering_Corutine()
    {
        yield return new WaitForSeconds(0.75f);
        img.maskable = true;
    }
}
