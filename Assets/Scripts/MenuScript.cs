﻿using HoloToolkit.Unity.SpatialMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour, IPointerClickHandler {

    private string menuId = "default"; // 404
    private Menu menu = null;
    private bool isMoving = false;
    
    public Transform ListItemPrefab;
    public Transform ButtonsPrefab;
    public Transform ButtonPrefab;
    public Transform InputPrefab;
    public Transform TextPrefab;
    public Transform SliderPrefab;
    public Transform IndicatorPrefab;

    public GameObject TapToPlaceObject;
    public GameObject theListContainer;
    public GameObject theInputsContainer;
    private Dictionary<string, GameObject> theList = new Dictionary<string, GameObject>();
    private List<GameObject> theInputs = new List<GameObject>();
    private Dictionary<string, GameObject> theInputsId = new Dictionary<string, GameObject>();

    // Use this for initialization
    void Start ()
    {
        if (theListContainer == null)
        {
            try
            {
                theListContainer = transform.Find("Canvas/Right").gameObject;
            }
            catch
            {
                Debug.Log("Error setting up menu");
            }
        }
        if (theInputsContainer == null)
        {
            try
            {
                theInputsContainer = transform.Find("Canvas/Left").gameObject;
            }
            catch
            {
                Debug.Log("Error setting up menu");
            }
        }
        if (TapToPlaceObject == null)
        {
            try
            {
                TapToPlaceObject = transform.Find("Canvas/TapToPlace").gameObject;
                TapToPlaceObject.SetActive(false);
            }
            catch
            {
                Debug.Log("Error setting up menu");
            }
        }
        
        startMove();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            move();
        }		
    }

    /// <summary>
    /// Sets the menu id and starts downloading the menu every second.
    /// </summary>
    /// <param name="id">the menu id</param>
    public void setId (string id)
    {
        menuId = id;
        InvokeRepeating("reload", .01f, 1.0f);
    }

    /// <summary>
    /// Build contetnt of the menu according to the menu object
    /// </summary>
    /// <param name="menu"></param>
    private void BuildMenu (Menu menu)
    {
        Debug.Log("Building menu");
        
        //Delete old menu
        foreach (KeyValuePair<string, GameObject> thing in theList)
        {
            Destroy(thing.Value);
        }
        theList.Clear();
        foreach (GameObject thing in theInputs)
        {
            Destroy(thing);
        }
        theInputs.Clear();
        foreach (KeyValuePair<string, GameObject> thing in theInputsId)
        {
            Destroy(thing.Value);
        }
        theInputsId.Clear();

        //Build new menu
        try
        {
            ((Text)transform.Find("Canvas/TitleBar/Title").gameObject.GetComponent(typeof(Text))).text = menu.title;

            foreach (Menu.MenuInput entry in menu.inputs)
            {
                //Add the inputs
                GameObject inputItem = null;
                switch (entry.type)
                {
                    case "buttons":
                        inputItem = Instantiate(ButtonsPrefab, new Vector3(0, -80f * theInputs.Count, 0), Quaternion.identity).gameObject;
                        //Add the individual buttons
                        int buttonCount = 0;
                        foreach (Menu.MenuButton b in entry.buttons)
                        {
                            GameObject buttonItem = Instantiate(ButtonPrefab, new Vector3(-185 + 120f * buttonCount++, 0, 0), Quaternion.identity).gameObject;
                            buttonItem.transform.SetParent(inputItem.transform, false);
                            ((Text)buttonItem.transform.Find("Text").gameObject.GetComponent<Text>()).text = b.text; //check!!!
                            ((Button)buttonItem.GetComponent(typeof(Button))).onClick.AddListener(delegate { menuFunction(b.id, b.value); });
                            if (!theInputsId.ContainsKey(b.id))
                            {
                                theInputsId.Add(b.id, buttonItem);
                            }
                            else
                            {
                                Debug.Log("Duplicate id in Input");
                            }
                        }
                        break;
                    case "input":
                        inputItem = Instantiate(InputPrefab, new Vector3(0, -80f * theInputs.Count, 0), Quaternion.identity).gameObject;
                        ((Text)inputItem.transform.Find("Text").gameObject.GetComponent(typeof(Text))).text = entry.text;
                        InputField field = ((InputField)inputItem.transform.Find("InputField").gameObject.GetComponent(typeof(InputField)));
                        field.text = entry.value;
                        Button btn = ((Button)inputItem.transform.Find("Button").gameObject.GetComponent(typeof(Button)));
                        btn.onClick.AddListener(delegate { menuFunction(entry.id, field.text); });
                        break;
                    case "text":
                        inputItem = Instantiate(TextPrefab, new Vector3(0, -80f * theInputs.Count, 0), Quaternion.identity).gameObject;
                        ((Text)inputItem.GetComponent(typeof(Text))).text = entry.text;
                        break;
                    case "slider":
                        inputItem = Instantiate(SliderPrefab, new Vector3(0, -80f * theInputs.Count, 0), Quaternion.identity).gameObject;
                        // TODO ...
                        break;
                    case "indicator":
                        inputItem = Instantiate(IndicatorPrefab, new Vector3(0, -80f * theInputs.Count, 0), Quaternion.identity).gameObject;
                        // TODO ...
                        break;
                    default:
                        Debug.Log("Unknown input type");
                        break;
                }
                if (inputItem != null)
                {
                    inputItem.transform.SetParent(theInputsContainer.transform, false);
                    //((Text)inputItem.GetComponent(typeof(Text))).text = "<b>" + entry.title + "</b> " + entry.text;
                    theInputs.Add(inputItem);
                    if (entry.id != null) //true for buttons
                    {
                        if (!theInputsId.ContainsKey(entry.id))
                        {
                            theInputsId.Add(entry.id, inputItem);
                        }
                        else
                        {
                            Debug.Log("Duplicate id in Input");
                        }
                    }
                }
            }

            foreach (Menu.ListItem entry in menu.list)
            {
                if (!theList.ContainsKey(entry.id))
                {
                    GameObject listItem = Instantiate(ListItemPrefab, new Vector3(0, -40f * theList.Count, 0), Quaternion.identity).gameObject;
                    listItem.transform.SetParent(theListContainer.transform, false);
                    ((Text)listItem.GetComponent(typeof(Text))).text = "<b>" + entry.title + "</b> " + entry.text;
                    theList.Add(entry.id, listItem);
                }
                else
                {
                    Debug.Log("Duplicate id in List");
                }
            }
        }
        catch
        {
            Debug.Log("Error building menu");
        }
    }

    /// <summary>
    /// Updates some value in the menu.(TODO: not implemented yet)
    /// So far we simply download the entire menu every second and rebuild it all if something has changed.
    /// </summary>
    /// <param name="field">What to update</param>
    /// <param name="value">Which value</param>
    private void UpdateValue (string field, string value)
    {
        //update the field.
    }

    /// <summary>
    /// Closes the menu window
    /// </summary>
    public void close()
    {
        Destroy(transform.gameObject);
    }

    /// <summary>
    /// Method to reload everything in the menu from the server.
    /// </summary>
    public void reload()
    {
        //Reload everything in the menu
        Backend.Instance.requestMenu(menuId, onMenuDownloaded);
    }

    private void move()
    { 
        //find position gazing at
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;
        Vector3 position;
        Quaternion orientation;
        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f))
        {
            //hit!
            //Debug.Log("Backend: createMenu - hit");
            position = hitInfo.point;
            //orientation = Quaternion.LookRotation(-hitInfo.normal);
            orientation = Quaternion.FromToRotation(Vector3.forward,-hitInfo.normal);
            
            float wallInclinationThreshold = 10.0F;

            Vector3 normalVerticalProjection = new Vector3(0.0F, orientation.y, 0.0F);
            Vector3 normalHorizontalProjection = new Vector3(orientation.x, 0.0F, orientation.z);

            bool isWall = normalHorizontalProjection.magnitude / normalVerticalProjection.magnitude > wallInclinationThreshold;
            if (isWall)
            {
                //orientation.y = 0;
                //orientation.z = 0;
            }
            else
            {
                bool isFloor = normalVerticalProjection.magnitude / normalHorizontalProjection.magnitude > wallInclinationThreshold;
                if (isFloor)
                {
                    //orientation.y = 0;
                    //orientation.z = 0;
                }
            }
        }
        else
        {
            //2m in front
            //Debug.Log("Backend: createMenu - no hit");
            position = headPosition + 2f * gazeDirection;
            orientation = Camera.main.transform.localRotation;
            orientation.x = 0;
            orientation.z = 0;
        }
        transform.position = position;
        transform.rotation = orientation;
    }

    public void startMove()
    {
        isMoving = true;
        SpatialMappingManager.Instance.StartObserver();
        TapToPlaceObject.SetActive(true);
    }
    public void stopMove()
    {
        isMoving = false;
        SpatialMappingManager.Instance.StopObserver();
        TapToPlaceObject.SetActive(false);
    }

    /// <summary>
    /// Sends the command to the backend.
    /// </summary>
    /// <param name="id">what command to send</param>
    /// <param name="value">what value to send</param>
    public void menuFunction (string id, string value)
    {
        Backend.Instance.sendCommand(menuId, id, value, onCommandSet);
    }

    public void onCommandSet(string output)
    {
        Debug.Log("Command: " + output);
        //?
    }

    /// <summary>
    /// Callback function which updates the menu with json as a string.
    /// </summary>
    /// <param name="output">json object in a string</param>
    public void onMenuDownloaded (string output)
    {
        try
        {
            if (menu == null)
            {
                menu = new Menu(output);
                BuildMenu(menu);
            }
            else
            {
                Menu newMenu = new Menu(output);
                if (!newMenu.Equals(menu))
                {
                    menu = newMenu;
                    BuildMenu(menu);
                }
            }   
        }
        catch
        {
            Debug.Log("Error loading menu");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isMoving)
        {
            stopMove();
        }
    }
}
