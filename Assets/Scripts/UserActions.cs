using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
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
        MenuScript theMenu = MenuGazedAt();
        if (theMenu != null)
        {
            theMenu.close();
        }
    }

    //Starts movig the (if any) gazed at menu
    public void MoveMenu()
    {
        MenuScript theMenu = MenuGazedAt();
        if (theMenu != null)
        {
            theMenu.startMove();
        }
    }

    /// <summary>
    /// Returns the menu currently gazed at
    /// </summary>
    /// <returns>The MenuScript of the menu</returns>
    private MenuScript MenuGazedAt()
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
    }
}
