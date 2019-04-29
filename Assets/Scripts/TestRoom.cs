using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using PatternUtils;
using System.Runtime.InteropServices;

public class TestRoom : MonoBehaviour
{
    public GameObject Panel;

    public GameObject NoteA;
    public GameObject NoteB;
    public GameObject Flare;
    public GameObject LongFlare;

    bool grooveLight = false;
    Animation grooveLightAnim;

    bool key1update = false;
    Animation line1Anim;
    bool key2update = false;
    Animation line2Anim;
    bool key3update = false;
    Animation line3Anim;
    bool key4update = false;
    Animation line4Anim;

    Transform header;

    int[] currentIndex = new int[8];

    List<NoteInLine>[] noteInLines = new List<NoteInLine>[4];
    FlareAnimCTL[] flarePlayList = new FlareAnimCTL[4];
    FlareAnimCTL[] LongflarePlayList = new FlareAnimCTL[4];

    float fallSpeed = 4f * 2f;
    double position = 0d;
    double positionDelta;

    int readyFrame;
    bool isStarted = false;
    JudgmentAnimCTL judgmentAnim;

    int combo = 0;
    int maxCombo = 0;

    ComboCounter comboCounter;

    // Start is called before the first frame update
    void Start()
    {
        grooveLightAnim = Panel.transform.Find("Groove").GetComponent<Animation>();
        line1Anim = Panel.transform.Find("Keys/4/Line1").GetComponent<Animation>();
        line2Anim = Panel.transform.Find("Keys/4/Line2").GetComponent<Animation>();
        line3Anim = Panel.transform.Find("Keys/4/Line3").GetComponent<Animation>();
        line4Anim = Panel.transform.Find("Keys/4/Line4").GetComponent<Animation>();

        header = Panel.transform.Find("NoteArea/Header");

        for (int i = 0; i < 4; i++)
        {
            noteInLines[i] = new List<NoteInLine>();
        }

        for (int i = 0; i < 4; i++)
        {
            var flare = Instantiate(Flare);
            var longFlare = Instantiate(LongFlare);
            flarePlayList[i] = flare.GetComponent<FlareAnimCTL>();
            LongflarePlayList[i] = longFlare.GetComponent<FlareAnimCTL>();
            flare.transform.SetParent(Panel.transform, false);
            longFlare.transform.SetParent(Panel.transform, false);
            switch (i)
            {
                case 0:
                    flare.transform.localPosition = new Vector3(-106, -186, 0);
                    longFlare.transform.localPosition = new Vector3(-106, -186, 0);
                    break;
                case 1:
                    flare.transform.localPosition = new Vector3(-38, -186, 0);
                    longFlare.transform.localPosition = new Vector3(-38, -186, 0);
                    break;
                case 2:
                    flare.transform.localPosition = new Vector3(36, -186, 0);
                    longFlare.transform.localPosition = new Vector3(36, -186, 0);
                    break;
                case 3:
                    flare.transform.localPosition = new Vector3(104, -186, 0);
                    longFlare.transform.localPosition = new Vector3(104, -186, 0);
                    break;
            }
        }
        var judgment = Instantiate(Panel.GetComponent<Panel>().Judgment);
        judgmentAnim = judgment.GetComponent<JudgmentAnimCTL>();
        judgment.transform.Find("KOOL").GetComponent<Animation>()["KOOL"].normalizedTime = 1;
        judgment.transform.Find("Any").GetComponent<Animation>()["Any"].normalizedTime = 1;
        judgment.transform.Find("KOOL").GetComponent<Animation>().Play("KOOL");
        judgment.transform.Find("Any").GetComponent<Animation>().Play("Any");
        judgment.transform.SetParent(Panel.transform.Find("Judgment"), false);

        comboCounter = Panel.transform.Find("Combo/ComboCounter").GetComponent<ComboCounter>();

        EZR.PlayManager.GameType = EZR.GameType.DJMAX;
        EZR.PlayManager.SongName = "fareast";
        EZR.PlayManager.GameMode = EZR.GameMode.Mode.FourButtons;
        EZR.PlayManager.GameDifficult = EZR.GameDifficult.Difficult.DJMAX_MX;
        EZR.PlayManager.Reset();
        EZR.PlayManager.LoadPattern();
        EZR.PlayManager.LoopStop += loopStop;
        EZR.PlayManager.Groove += groove;
    }

