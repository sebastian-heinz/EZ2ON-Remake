using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    void Start()
    {
        var option = EZR.UserSaveData.GetOption();
        var frostedGlass = transform.Find("FrostedGlass").gameObject;
        frostedGlass.SetActive(option.FrostedGlassEffect);

        switch (EZR.DisplayLoop.PanelResource)
        {
            case "R14":
                transform.Find("Panel/R14").GetComponent<Toggle>().isOn = true;
                break;
            case "001":
                transform.Find("Panel/001").GetComponent<Toggle>().isOn = true;
                break;
            case "007":
                transform.Find("Panel/007").GetComponent<Toggle>().isOn = true;
                break;
        }
        switch (EZR.DisplayLoop.NoteResource)
        {
            case "Note_04":
                transform.Find("Note/Note_04").GetComponent<Toggle>().isOn = true;
                break;
            case "Note_01":
                transform.Find("Note/Note_01").GetComponent<Toggle>().isOn = true;
                break;
            case "Note_02":
                transform.Find("Note/Note_02").GetComponent<Toggle>().isOn = true;
                break;
        }
    }

    public void ClickSound()
    {
        EZR.MemorySound.PlaySound("e_click");
    }

    public void TogglePanel()
    {
        var toggle = EventSystem.current.currentSelectedGameObject;
        switch (toggle.name)
        {
            case "R14":
                EZR.DisplayLoop.PanelResource = "R14";
                break;
            case "001":
                EZR.DisplayLoop.PanelResource = "001";
                break;
            case "007":
                EZR.DisplayLoop.PanelResource = "007";
                break;
        }
    }

    public void ToggleNote()
    {
        var toggle = EventSystem.current.currentSelectedGameObject;
        switch (toggle.name)
        {
            case "Note_04":
                EZR.DisplayLoop.NoteResource = "Note_04";
                break;
            case "Note_01":
                EZR.DisplayLoop.NoteResource = "Note_01";
                break;
            case "Note_02":
                EZR.DisplayLoop.NoteResource = "Note_02";
                break;
        }
    }

    public void BtnClose()
    {
        EZR.UserSaveData.SetInventory();
        EZR.UserSaveData.SaveData();
        Destroy(gameObject);
        EZR.MemorySound.PlaySound("e_click");
    }
}
