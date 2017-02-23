using HoloToolkit.Unity.SpatialMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;

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

        //hides any image
        transform.Find("Canvas/Image").gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        
        //Build new menu
        try
        {
            ((Text)transform.Find("Canvas/TitleBar/Title").gameObject.GetComponent(typeof(Text))).text = menu.title;

            if (menu.background != null)
            {
                Backend.Instance.requestImage(menu.background, onImageDownloaded);
            }

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

    /// <summary>
    /// Moves and rotates the menu 10% closer to where the user is gazing at. After a sudden movement it will be 10% from its target at 0.3 s and 0.2% from its target at 1 s (with 60 fps).
    /// </summary>
    private void move()
    { 
        //find position gazing at
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;
        Vector3 position;
        Quaternion orientation;
        RaycastHit hitInfo;
        var layermask = 1 << 31; // 31: Physics layer
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, layermask))
        {
            position = hitInfo.point;
            orientation = Quaternion.LookRotation(-hitInfo.normal);

            float wallInclinationThreshold = 10.0F;

            Vector3 normalVerticalProjection = new Vector3(0.0F, hitInfo.normal.y, 0.0F);
            Vector3 normalHorizontalProjection = new Vector3(hitInfo.normal.x, 0.0F, hitInfo.normal.z);

            bool isVertical = normalHorizontalProjection.magnitude / normalVerticalProjection.magnitude > wallInclinationThreshold;
            if (isVertical)
            {
                //Debug.Log("Wall");
                orientation = Quaternion.LookRotation(-hitInfo.normal);
            }
            else
            {
                bool isHorizontal = normalVerticalProjection.magnitude / normalHorizontalProjection.magnitude > wallInclinationThreshold;
                if (isHorizontal)
                {
                    if (hitInfo.normal.y > 0)
                    {
                        //Debug.Log("Floor");
                        orientation = Quaternion.LookRotation(-hitInfo.normal, Camera.main.transform.forward);
                    }
                    else
                    {
                        //Debug.Log("Roof");
                        orientation = Quaternion.LookRotation(-hitInfo.normal, -Camera.main.transform.forward);
                    }
                }
            }
        }
        else
        {
            //Debug.Log("Floating menu");
            position = headPosition + 2f * gazeDirection; //2m in front
            orientation = Camera.main.transform.localRotation; // facing user
            orientation.x = 0;
            orientation.z = 0;
        }

        transform.position = Vector3.Lerp(transform.position, position, 0.1f);
        transform.rotation = Quaternion.Lerp(transform.rotation, orientation, 0.1f);
    }

    /// <summary>
    /// Sets the menu in the move mode.
    /// </summary>
    public void startMove()
    {
        DestroyImmediate(gameObject.GetComponent<WorldAnchor>());
        isMoving = true;
        UserActions.Instance.MovingMenu = this;
        SpatialMappingManager.Instance.StartObserver();
        TapToPlaceObject.SetActive(true);
    }

    /// <summary>
    /// Places the menu
    /// </summary>
    public void stopMove()
    {
        SpatialMappingManager.Instance.StopObserver();
        isMoving = false;
        UserActions.Instance.MovingMenu = null;
        TapToPlaceObject.SetActive(false);
        WorldAnchor anchor = gameObject.AddComponent<WorldAnchor>();
        anchor.OnTrackingChanged += Anchor_OnTrackingChanged;
    }

    /// <summary>
    /// Event function called if something happens to the world anchor to which it is attached.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="located"></param>
    private void Anchor_OnTrackingChanged(WorldAnchor self, bool located)
    {
        Debug.Log("World Anchor changed to " + located.ToString());
        // This simply activates/deactivates this object and all children when tracking changes
        //self.gameObject.SetActive(located);
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

    /// <summary>
    /// Callback function when a command has been set (when a button is clicked and it is sent to the server)
    /// </summary>
    /// <param name="output"></param>
    public void onCommandSet(string output)
    {
        //Debug.Log("Command: " + output);
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

    /// <summary>
    /// Callback function wich updates the menu background with the downloaded menu.
    /// </summary>
    /// <param name="image"></param>
    public void onImageDownloaded(Texture2D image)
    {
        try
        {
            Rect rec = new Rect(0, 0, image.width, image.height);
            Sprite sprite = Sprite.Create(image, rec, new Vector2(0.5f, 0.5f), 1);
            GameObject localImage = transform.Find("Canvas/Image").gameObject;
            Image theImage = localImage.GetComponent<Image>();
            theImage.color = new Color(1, 1, 1, 1);
            theImage.sprite = sprite;
        }
        catch
        {
            Debug.Log("Error loading image");
        }
    }
        
    /// <summary>
    /// Places the menu when it is clicked if it was moving. Otherwise it does nothing.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isMoving)
        {
            stopMove();
        }
    }
}