    void StartPlay()
    {
        EZR.Master.InputEvent += inputEvent;
        EZR.Master.MainLoop += judgmentEvent;
        EZR.PlayManager.Start();
        // EZR.PlayManager.Position = 3000;
        // position = EZR.PlayManager.Position;
        isStarted = true;
        var videoPlayer = GameObject.Find("BGASource").GetComponent<VideoPlayer>();
        videoPlayer.url = Path.Combine(EZR.Master.GameResourcesFolder,
            EZR.PlayManager.GameType.ToString(),
            "Ingame",
            EZR.PlayManager.SongName + ".mp4"
        );
        videoPlayer.Play();
    }

    void Update()
    {
        readyFrame++;
        if (readyFrame == 10)
        {
            StartPlay();
        }

        if (grooveLight)
        {
            grooveLight = false;
            grooveLightAnim["GrooveLight"].time = 0;
            if (!grooveLightAnim.isPlaying)
                grooveLightAnim.Play();
        }

        if (key1update)
        {
            if (EZR.Master.Key1State)
            {
                line1Anim.Play("KeyDown");
            }
            else
            {
                line1Anim["KeyUp"].time = 0;
                line1Anim.Play("KeyUp");
            }
            key1update = false;
        }

        if (key2update)
        {
            if (EZR.Master.Key2State)
            {
                line2Anim.Play("KeyDown");
            }
            else
            {
                line2Anim["KeyUp"].time = 0;
                line2Anim.Play("KeyUp");
            }
            key2update = false;
        }

        if (key3update)
        {
            if (EZR.Master.Key3State)
            {
                line3Anim.Play("KeyDown");
            }
            else
            {
                line3Anim["KeyUp"].time = 0;
                line3Anim.Play("KeyUp");
            }
            key3update = false;
        }

        if (key4update)
        {
            if (EZR.Master.Key4State)
            {
                line4Anim.Play("KeyDown");
            }
            else
            {
                line4Anim["KeyUp"].time = 0;
                line4Anim.Play("KeyUp");
            }
            key4update = false;
        }

        for (int i = 0; i < 4; i++)
        {
            while (currentIndex[i] < EZR.PlayManager.TimeLines.Lines[i].Notes.Count &&
            currentIndex[i] < EZR.PlayManager.TimeLines.LinesIndex[i] + 10)
            {
                GameObject note;
                Vector3 pos;
                Pattern.Note patternNote = EZR.PlayManager.TimeLines.Lines[i].Notes[currentIndex[i]];
                switch (i)
                {
                    case 0:
                        note = Instantiate(NoteA);
                        pos = new Vector3(
                            -105,
                           patternNote.position * fallSpeed
                            , 0
                        );
                        break;
                    case 1:
                        note = Instantiate(NoteB);
                        pos = new Vector3(
                            -37,
                           patternNote.position * fallSpeed
                            , 0
                        );
                        break;
                    case 2:
                        note = Instantiate(NoteB);
                        pos = new Vector3(
                            37,
                           patternNote.position * fallSpeed
                            , 0
                        );
                        break;
                    case 3:
                        note = Instantiate(NoteA);
                        pos = new Vector3(
                            105,
                           patternNote.position * fallSpeed
                            , 0
                        );
                        break;
                    default:
                        note = null;
                        pos = Vector3.zero;
                        break;
                }

                var noteInLine = note.GetComponent<NoteInLine>();
                noteInLine.index = currentIndex[i];
                noteInLine.Position = patternNote.position;
                noteInLine.NoteScaleY = note.transform.localScale.y;

                noteInLines[i].Add(noteInLine);

                note.transform.SetParent(header, false);

                note.transform.localPosition = pos;
                var rect = note.GetComponent<RectTransform>();
                noteInLine.NoteHeight = rect.sizeDelta.y;

                if (patternNote.length > 6)
                {
                    noteInLine.NoteLength = patternNote.length;
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x, noteInLine.NoteLength * fallSpeed / noteInLine.NoteScaleY + noteInLine.NoteHeight);
                }

                currentIndex[i]++;
            }
        }

