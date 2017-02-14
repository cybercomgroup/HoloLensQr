using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Procurios.Public;
using System;

/*public class Menu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}*/

public class Menu
{
    public Menu (string json)
    {
        Hashtable a = (Hashtable)JSON.JsonDecode(json);

        title = (string)a["title"];
        
        foreach (Hashtable entry in (ArrayList)a["inputs"])
        {
            MenuInput anInput = new MenuInput();
            if (entry.ContainsKey("type"))
                anInput.type = (string)entry["type"];
            if (entry.ContainsKey("text"))
                anInput.text = (string)entry["text"];
            if (entry.ContainsKey("id"))
                anInput.id = (string)entry["id"];
            if (entry.ContainsKey("value"))
                anInput.value = (string)entry["value"];
            if (anInput.type == "buttons")
            {
                if (!entry.ContainsKey("buttons"))
                    throw new Exception();
                foreach (Hashtable b in (ArrayList)entry["buttons"])
                {
                    MenuButton aButton = new MenuButton();
                    if (b.ContainsKey("text"))
                        aButton.text = (string)b["text"];
                    if (b.ContainsKey("id"))
                        aButton.id = (string)b["id"];
                    if (b.ContainsKey("value"))
                        aButton.value = (string)b["value"];
                    anInput.buttons.Add(aButton);
                }
            }
            inputs.Add(anInput);
        }
        foreach (Hashtable entry in (ArrayList)a["list"])
        {
            ListItem anItem = new ListItem();
            if (entry.ContainsKey("title"))
                anItem.title = (string)entry["title"];
            if (entry.ContainsKey("id"))
                anItem.id = (string)entry["id"];
            if (entry.ContainsKey("text"))
                anItem.text = (string)entry["text"];

            list.Add(anItem);
        }
    }

    public string title = null;
    public List<MenuInput> inputs = new List<MenuInput>();
    public List<ListItem> list = new List<ListItem>();

    public class MenuInput
    {
        public string type = null;
        public string text = null;
        public List<MenuButton> buttons = new List<MenuButton>();
        public string id = null;
        public string value = null;

        public override bool Equals(object obj)
        {
            MenuInput other = obj as MenuInput;
            if (other == null)
                return false;
            if (type == other.type && text == other.text && id == other.id && value == other.value)
            {
                if (buttons.Count != other.buttons.Count)
                    return false;
                for (int i = 0; i < buttons.Count; i++)
                    if (!buttons[i].Equals(other.buttons[i]))
                        return false;
                return true;
            }
            else
                return false;
        }
    }

    public class MenuButton
    {
        public string text = null;
        public string id = null;
        public string value = null;

        public override bool Equals(object obj)
        {
            MenuButton other = obj as MenuButton;
            if (other == null)
                return false;
            return value == other.value && text == other.text && id == other.id;
        }
    }

    public class ListItem
    {
        public string title = null;
        public string id = null;
        public string text = null;

        public override bool Equals(object obj) //public bool Equals(ListItem other)
        {
            ListItem other = obj as ListItem;
            if (other == null)
                return false;
            return title == other.title && text == other.text && id == other.id;
        }
    }


    public override bool Equals(object obj)
    {
        Menu other = obj as Menu;
        if (other == null)
            return false;
        if (title != other.title)
            return false;
        if (list.Count != other.list.Count)
            return false;
        for (int i = 0; i < list.Count; i++)
            if (!list[i].Equals(other.list[i]))
                return false;
        if (inputs.Count != other.inputs.Count)
            return false;
        for (int i = 0; i < inputs.Count; i++)
            if (!inputs[i].Equals(other.inputs[i]))
                return false;
        return true;
    }

}