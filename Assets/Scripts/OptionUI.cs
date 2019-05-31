using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionUI : MonoBehaviour
{
    EZR.Option option;
    Text sliderLimitFPSText;

    void Start()
    {
        sliderLimitFPSText = transform.Find("GroupSystem/BarPerformance/ChkBoxLimitFPS/SliderLimitFPS/Text").GetComponent<Text>();

        option = EZR.UserSaveData.GetOption();

        updateSystem();
    }

    void updateSystem()
    {
        var screenMode = transform.Find("GroupSystem/BarScreenMode");
        switch (option.FullScreenMode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                screenMode.Find("ChkBoxExclusiveFullScreen").GetComponent<Toggle>().isOn = true;
                break;
            case FullScreenMode.FullScreenWindow:
                screenMode.Find("ChkBoxFullScreenWindow").GetComponent<Toggle>().isOn = true;
                break;
            default:
                screenMode.Find("ChkBoxWindowed").GetComponent<Toggle>().isOn = true;
                break;
        }
        var resolutions = transform.Find("GroupSystem/BarResolutions/Dropdown").GetComponent<Dropdown>();
        resolutions.options.Clear();
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            var resolution = Screen.resolutions[i];
            resolutions.options.Add(new Dropdown.OptionData(resolution.width + "×" + resolution.height));
            if (option.Resolution.width == resolution.width && option.Resolution.height == resolution.height)
                resolutions.value = i;
        }
        var language = transform.Find("GroupSystem/BarLanguage");
        switch (option.Language)
        {
            default:
                language.Find("ChkBoxSChinese").GetComponent<Toggle>().isOn = true;
                break;
            case SystemLanguage.English:
                language.Find("ChkBoxEnglish").GetComponent<Toggle>().isOn = true;
                break;
        }
        var timePrecision = transform.Find("GroupSystem/BarTimePrecision");
        switch (option.TimePrecision)
        {
            default:
                timePrecision.Find("ChkBox1ms").GetComponent<Toggle>().isOn = true;
                break;
            case 5:
                timePrecision.Find("ChkBox5ms").GetComponent<Toggle>().isOn = true;
                break;
            case 10:
                timePrecision.Find("ChkBox10ms").GetComponent<Toggle>().isOn = true;
                break;
        }
        var performance = transform.Find("GroupSystem/BarPerformance");
        performance.Find("ChkBoxFrostedGlass").GetComponent<Toggle>().isOn = option.FrostedGlassEffect;
        performance.Find("ChkBoxVSync").GetComponent<Toggle>().isOn = option.VSync;
        var chkBoxLimitFPS = performance.Find("ChkBoxLimitFPS");
        if (!option.VSync) chkBoxLimitFPS.gameObject.SetActive(true);
        else chkBoxLimitFPS.gameObject.SetActive(false);
        chkBoxLimitFPS.GetComponent<Toggle>().isOn = option.LimitFPS;
        var sliderLimitFPS = chkBoxLimitFPS.Find("SliderLimitFPS");
        if (option.LimitFPS) sliderLimitFPS.gameObject.SetActive(true);
        else sliderLimitFPS.gameObject.SetActive(false);
        sliderLimitFPS.Find("Text").GetComponent<Text>().text = option.TargetFrameRate.ToString();
        sliderLimitFPS.Find("Slider").GetComponent<Slider>().value = option.TargetFrameRate;
    }

    public void ToggleScreenMode(bool value)
    {
        if (!value) return;
        var toggle = EventSystem.current.currentSelectedGameObject;
        switch (toggle.name)
        {
            case "ChkBoxExclusiveFullScreen":
                option.FullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case "ChkBoxFullScreenWindow":
                option.FullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case "ChkBoxWindowed":
                option.FullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }

    public void DropdownResolution(int index)
    {
        option.Resolution = Screen.resolutions[index];
    }

    public void ToggleTimePrecision(bool value)
    {
        if (!value) return;
        var toggle = EventSystem.current.currentSelectedGameObject;
        switch (toggle.name)
        {
            case "ChkBox1ms":
                option.TimePrecision = 1;
                break;
            case "ChkBox5ms":
                option.TimePrecision = 5;
                break;
            case "ChkBox10ms":
                option.TimePrecision = 10;
                break;
        }
    }

    public void TogglePerformance(bool value)
    {
        var toggle = EventSystem.current.currentSelectedGameObject;
        switch (toggle.name)
        {
            case "ChkBoxFrostedGlass":
                option.FrostedGlassEffect = value;
                break;
            case "ChkBoxVSync":
                option.VSync = value;
                break;
            case "ChkBoxLimitFPS":
                option.LimitFPS = value;
                break;
        }
        var performance = transform.Find("GroupSystem/BarPerformance");
        var chkBoxLimitFPS = performance.Find("ChkBoxLimitFPS");
        if (!option.VSync) chkBoxLimitFPS.gameObject.SetActive(true);
        else chkBoxLimitFPS.gameObject.SetActive(false);
        var sliderLimitFPS = chkBoxLimitFPS.Find("SliderLimitFPS");
        if (option.LimitFPS) sliderLimitFPS.gameObject.SetActive(true);
        else sliderLimitFPS.gameObject.SetActive(false);
    }

    public void SliderLimitFPS(float value)
    {
        option.TargetFrameRate = (int)value;
        sliderLimitFPSText.text = option.TargetFrameRate.ToString();
    }

    public void ClickSound()
    {
        EZR.MemorySound.PlaySound("e_click");
    }

    public void Apply()
    {
        EZR.UserSaveData.SetOption(option);
        EZR.UserSaveData.SaveData();

        // 设置画面模式
        if (option.VSync) QualitySettings.vSyncCount = 1;
        else QualitySettings.vSyncCount = 0;
        if (option.LimitFPS)
            Application.targetFrameRate = option.TargetFrameRate;
        else
            Application.targetFrameRate = 0;
        Screen.SetResolution(option.Resolution.width, option.Resolution.height, option.FullScreenMode);
        // 设置时间粒度
        EZR.Master.TimePrecision = option.TimePrecision;

        Destroy(gameObject);
        EZR.MemorySound.PlaySound("e_click");
    }

    public void Close()
    {
        Destroy(gameObject);
        EZR.MemorySound.PlaySound("e_click");
    }
}
