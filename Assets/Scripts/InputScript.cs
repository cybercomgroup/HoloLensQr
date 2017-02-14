using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class InputScript : MonoBehaviour, IPointerClickHandler
{

    //public InputField inputField;
    TouchScreenKeyboard keyboard;

    // Use this for initialization
    void Start () {
        //Load inputfield
        /*if (inputField == null)
        {
            try
            {
                inputField = (InputField)transform.Find("InputField").gameObject.GetComponent(typeof(InputField));
                //inputField.onclick...
            }
            catch
            {
                Debug.Log("Input: Error loading inputField");
            }
        }*/

        //add onclick listener

        //InputFieldPrefab = ((InputField)inputItem.transform.Find("InputField").gameObject.GetComponent(typeof(InputField)));
        //this.onClick.AddListener(delegate { menuFunction(entry.id, field.text); });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        loadKeyboard();
    }

    // Update is called once per frame
    void Update () {		
	}

    public void loadKeyboard ()
    {
        //TODO: This doesn't work...
        /*Debug.Log("Loading keyboard");
        keyboard = new TouchScreenKeyboard("a", TouchScreenKeyboardType.Default, false, false, false, false, "b");*/
    }

    public void closeKeyboard ()
    {
        try
        {
            ((InputField)transform.gameObject.GetComponent(typeof(InputField))).text = keyboard.text;
        }
        catch
        {
            // ?
        }
        keyboard = null;
    }
}
