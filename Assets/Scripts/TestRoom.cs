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

    GameObject[] roundList = new GameObject[792];
    Dictionary<int, Coroutine> roundCoroutineDic = new Dictionary<int, Coroutine>();
    List<int> RoundCount = new List<int>();

    string debugMessage = "";

    bool grooveLight = false;
    Animation grooveLightAnim;

    bool key1state = false;
    Animation line1Anim;
    bool key2state = false;
    Animation line2Anim;
    bool key3state = false;
    Animation line3Anim;
    bool key4state = false;
    Animation line4Anim;

    GameObject noteArea;

    // Start is called before the first frame update
    void Start()
    {
        grooveLightAnim = Panel.transform.Find("Groove").GetComponent<Animation>();
        line1Anim = Panel.transform.Find("Keys/4/Line1").GetComponent<Animation>();
        line2Anim = Panel.transform.Find("Keys/4/Line2").GetComponent<Animation>();
        line3Anim = Panel.transform.Find("Keys/4/Line3").GetComponent<Animation>();
        line4Anim = Panel.transform.Find("Keys/4/Line4").GetComponent<Animation>();

        noteArea = Panel.transform.Find("NoteArea").gameObject;

        EZR.PlayManager.GameType = EZR.GameType.DJMAX;
        EZR.PlayManager.SongName = "fareast";
        EZR.PlayManager.GameMode = EZR.GameMode.Mode.FourButtons;
        EZR.PlayManager.GameDifficult = EZR.GameDifficult.Difficult.DJMAX_HD;
        EZR.PlayManager.Reset();
        EZR.PlayManager.LoadPattern();
        genDebugRound();
        EZR.PlayManager.DebugEvent += debugEvent;
        EZR.PlayManager.LoopStop += loopStop;
        EZR.PlayManager.Groove += groove;
        // EZR.Master.InputEvent += inputEvent;
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

        if (key1state != EZR.Master.Key1State)
        {
            key1state = EZR.Master.Key1State;
            if (key1state)
            {
                line1Anim.Play("KeyDown");
            }
            else
            {
                line1Anim["KeyUp"].time = 0;
                line1Anim.Play("KeyUp");
            }
        }

        if (key2state != EZR.Master.Key2State)
        {
            key2state = EZR.Master.Key2State;
            if (key2state)
            {
                line2Anim.Play("KeyDown");
            }
            else
            {
                line2Anim["KeyUp"].time = 0;
                line2Anim.Play("KeyUp");
            }
        }

        if (key3state != EZR.Master.Key3State)
        {
            key3state = EZR.Master.Key3State;
            if (key3state)
            {
                line3Anim.Play("KeyDown");
            }
            else
            {
                line3Anim["KeyUp"].time = 0;
                line3Anim.Play("KeyUp");
            }
        }

        if (key4state != EZR.Master.Key4State)
        {
            key4state = EZR.Master.Key4State;
            if (key4state)
            {
                line4Anim.Play("KeyDown");
            }
            else
            {
                line4Anim["KeyUp"].time = 0;
                line4Anim.Play("KeyUp");
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
        // EZR.Master.InputEvent -= inputEvent;
    }

    void groove()
    {
        grooveLight = true;
    }

    // void inputEvent(int keyId, bool state)
    // {
    //     switch (keyId)
    //     {
    //         case 0:
    //             key1state = state;
    //             break;
    //         case 1:
    //             key2state = state;
    //             break;
    //         case 2:
    //             key3state = state;
    //             break;
    //         case 3:
    //             key4state = state;
    //             break;
    //     }
    // }
}