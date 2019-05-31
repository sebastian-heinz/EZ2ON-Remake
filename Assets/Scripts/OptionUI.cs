using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionUI : MonoBehaviour
{
    EZR.Option option;
    Text sliderLimitFPSText;
    List<Resolution> resolutions = new List<Resolution>();
    Dropdown dropdownResolutions;

    void Start()
    {
        sliderLimitFPSText = transform.Find("GroupSystem/BarPerformance/ChkBoxLimitFPS/SliderLimitFPS/Text").GetComponent<Text>();
        dropdownResolutions = transform.Find("GroupSystem/BarResolutions/Dropdown").GetComponent<Dropdown>();

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
        dropdownResolutions.options.Clear();
        foreach (var resolutionA in Screen.resolutions)
        {
            var isHit = false;
            foreach (var resolutionB in resolutions)
            {
                if (resolutionA.width == resolutionB.width &&
                resolutionA.height == resolutionB.height)
                {
                    isHit = true;
                    break;
                }
            }
            if (!isHit) resolutions.Add(new Resolution()
            {
                width = resolutionA.width,
                height = resolutionA.height
            });
        }
        for (int i = 0; i < resolutions.Count; i++)
        {
            var resolution = resolutions[i];
            dropdownResolutions.options.Add(new Dropdown.OptionData(resolution.width + "×" + resolution.height));
            if (option.Resolution.width == resolution.width &&
            option.Resolution.height == resolution.height)
                dropdownResolutions.value = i;
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
        var chkBoxSimVSync = performance.Find("ChkBoxSimVSync");
        var chkBoxLimitFPS = performance.Find("ChkBoxLimitFPS");
        if (!option.VSync)
        {
            chkBoxSimVSync.gameObject.SetActive(true);
            chkBoxLimitFPS.gameObject.SetActive(true);
        }
        else
        {
            chkBoxSimVSync.gameObject.SetActive(false);
            chkBoxLimitFPS.gameObject.SetActive(false);
        }
        chkBoxSimVSync.GetComponent<Toggle>().isOn = option.SimVSync;
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

    public void DropdownResolutionsClick()
    {
        dropdownResolutions.transform.Find("Dropdown List").GetComponent<ScrollRect>().verticalNormalizedPosition =
        1 - (float)dropdownResolutions.value / (resolutions.Count - 1);
        EZR.MemorySound.PlaySound("e_click");
    }
    public void DropdownResolutions(int index)
    {
        option.Resolution = resolutions[index];
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
            case "ChkBoxSimVSync":
                option.SimVSync = value;
                break;
            case "ChkBoxLimitFPS":
                option.LimitFPS = value;
                break;
        }
        var performance = transform.Find("GroupSystem/BarPerformance");
        var chkBoxSimVSync = performance.Find("ChkBoxSimVSync");
        var chkBoxLimitFPS = performance.Find("ChkBoxLimitFPS");
        if (!option.VSync)
        {
            chkBoxSimVSync.gameObject.SetActive(true);
            chkBoxLimitFPS.gameObject.SetActive(true);
        }
        else
        {
            chkBoxSimVSync.gameObject.SetActive(false);
            chkBoxLimitFPS.gameObject.SetActive(false);
        }
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
        if (option.Resolution.width != Screen.currentResolution.width ||
        option.Resolution.height != Screen.currentResolution.height ||
        option.FullScreenMode != Screen.fullScreenMode)
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
