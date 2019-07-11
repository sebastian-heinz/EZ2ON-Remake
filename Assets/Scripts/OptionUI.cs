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

        // 找毛玻璃
        var frostedGlass = transform.Find("FrostedGlass").gameObject;
        frostedGlass.SetActive(option.FrostedGlassEffect);

        transform.Find("BtnSystem").GetComponent<EZR.ButtonExtension>().SetSelected(true);

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

        updateResolutions();

        var language = transform.Find("GroupSystem/BarLanguage");
        switch (option.Language)
        {
            default:
                language.Find("ChkBoxSChinese").GetComponent<Toggle>().isOn = true;
                break;
            case SystemLanguage.English:
                language.Find("ChkBoxEnglish").GetComponent<Toggle>().isOn = true;
                break;
            case SystemLanguage.Korean:
                language.Find("ChkBoxKorean").GetComponent<Toggle>().isOn = true;
                break;
            case SystemLanguage.Japanese:
                language.Find("ChkBoxJapanese").GetComponent<Toggle>().isOn = true;
                break;
        }
        var performance = transform.Find("GroupSystem/BarPerformance");
        performance.Find("ChkBoxFrostedGlass").GetComponent<Toggle>().isOn = option.FrostedGlassEffect;
        performance.Find("ChkBoxVSync").GetComponent<Toggle>().isOn = option.VSync;
        var chkBoxLimitFPS = performance.Find("ChkBoxLimitFPS");
        if (!option.VSync)
        {
            chkBoxLimitFPS.gameObject.SetActive(true);
        }
        else
        {
            chkBoxLimitFPS.gameObject.SetActive(false);
        }
        chkBoxLimitFPS.GetComponent<Toggle>().isOn = option.LimitFPS;
        var sliderLimitFPS = chkBoxLimitFPS.Find("SliderLimitFPS");
        if (option.LimitFPS) sliderLimitFPS.gameObject.SetActive(true);
        else sliderLimitFPS.gameObject.SetActive(false);
        sliderLimitFPS.Find("Text").GetComponent<Text>().text = option.TargetFrameRate.ToString();
        sliderLimitFPS.Find("Slider").GetComponent<Slider>().value = option.TargetFrameRate;
        var panelPosition = transform.Find("GroupSkin/BarPanelPosition");
        switch (option.PanelPosition)
        {
            case EZR.Option.PanelPositionEnum.Left:
                panelPosition.Find("ChkBoxLeft").GetComponent<Toggle>().isOn = true;
                break;
            case EZR.Option.PanelPositionEnum.Center:
                panelPosition.Find("ChkBoxCenter").GetComponent<Toggle>().isOn = true;
                break;
            case EZR.Option.PanelPositionEnum.Right:
                panelPosition.Find("ChkBoxRight").GetComponent<Toggle>().isOn = true;
                break;
        }
        var targetLineType = transform.Find("GroupSkin/BarLineType");
        switch (option.TargetLineType)
        {
            case EZR.Option.TargetLineTypeEnum.New:
                targetLineType.Find("ChkBoxNew").GetComponent<Toggle>().isOn = true;
                break;
            case EZR.Option.TargetLineTypeEnum.Classic:
                targetLineType.Find("ChkBoxClassic").GetComponent<Toggle>().isOn = true;
                break;
        }
        var panelETC = transform.Find("GroupSkin/BarETC");
        panelETC.Find("ChkBoxShowFastSlow").GetComponent<Toggle>().isOn = option.ShowFastSlow;
        updateJudgmentOffset();
        transform.Find("GroupSound/SliderMasterVolume").GetComponent<Slider>().value = option.Volume.Master;
        transform.Find("GroupSound/SliderGameVolume").GetComponent<Slider>().value = option.Volume.Game;
        transform.Find("GroupSound/SliderMainVolume").GetComponent<Slider>().value = option.Volume.Main;
        transform.Find("GroupSound/SliderBGMVolume").GetComponent<Slider>().value = option.Volume.BGM;
        transform.Find("GroupSound/ChkBoxLive3D").GetComponent<Toggle>().isOn = option.Volume.Live3D;
    }

    void updateResolutions()
    {
        if (dropdownResolutions == null) return;
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
        bool isHit2 = false;
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (option.FullScreenMode == FullScreenMode.Windowed &&
            i == resolutions.Count - 1)
                break;
            var resolution = resolutions[i];
            dropdownResolutions.options.Add(new Dropdown.OptionData(resolution.width + "×" + resolution.height));
            if (option.Resolution.width == resolution.width &&
            option.Resolution.height == resolution.height)
            {
                isHit2 = true;
                dropdownResolutions.value = i;
            }
        }
        if (!isHit2)
        {
            dropdownResolutions.value = resolutions.Count - 1;
            option.Resolution = resolutions[resolutions.Count - 1];
        }
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
        updateResolutions();
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
        if (!option.VSync)
        {
            chkBoxLimitFPS.gameObject.SetActive(true);
        }
        else
        {
            chkBoxLimitFPS.gameObject.SetActive(false);
        }
        var sliderLimitFPS = chkBoxLimitFPS.Find("SliderLimitFPS");
        if (option.LimitFPS) sliderLimitFPS.gameObject.SetActive(true);
        else sliderLimitFPS.gameObject.SetActive(false);
    }

    public void TogglePanelPosition(bool value)
    {
        if (!value) return;
        var toggle = EventSystem.current.currentSelectedGameObject;
        switch (toggle.name)
        {
            case "ChkBoxLeft":
                option.PanelPosition = EZR.Option.PanelPositionEnum.Left;
                break;
            case "ChkBoxCenter":
                option.PanelPosition = EZR.Option.PanelPositionEnum.Center;
                break;
            case "ChkBoxRight":
                option.PanelPosition = EZR.Option.PanelPositionEnum.Right;
                break;
        }
    }

    public void ToggleTargetLineType(bool value)
    {
        if (!value) return;
        var toggle = EventSystem.current.currentSelectedGameObject;
        switch (toggle.name)
        {
            case "ChkBoxNew":
                option.TargetLineType = EZR.Option.TargetLineTypeEnum.New;
                break;
            case "ChkBoxClassic":
                option.TargetLineType = EZR.Option.TargetLineTypeEnum.Classic;
                break;
        }
    }

    public void ToggleETC(bool value)
    {
        var toggle = EventSystem.current.currentSelectedGameObject;
        switch (toggle.name)
        {
            case "ChkBoxShowFastSlow":
                option.ShowFastSlow = value;
                break;
        }
    }

    public void SliderLimitFPS(float value)
    {
        option.TargetFrameRate = (int)value;
        sliderLimitFPSText.text = option.TargetFrameRate.ToString();
    }

    public void BtnSwitchTag()
    {
        var btn = EventSystem.current.currentSelectedGameObject.GetComponent<EZR.ButtonExtension>();
        if (btn.IsSelected) return;
        transform.Find("GroupSystem").gameObject.SetActive(false);
        transform.Find("GroupSkin").gameObject.SetActive(false);
        transform.Find("GroupSound").gameObject.SetActive(false);
        switch (btn.gameObject.name)
        {
            case "BtnSystem":
                transform.Find("GroupSystem").gameObject.SetActive(true);
                break;
            case "BtnSkin":
                transform.Find("GroupSkin").gameObject.SetActive(true);
                break;
            case "BtnSound":
                transform.Find("GroupSound").gameObject.SetActive(true);
                break;
        }
        updateSystem();
        btn.SetSelected(true);
        EZR.MemorySound.PlaySound("e_click");
    }

    void updateJudgmentOffset()
    {
        var value = transform.Find("GroupSkin/BarOffset/UpDownOffset/Value").GetComponent<Text>();
        if (option.JudgmentOffset == 0)
        {
            value.text = "0";
        }
        else if (option.JudgmentOffset > 0)
        {
            value.text = "+" + option.JudgmentOffset;
        }
        else if (option.JudgmentOffset < 0)
        {
            value.text = option.JudgmentOffset.ToString();
        }
    }

    Coroutine offsetDelayCoroutine;
    public void BtnOffsetSubtractDown()
    {
        addOffset(-1);
        if (offsetDelayCoroutine != null) StopCoroutine(offsetDelayCoroutine);
        offsetDelayCoroutine = StartCoroutine(offsetDelay(-1));
    }
    public void BtnOffsetAddDown()
    {
        addOffset(1);
        if (offsetDelayCoroutine != null) StopCoroutine(offsetDelayCoroutine);
        offsetDelayCoroutine = StartCoroutine(offsetDelay(1));
    }
    public void BtnOffsetUp()
    {
        StopCoroutine(offsetDelayCoroutine);
        offsetDelayCoroutine = null;
    }
    IEnumerator offsetDelay(int value)
    {
        yield return new WaitForSeconds(0.2f);
        for (; ; )
        {
            yield return new WaitForSeconds(0.075f);
            addOffset(value);
        }
    }

    void addOffset(int value)
    {
        option.JudgmentOffset += value;
        updateJudgmentOffset();
        EZR.MemorySound.PlaySound("e_click");
    }

    public void SliderVolume(float value)
    {
        var slider = EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();
        if (slider == null) return;
        switch (slider.name)
        {
            case "SliderMasterVolume":
                option.Volume.Master = (int)slider.value;
                break;
            case "SliderGameVolume":
                option.Volume.Game = (int)slider.value;
                break;
            case "SliderMainVolume":
                option.Volume.Main = (int)slider.value;
                break;
            case "SliderBGMVolume":
                option.Volume.BGM = (int)slider.value;
                break;

        }
    }

    public void ToggleSound(bool value)
    {
        var toggle = EventSystem.current.currentSelectedGameObject;
        switch (toggle.name)
        {
            case "ChkBoxLive3D":
                option.Volume.Live3D = value;
                break;
        }
    }

    public void ClickSound()
    {
        EZR.MemorySound.PlaySound("e_click");
    }

    public void Apply()
    {
        EZR.UserSaveData.SetOption(option);
        EZR.UserSaveData.SaveData();

        EZR.Option.ApplyOption(option);

        Destroy(gameObject);
        EZR.MemorySound.PlaySound("e_click");
    }

    public void Close()
    {
        Destroy(gameObject);
        EZR.MemorySound.PlaySound("e_click");
    }
}
