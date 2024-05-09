using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckActionPanel : MonoBehaviour
{
    private TextMeshProUGUI label;
    private Button buttonCancelAction;
    private Button buttonConfirmAction;
    
    void OnEnable() => SetFields();

    void SetFields()
    {
        label               = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        buttonCancelAction  = transform.GetChild(0).GetChild(2).GetComponent<Button>();
        buttonConfirmAction = transform.GetChild(0).GetChild(3).GetComponent<Button>();

        buttonCancelAction.onClick.AddListener(() => ClosePanel());
    }

    public bool IsOn() => transform.GetChild(0).gameObject.activeInHierarchy;

    public void ClosePanel()
    {
        GetComponent<Animator>().SetBool("ShowPanel", false);
        buttonConfirmAction.onClick.RemoveAllListeners();
    }

    public void OpenPanel(Transform obj, string _text, Action act)
    {
        GetComponent<Animator>().SetBool("ShowPanel", true);
        transform.position = obj.position + Vector3.right * (obj.GetComponent<RectTransform>().rect.width);


        label.text = _text;
        buttonConfirmAction.onClick.AddListener(() => act());
        buttonConfirmAction.onClick.AddListener(() => ClosePanel());
    }

    public void OpenPanel(Transform obj, string _text, Action<int> act,int param)
    {
        GetComponent<Animator>().SetBool("ShowPanel", true);
        transform.position = obj.position + Vector3.right * (obj.GetComponent<RectTransform>().rect.width);

        label.text = _text;
        buttonConfirmAction.onClick.AddListener(() => act(param));
        buttonConfirmAction.onClick.AddListener(() => ClosePanel());
    }







}
