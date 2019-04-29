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
    FlarePlay[] flarePlayList = new FlarePlay[4];
    FlarePlay[] LongflarePlayList = new FlarePlay[4];

    float fallSpeed = 3.5f * 2f;
    double position = 0d;

    int readyFrame;
    bool isStarted = false;

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
            flarePlayList[i] = flare.GetComponent<FlarePlay>();
            LongflarePlayList[i] = longFlare.GetComponent<FlarePlay>();
            flare.transform.SetParent(Panel.transform, false);
            longFlare.transform.SetParent(Panel.transform, false);
            switch (i)
            {
                case 0:
                    flare.transform.localPosition = new Vector3(-106, -199, 0);
                    longFlare.transform.localPosition = new Vector3(-106, -199, 0);
                    break;
                case 1:
                    flare.transform.localPosition = new Vector3(-38, -199, 0);
                    longFlare.transform.localPosition = new Vector3(-38, -199, 0);
                    break;
                case 2:
                    flare.transform.localPosition = new Vector3(36, -199, 0);
                    longFlare.transform.localPosition = new Vector3(36, -199, 0);
                    break;
                case 3:
                    flare.transform.localPosition = new Vector3(104, -199, 0);
                    longFlare.transform.localPosition = new Vector3(104, -199, 0);
                    break;
            }
        }

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
        EZR.PlayManager.Start();
        // EZR.PlayManager.Position = 3000;
        // position = EZR.PlayManager.Position;
        isStarted = true;
        GameObject.Find("BGASource").GetComponent<VideoPlayer>().Play();
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

                noteInLines[i].Add(noteInLine);

                note.transform.SetParent(header, false);

                note.transform.localPosition = pos;
                var rect = note.GetComponent<RectTransform>();

                if (patternNote.length > 6)
                {
                    noteInLine.NoteLength = patternNote.length;
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x, patternNote.length * fallSpeed / 1.5f);//1.5要改成变量
                }

                currentIndex[i]++;
            }
        }

        // Unity Delta Time Position 用于消除音符抖动
        if (isStarted)
            position += Time.unscaledDeltaTime * ((EZR.PlayManager.TimeLines.BPM / 4d / 60d) * PatternUtils.Pattern.MeasureLength);

        header.localPosition = new Vector3(0,
            -(float)(position * fallSpeed),
            0
        );

        // 长音符和摧毁音符
        for (int i = 0; i < 4; i++)
        {
            for (int j = noteInLines[i].Count - 1; j >= 0; j--)
            {
                if (noteInLines[i][j].isDestroy ||
                (noteInLines[i][j].Position + noteInLines[i][j].NoteLength - EZR.PlayManager.Position < -64))
                {
                    if (noteInLines[i][j].NoteLength > 0)
                        LongflarePlayList[i].isStop = true;
                    Destroy(noteInLines[i][j].gameObject);
                    noteInLines[i].RemoveAt(j);
                    continue;
                }
                if (noteInLines[i][j].isLongPressed)
                {
                    var rect = noteInLines[i][j].GetComponent<RectTransform>();
                    noteInLines[i][j].transform.localPosition = new Vector3(
                        noteInLines[i][j].transform.localPosition.x,
                        (float)(EZR.PlayManager.Position * fallSpeed),
                        0
                    );
                    rect.sizeDelta = new Vector2(
                        rect.sizeDelta.x,
                        (float)((noteInLines[i][j].Position + noteInLines[i][j].NoteLength - EZR.PlayManager.Position) * fallSpeed / 1.5d)//1.5要改成变量
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
        if (state)
        {
            Pattern.Note note;
            NoteInLine noteInLine;

            if (noteInLines[keyId].Count > 0 &&
                !noteInLines[keyId][0].isLongPressed &&
                !noteInLines[keyId][0].isInvalidLongNote)
            {
                noteInLine = noteInLines[keyId][0];
                note = EZR.PlayManager.TimeLines.Lines[keyId].Notes[noteInLine.index];

                if (noteInLine.Position - EZR.PlayManager.Position < 24)
                {
                    if (note.length > 6)
                    {
                        noteInLine.isLongPressed = true;
                        LongflarePlayList[keyId].isPlay = true;
                    }
                    else
                        noteInLine.isDestroy = true;
                    flarePlayList[keyId].isPlay = true;
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
                noteInLines[keyId][0].isInvalidLongNote = true;
                LongflarePlayList[keyId].isStop = true;
                if (noteInLines[keyId][0].LongNoteSound != null &&
                Mathf.Abs((float)(noteInLines[keyId][0].Position + noteInLines[keyId][0].NoteLength - EZR.PlayManager.Position)) > 24)
                {
                    ((FMOD.Channel)noteInLines[keyId][0].LongNoteSound).stop();
                }
                noteInLines[keyId].RemoveAt(0);
            }
        }
    }
}