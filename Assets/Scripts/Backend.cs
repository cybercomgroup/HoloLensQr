using HoloToolkit.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Backend : Singleton<Backend> {
    protected Backend() { }

    private int cacheNumber = 0;
    private string adress = "http://10.90.131.67/";

    public GameObject MenuPrefab = null;
    public GameObject NotificationPrefab = null;
    private GameObject note = null;
    //public GameObject NotificationObject;

    // Use this for initialization
    void Start()
    {
        if (MenuPrefab == null)
        {
            try
            {
                MenuPrefab = (GameObject)Resources.Load("Menu", typeof(GameObject));
            }
            catch
            {
                Debug.Log("Error loading prefabs");
            }
        }
        if (NotificationPrefab == null)
        {
            try
            {
                NotificationPrefab = (GameObject)Resources.Load("Notification", typeof(GameObject));
            }
            catch
            {
                Debug.Log("Error loading prefabs");
            }
        }
    }

    void Update()
    {
        if (note != null)
        {
            SetNotePosition();
        }
    }

    private void SetNotePosition()
    {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;
        Vector3 position = headPosition + 2f * gazeDirection;
        Quaternion toQuat = Camera.main.transform.localRotation;
        toQuat.x = 0;
        toQuat.z = 0;
        note.transform.position = position;
        note.transform.rotation = toQuat;
    }

    public void showNote (string text)
    {
        Debug.Log(text);
        if (note != null)
        {
            Destroy(note);
        }
        note = Instantiate(NotificationPrefab).gameObject;
        ((TextMesh)note.GetComponent(typeof(TextMesh))).text = text;
        Destroy(note, 10f);
    }


    public void createMenu (string id)
    {
        if (MenuPrefab != null)
        {
            Debug.Log("Backend: createMenu - start");
            GameObject menu = Instantiate(MenuPrefab).gameObject;
            ((MenuScript)menu.GetComponent(typeof(MenuScript))).setId(id);
        }
        else
        {
            Debug.Log("couldn't find Menu Prefab");
        }
    }

    /// <summary>
    /// Creates a string to be attached to an url making it unique and 
    /// thereby not cached.
    /// </summary>
    /// <returns>"?cache=nnn"</returns>
    private string noCache ()
    {
        return "?cache=" + (cacheNumber++).ToString();
    }

    public void requestMenu(string id, Action<string> callback)
    {
        String url = adress + "menu/" + id + noCache();
        WWW www = new WWW(url);
        StartCoroutine(GetText(www, callback));
    }

    public void requestImage(string id, Action<Texture2D> callback)
    {
        String url = adress + "image/" + id + noCache();
        WWW www = new WWW(url);
        StartCoroutine(GetImage(www, callback));
    }

    public void sendCommand (string menuId, string commandId, string value, Action<string> callback)
    {
        //String url = adress + menuId + "/" + commandId + "/" + WWW.EscapeURL(value) + noCache();
        String url = adress + menuId + "/" + commandId + "/" + System.Uri.EscapeUriString(value) + noCache();
        WWW www = new WWW(url);
        StartCoroutine(GetText(www, callback));
    }

    IEnumerator GetText(WWW www, Action<string> callback)
    {
        yield return www;
        if (www.error == null)
        {
            //Debug.Log("WWW Ok!");
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
        callback(www.text);
    }
    IEnumerator GetImage(WWW www, Action<Texture2D> callback)
    {
        yield return www;
        if (www.error == null)
        {
            //Debug.Log("WWW Ok!");
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
        callback(www.texture);
    }
}
