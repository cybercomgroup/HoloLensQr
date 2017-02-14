using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour, IFocusable, IInputClickHandler
{
    public Color highlightColor = new Color(1,1,1,1);
    public Color defaultColor = new Color(1,1,1,0.5f);
    public String command;

    // Use this for initialization
    void Start()
    {
        ((Image)this.gameObject.GetComponent("Image")).color = defaultColor;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnFocusEnter()
    {
        //throw new NotImplementedException();
        ((Image)this.gameObject.GetComponent("Image")).color = highlightColor;
    }

    public void OnFocusExit()
    {
        //throw new NotImplementedException();
        ((Image)this.gameObject.GetComponent("Image")).color = defaultColor;
    }

    public void OnInputClicked(InputEventData eventData)
    {
        //throw new NotImplementedException();
        Debug.Log("Clicked");
        //Backend.Instance.someFunction();
        //this.transform.parent.parent.GetComponent<MenuScript>().menuFunction("click");
    }

}