        // Unity Delta Time Position 用于消除音符抖动
        if (isStarted)
        {
            positionDelta = Time.unscaledDeltaTime * ((EZR.PlayManager.TimeLines.BPM / 4d / 60d) * PatternUtils.Pattern.MeasureLength);
            position += positionDelta;
        }

        header.localPosition = new Vector3(0,
            -(float)(position * fallSpeed),
            0
        );

        // 长音符和移除音符
        for (int i = 0; i < 4; i++)
        {
            if (noteInLines[i].Count > 0)
            {
                if (noteInLines[i][0].Position + noteInLines[i][0].NoteLength - EZR.PlayManager.Position < -EZR.JudgmentDelta.MISS)
                {
                    noteInLines[i].RemoveAt(0);
                    continue;
                }
                if (noteInLines[i][0].isLongPressed)
                {
                    var rect = noteInLines[i][0].GetComponent<RectTransform>();
                    noteInLines[i][0].transform.localPosition = new Vector3(
                        noteInLines[i][0].transform.localPosition.x,
                        (float)(EZR.PlayManager.Position * fallSpeed),
                        0
                    );
                    rect.sizeDelta = new Vector2(
                        rect.sizeDelta.x,
                        (float)((noteInLines[i][0].Position + noteInLines[i][0].NoteLength - EZR.PlayManager.Position) *
                        fallSpeed / noteInLines[i][0].NoteScaleY + noteInLines[i][0].NoteHeight)
                    );
                }
            }
        }
    }

    void loopStop()
    {
        EZR.PlayManager.LoopStop -= loopStop;
        EZR.PlayManager.Groove -= groove;
        EZR.Master.InputEvent -= inputEvent;
        EZR.Master.MainLoop -= judgmentEvent;
    }

    void judgmentEvent()
    {
        for (int i = 0; i < 4; i++)
        {
            if (noteInLines[i].Count > 0)
            {
                // FAIL
                if (noteInLines[i][0].NoteLength > 0)
                {

                    if (noteInLines[i][0].isLongPressed)
                    {
                        // 长音连击
                        noteInLines[i][0].LongNoteRate += EZR.PlayManager.PositionDelta;
                        if (noteInLines[i][0].LongNoteRate >= 12)
                        {
                            noteInLines[i][0].LongNoteRate -= 12;
                            judgmentAnim.Play("kool");
                            addCombo();
                            flarePlayList[i].Play();
                        }
                        // 自动结尾
                        if (noteInLines[i][0].Position + noteInLines[i][0].NoteLength <= EZR.PlayManager.Position)
                        {
                            judgmentAnim.Play("kool");
                            addCombo();
                            flarePlayList[i].Play();
                            LongflarePlayList[i].isStop = true;
                            noteInLines[i][0].isDestroy = true;
                            noteInLines[i].RemoveAt(0);
                            goto endif;
                        }
                    }
                    if (noteInLines[i][0].Position + noteInLines[i][0].NoteLength - EZR.PlayManager.Position < -EZR.JudgmentDelta.GOOD / 2f)
                    {
                        judgmentAnim.Play("miss");
                        comboBreak();
                        LongflarePlayList[i].isStop = true;
                        noteInLines[i].RemoveAt(0);
                    }
                }
                else if (noteInLines[i][0].Position - EZR.PlayManager.Position < -EZR.JudgmentDelta.MISS / 2f * EZR.JudgmentDelta.Scale)
                {
                    judgmentAnim.Play("fail");
                    comboBreak();
                    noteInLines[i].RemoveAt(0);
                }
            }

        endif: if (noteInLines[i].Count > 0)
            {
                // Auto play
                if (EZR.PlayManager.isAutoPlay)
                {
                    if (noteInLines[i][0].NoteLength > 6)
                    {
                        // 长音
                        if (!noteInLines[i][0].isLongPressed && noteInLines[i][0].Position <= EZR.PlayManager.Position)
                        {
                            addCombo();
                            judgmentAnim.Play("kool");
                            var note = EZR.PlayManager.TimeLines.Lines[i].Notes[noteInLines[i][0].index];
                            noteInLines[i][0].isLongPressed = true;
                            LongflarePlayList[i].Play();
                            flarePlayList[i].Play();
                            EZR.MemorySound.playSound(note.id, note.vol, note.pan, EZR.MemorySound.Main);
                        }
                        // else if (noteInLines[i][0].isLongPressed &&
                        // noteInLines[i][0].Position + noteInLines[i][0].NoteLength <= EZR.PlayManager.Position)
                        // {
                        //     noteInLines[i][0].isLongPressed = false;
                        //     noteInLines[i][0].isDestroy = true;
                        //     LongflarePlayList[i].isStop = true;
                        //     flarePlayList[i].Play();
                        //     noteInLines[i].RemoveAt(0);
                        // }
                    }
                    else
                    {
                        // 短音
                        if (noteInLines[i][0].Position <= EZR.PlayManager.Position)
                        {
                            addCombo();
                            judgmentAnim.Play("kool");
                            var note = EZR.PlayManager.TimeLines.Lines[i].Notes[noteInLines[i][0].index];
                            noteInLines[i][0].isDestroy = true;
                            flarePlayList[i].Play();
                            EZR.MemorySound.playSound(note.id, note.vol, note.pan, EZR.MemorySound.Main);
                            noteInLines[i].RemoveAt(0);
                        }
                    }
                }
            }
        }
    }

    void groove()
    {
        grooveLight = true;
    }

    void inputEvent(int keyId, bool state)
    {
        switch (keyId)
        {
            case 0:
                key1update = true;
                break;
            case 1:
                key2update = true;
                break;
            case 2:
                key3update = true;
                break;
            case 3:
                key4update = true;
                break;
        }

        // 判定
        if (state)
        {
            Pattern.Note note;
            NoteInLine noteInLine;

            if (noteInLines[keyId].Count > 0)
            {
                noteInLine = noteInLines[keyId][0];
                note = EZR.PlayManager.TimeLines.Lines[keyId].Notes[noteInLine.index];

                double judgmentDelta = Math.Abs(noteInLine.Position - EZR.PlayManager.Position);

                bool isHit = false;
                if (note.length > 6)
                {
                    if (judgmentDelta <= EZR.JudgmentDelta.KOOL / 2f)
                    {
                        judgmentAnim.Play("kool");
                        flarePlayList[keyId].Play();
                        LongflarePlayList[keyId].Play();
                        isHit = true;
                    }
                    else if (judgmentDelta > EZR.JudgmentDelta.KOOL / 2f && judgmentDelta <= EZR.JudgmentDelta.COOL / 2f)
                    {
                        judgmentAnim.Play("cool");
                        flarePlayList[keyId].Play();
                        LongflarePlayList[keyId].Play();
                        isHit = true;
                    }
                    else if (judgmentDelta > EZR.JudgmentDelta.COOL / 2f && judgmentDelta <= EZR.JudgmentDelta.GOOD / 2f)
                    {
                        judgmentAnim.Play("good");
                        flarePlayList[keyId].Play(0.6f);
                        LongflarePlayList[keyId].Play(0.6f);
                        isHit = true;
                    }
                    else if (judgmentDelta > EZR.JudgmentDelta.GOOD / 2f && judgmentDelta <= EZR.JudgmentDelta.MISS / 2f)
                    {
                        comboBreak();
                        judgmentAnim.Play("miss");
                        noteInLines[keyId].RemoveAt(0);
                    }
                    if (isHit)
                    {
                        addCombo();
                        noteInLine.isLongPressed = true;
                    }
                }
                else
                {
                    if (judgmentDelta <= EZR.JudgmentDelta.KOOL / 2f * EZR.JudgmentDelta.Scale)
                    {
                        judgmentAnim.Play("kool");
                        flarePlayList[keyId].Play();
                        isHit = true;
                    }
                    else if (judgmentDelta > EZR.JudgmentDelta.KOOL / 2f * EZR.JudgmentDelta.Scale && judgmentDelta <= EZR.JudgmentDelta.COOL / 2f * EZR.JudgmentDelta.Scale)
                    {
                        judgmentAnim.Play("cool");
                        flarePlayList[keyId].Play();
                        isHit = true;
                    }
                    else if (judgmentDelta > EZR.JudgmentDelta.COOL / 2f * EZR.JudgmentDelta.Scale && judgmentDelta <= EZR.JudgmentDelta.GOOD / 2f * EZR.JudgmentDelta.Scale)
                    {
                        judgmentAnim.Play("good");
                        flarePlayList[keyId].Play(0.6f);
                        isHit = true;
                    }
                    else if (judgmentDelta > EZR.JudgmentDelta.GOOD / 2f * EZR.JudgmentDelta.Scale && judgmentDelta <= EZR.JudgmentDelta.MISS / 2f * EZR.JudgmentDelta.Scale)
                    {
                        comboBreak();
                        judgmentAnim.Play("miss");
                        noteInLines[keyId].RemoveAt(0);
                    }
                    if (isHit)
                    {
                        addCombo();
                        noteInLine.isDestroy = true;
                        noteInLines[keyId].RemoveAt(0);
                    }
                }
            }
            else
            {
                noteInLine = null;
                note = EZR.PlayManager.TimeLines.Lines[keyId].Notes[EZR.PlayManager.TimeLines.Lines[keyId].Notes.Count - 1];
            }

            var channel = EZR.MemorySound.playSound(note.id, note.vol, note.pan, EZR.MemorySound.Main);
            if (noteInLine != null && note.length > 6)
            {
                noteInLine.LongNoteSound = channel;
            }
        }
        else if (noteInLines[keyId].Count > 0)
        {
            if (noteInLines[keyId][0].isLongPressed)
            {
                noteInLines[keyId][0].isLongPressed = false;
                LongflarePlayList[keyId].isStop = true;

                bool isHit = false;
                double judgmentDelta = Math.Abs(noteInLines[keyId][0].Position + noteInLines[keyId][0].NoteLength - EZR.PlayManager.Position);
                if (judgmentDelta <= EZR.JudgmentDelta.KOOL / 2f)
                {
                    judgmentAnim.Play("kool");
                    flarePlayList[keyId].Play();
                    isHit = true;
                }
                else if (judgmentDelta > EZR.JudgmentDelta.KOOL / 2f && judgmentDelta <= EZR.JudgmentDelta.COOL / 2f)
                {
                    judgmentAnim.Play("cool");
                    flarePlayList[keyId].Play();
                    isHit = true;
                }
                else if (judgmentDelta > EZR.JudgmentDelta.COOL / 2f && judgmentDelta <= EZR.JudgmentDelta.GOOD / 2f)
                {
                    judgmentAnim.Play("good");
                    addCombo();
                    flarePlayList[keyId].Play(0.6f);
                }
                else if (judgmentDelta > EZR.JudgmentDelta.GOOD / 2f && judgmentDelta <= EZR.JudgmentDelta.MISS / 2f)
                {
                    comboBreak();
                    judgmentAnim.Play("miss");
                }

                if (isHit)
                {
                    addCombo();
                    noteInLines[keyId][0].isDestroy = true;
                }
                else if (noteInLines[keyId][0].LongNoteSound != null)
                {
                    ((FMOD.Channel)noteInLines[keyId][0].LongNoteSound).stop();
                }
                noteInLines[keyId].RemoveAt(0);
            }
        }
    }

    void addCombo()
    {
        combo++;
        if (combo > maxCombo)
            maxCombo = combo;
        comboCounter.SetCombo(combo);
    }

    void comboBreak()
    {
        combo = 0;
        comboCounter.Clear();
    }
}