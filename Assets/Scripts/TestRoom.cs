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
    GameObject[] roundList = new GameObject[256];
    Dictionary<int, Coroutine> roundCoroutineDic = new Dictionary<int, Coroutine>();
    public static List<int> RoundCount = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        EZR.PlayManager.SongName = "Flower";
        EZR.PlayManager.GameMode = EZR.GameMode.Mode.ClubMix8;
        EZR.PlayManager.GameDifficult = EZR.GameDifficult.Difficult.SHD;
        EZR.PlayManager.Reset();
        EZR.PlayManager.LoadPattern();
        genDebugRound();
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
    }

    void genDebugRound()
    {
        for (int i = 0; i < roundList.Length; i++)
        {
            roundList[i] = Instantiate(DebugRound);
            roundList[i].transform.position = new Vector3(
                2f + (i % 12) * 0.4f,
                -4f + (i / 12) * 0.4f
            );
            roundList[i].transform.localScale = new Vector3(1.25f, 1.25f, 1);
            roundList[i].GetComponent<SpriteRenderer>().color = Color.black;
        }
    }

    void OnGUI()
    {
        text.text = EZR.PlayManager.DebugMessage + "\ndelta time (ms): " + EZR.PlayManager.DeltaTime * 1000;
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
}