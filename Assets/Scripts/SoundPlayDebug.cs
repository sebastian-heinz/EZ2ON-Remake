using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundPlayDebug : MonoBehaviour
{
    public Text DebugText;
    public GameObject DebugRound;
    GameObject[] roundList = new GameObject[792];
    Dictionary<int, Coroutine> roundCoroutineDic = new Dictionary<int, Coroutine>();
    List<int> RoundCount = new List<int>();
    string debugMessage = "";

    void Start()
    {
        genDebugRound();
        EZR.PlayManager.LoopStop += loopStop;
        EZR.PlayManager.DebugEvent += debugEvent;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < RoundCount.Count; i++)
        {
            GameObject round = roundList[RoundCount[i]];
            if (round == null) continue;
            //round.transform.localScale = new Vector3(1.25f, 1.25f, 1);
            round.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
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
        var roundGroup = GameObject.Find("Canvas/RoundGroup");
        if (roundGroup == null) return;
        for (int i = 0; i < roundList.Length; i++)
        {
            roundList[i] = Instantiate(DebugRound);
            roundList[i].transform.SetParent(roundGroup.transform, false);
            roundList[i].transform.localPosition = new Vector3(
                -567 + (i % 36) * 32,
                -200 + (i / 36) * 32,
                0
            );
            roundList[i].transform.localScale = Vector3.one;
            roundList[i].GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        }
    }

    void OnGUI()
    {
        DebugText.text = debugMessage + "\ndelta time (ms): " + EZR.PlayManager.DeltaTime * 1000;
    }

    IEnumerator roundFade(GameObject round)
    {
        float a = 0;
        Image image = round.GetComponent<Image>();
        for (; ; )
        {
            //round.transform.localScale = Vector3.Lerp(
            //    new Vector3(0.6f, 0.6f, 1),
            //     new Vector3(1.25f, 1.25f, 1),
            //    1 - Mathf.Pow(1 - a, 8)
            //);
            if (a > 1)
            {
                image.color = new Color(0, 0, 0, 0.5f);
                break;
            }
            image.color = Color.Lerp(
                new Color(1, 1, 0, 0.5f),
                new Color(0, 0, 0, 0.5f),
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
    }
}
