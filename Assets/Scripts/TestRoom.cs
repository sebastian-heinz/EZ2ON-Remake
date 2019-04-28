using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using PatternUtils;
using System.Runtime.InteropServices;

public class TestRoom : MonoBehaviour
{
    public Text text;
    public GameObject DebugRound;

    public GameObject Panel;

    public GameObject NoteA;
    public GameObject NoteB;
    public GameObject Flare;

    GameObject[] roundList = new GameObject[792];
    Dictionary<int, Coroutine> roundCoroutineDic = new Dictionary<int, Coroutine>();
    List<int> RoundCount = new List<int>();

    string debugMessage = "";

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
    FlarePlay[] flareList = new FlarePlay[4];

    float fallSpeed = 3.5f * 2f;

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
            flareList[i] = flare.GetComponent<FlarePlay>();
            flare.transform.SetParent(Panel.transform, false);
            switch (i)
            {
                case 0:
                    flare.transform.localPosition = new Vector3(-106, -199, 0);
                    break;
                case 1:
                    flare.transform.localPosition = new Vector3(-38, -199, 0);
                    break;
                case 2:
                    flare.transform.localPosition = new Vector3(36, -199, 0);
                    break;
                case 3:
                    flare.transform.localPosition = new Vector3(104, -199, 0);
                    break;
            }
        }

        EZR.PlayManager.GameType = EZR.GameType.DJMAX;
        EZR.PlayManager.SongName = "glorydaykr";
        EZR.PlayManager.GameMode = EZR.GameMode.Mode.FourButtons;
        EZR.PlayManager.GameDifficult = EZR.GameDifficult.Difficult.DJMAX_HD;
        EZR.PlayManager.Reset();
        EZR.PlayManager.LoadPattern();
        genDebugRound();
        EZR.PlayManager.DebugEvent += debugEvent;
        EZR.PlayManager.LoopStop += loopStop;
        EZR.PlayManager.Groove += groove;
        EZR.Master.InputEvent += inputEvent;
        EZR.PlayManager.Start();
    }

    void Update()
    {
        for (int i = 0; i < RoundCount.Count; i++)
        {
            GameObject round = roundList[RoundCount[i]];
            //round.transform.localScale = new Vector3(1.25f, 1.25f, 1);
            round.GetComponent<SpriteRenderer>().color = Color.white;
            if (roundCoroutineDic.ContainsKey(RoundCount[i]))
            {
                StopCoroutine(roundCoroutineDic[RoundCount[i]]);
                roundCoroutineDic[RoundCount[i]] = StartCoroutine(roundFade(round));
            }
            else
                roundCoroutineDic.Add(RoundCount[i], StartCoroutine(roundFade(round)));
        }
        RoundCount.Clear();

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
                switch (i)
                {
                    case 0:
                        note = Instantiate(NoteA);
                        pos = new Vector3(
                            -105,
                            EZR.PlayManager.TimeLines.Lines[i].Notes[currentIndex[i]].position * fallSpeed + 14
                            , 0
                        );
                        break;
                    case 1:
                        note = Instantiate(NoteB);
                        pos = new Vector3(
                            -37,
                            EZR.PlayManager.TimeLines.Lines[i].Notes[currentIndex[i]].position * fallSpeed + 14
                            , 0
                        );
                        break;
                    case 2:
                        note = Instantiate(NoteB);
                        pos = new Vector3(
                            37,
                            EZR.PlayManager.TimeLines.Lines[i].Notes[currentIndex[i]].position * fallSpeed + 14
                            , 0
                        );
                        break;
                    case 3:
                        note = Instantiate(NoteA);
                        pos = new Vector3(
                            105,
                            EZR.PlayManager.TimeLines.Lines[i].Notes[currentIndex[i]].position * fallSpeed + 14
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

                noteInLines[i].Add(noteInLine);

                note.transform.SetParent(header, false);

                note.transform.localPosition = pos;

                currentIndex[i]++;
            }
        }

        header.localPosition = new Vector3(0,
            (float)(-EZR.PlayManager.Position * fallSpeed + 14), 0
        );

        for (int i = 0; i < 4; i++)
        {
            for (int j = noteInLines[i].Count - 1; j >= 0; j--)
            {
                if (noteInLines[i][j].isDestroy || noteInLines[i][j].y < 0)
                {
                    Destroy(noteInLines[i][j].gameObject);
                    noteInLines[i].RemoveAt(j);
                }
            }
        }
    }

    void genDebugRound()
    {
        for (int i = 0; i < roundList.Length; i++)
        {
            roundList[i] = Instantiate(DebugRound);
            roundList[i].transform.position = new Vector3(
                -6f + (i % 36) * 0.32f,
                -4f + (i / 36) * 0.32f
            );
            roundList[i].transform.localScale = Vector3.one;
            roundList[i].GetComponent<SpriteRenderer>().color = Color.black;
        }
    }

    void OnGUI()
    {
        text.text = debugMessage + "\ndelta time (ms): " + EZR.PlayManager.DeltaTime * 1000;
    }

    IEnumerator roundFade(GameObject round)
    {
        float a = 0;
        SpriteRenderer spriteRenderer = round.GetComponent<SpriteRenderer>();
        for (; ; )
        {
            //round.transform.localScale = Vector3.Lerp(
            //    new Vector3(0.6f, 0.6f, 1),
            //     new Vector3(1.25f, 1.25f, 1),
            //    1 - Mathf.Pow(1 - a, 8)
            //);
            if (a > 1)
            {
                spriteRenderer.color = Color.black;
                break;
            }
            spriteRenderer.color = Color.Lerp(
                Color.yellow,
                Color.black,
                Mathf.Sqrt(1 - Mathf.Pow(1 - a, 2))
            );
            a += Time.deltaTime;
            yield return null;
        }
    }

    void debugEvent(string message, int id)
    {
        debugMessage = message;
        RoundCount.Add(id);
    }

    void loopStop()
    {
        EZR.PlayManager.LoopStop -= loopStop;
        EZR.PlayManager.DebugEvent -= debugEvent;
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
        if (!state) return;

        Pattern.Note note;
        NoteInLine noteInLine;

        if (noteInLines[keyId].Count == 0)
        {
            note = EZR.PlayManager.TimeLines.Lines[keyId].Notes[EZR.PlayManager.TimeLines.Lines[keyId].Notes.Count - 1];
        }
        else
        {
            noteInLine = noteInLines[keyId][0];
            if (noteInLine.y < 500)
            {
                noteInLine.isDestroy = true;
                flareList[keyId].isPlay = true;
            }
            note = EZR.PlayManager.TimeLines.Lines[keyId].Notes[noteInLine.index];
        }

        EZR.MemorySound.playSound(note.id, note.vol, note.pan, EZR.MemorySound.Main);
    }
}