using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserActions : Singleton<UserActions> {

    public MenuScript movingMenu = null;
    
    //Starts scanning 
    public void ScanQR()
    {
        Backend.Instance.GetComponent<QrScan>().OnScan();
    }

    //Closes the (if any) gazed at menu
    public void CloseMenu()
    {
        MenuScript theMenu = GazeMenu();
        if (theMenu != null)
        {
            theMenu.close();
        }
    }

    //Starts movig the (if any) gazed at menu
    public void MoveMenu()
    {
        MenuScript theMenu = GazeMenu();
        if (theMenu != null)
        {
            theMenu.startMove();
        }
    }

    private MenuScript GazeMenu()
    {
        if (GazeManager.Instance.IsGazingAtObject)
        {
            GameObject g = GazeManager.Instance.HitObject;
            if (g != null)
            {
                return g.GetComponentInParent<MenuScript>();
            }
        }
        return null;
    }

    //Places the currently (if any) moving menu
    public void PlaceMenu()
    {
        if (movingMenu != null)
        {
            movingMenu.stopMove();
        }
        //movingMenu = null;
    }
}
