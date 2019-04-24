using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

public class TestRoom : MonoBehaviour
{
    Dictionary<int, FMOD.Sound> SoundList;
    Note[] NoteList;
    JObject PatternData;
    double deltaTime;

    public Text text;
    string message = "";

    public GameObject DebugRound;
    GameObject[] roundList = new GameObject[256];
    Dictionary<int, Coroutine> roundCoroutineDic = new Dictionary<int, Coroutine>();
    List<int> roundCount = new List<int>();

    byte TaskExitFlag = 0;

    [DllImport("winmm.dll")]
    internal static extern uint timeBeginPeriod(uint period);

    [DllImport("winmm.dll")]
    internal static extern uint timeEndPeriod(uint period);

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TestRoom");
        LoadPatternData();
        genDebugRound();
        PlaySound();
    }

    void Update()
    {
        for (int i = 0; i < roundCount.Count; i++)
        {
            GameObject round = roundList[roundCount[i]];
            round.transform.localScale = new Vector3(1, 1, 1);
            round.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            if (roundCoroutineDic.ContainsKey(roundCount[i]))
            {
                StopCoroutine(roundCoroutineDic[roundCount[i]]);
                roundCoroutineDic[roundCount[i]] = StartCoroutine(roundFade(round));
            }
            else
                roundCoroutineDic.Add(roundCount[i], StartCoroutine(roundFade(round)));
        }
        roundCount.Clear();
    }

    void genDebugRound()
    {
        for (int i = 0; i < roundList.Length; i++)
        {
            roundList[i] = Instantiate(DebugRound);
            roundList[i].transform.position = new Vector3(
                1f + (i % 16) * 0.4f,
                -2f + (i / 16) * 0.4f
            );
            roundList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
    }

    void OnGUI()
    {
        text.text = message + "\ndelta time (ms): " + deltaTime * 1000;
    }

    struct Note
    {
        public int position;
        public int type;
        public int id;
        public int vol;
        public int pan;
        public int bpm;
        public int length;
    }

    //读取曲谱
    void LoadPatternData()
    {
        string songsPath = Path.Combine(EZR.Master.GameResourcesFolder, "EZ2ON\\Songs\\kamui");
        //读json
        string patternFileName = Path.Combine(songsPath, "8-kamui-shd.json");
        Debug.Log(patternFileName);

        PatternData = PatternUtils.Parse(File.ReadAllText(patternFileName));
        Debug.Log(PatternData.ToString());

        SoundList = new Dictionary<int, FMOD.Sound>();

        for (int i = 0; i < ((JArray)PatternData["soundList"]).Count; i++)
        {
            var jsound = PatternData["soundList"][i];
            string filename = (string)jsound["filename"];
            int type = jsound["type"] != null ? (int)jsound["type"] : 0;

            if (filename != null)
            {
                string extName = "wav";
                string fullName;
                for (int j = 0; j < 3; j++)
                {
                    switch (j)
                    {
                        case 1:
                            extName = "mp3";
                            break;
                        case 2:
                            extName = "ogg";
                            break;
                    }
                    fullName = Path.Combine(songsPath, Path.ChangeExtension(filename, extName));
                    if (File.Exists(fullName))
                    {
                        var soundResult = FMODUnity.RuntimeManager.LowlevelSystem.createSound(fullName, FMOD.MODE._2D, out FMOD.Sound sound);
                        if (soundResult == FMOD.RESULT.OK)
                        {
                            //FMOD.Channel channel;
                            //switch (type)
                            //{
                            //    case 1:
                            //        break;
                            //    default:
                            //        SoundList.Add(i, sound);
                            //        break;
                            //}
                            SoundList.Add(i, sound);
                            break;
                        }
                    }
                }
            }
        }

        List<Note> notes = new List<Note>();
        for (int i = 0; i < ((JArray)PatternData["bpmList"]).Count; i++)
        {
            var jbpm = PatternData["bpmList"][i];
            if ((int)jbpm["type"] == 3)
            {
                int position = (int)jbpm["position"];
                notes.Add(new Note()
                {
                    position = position,
                    type = (int)jbpm["type"],
                    id = -1,
                    vol = 0,
                    pan = 0,
                    bpm = (int)jbpm["bpm"],
                    length = 0
                });
            }
        }

        for (int i = 0; i < ((JArray)PatternData["trackList"]).Count; i++)
        {
            var jtrack = PatternData["trackList"][i];
            foreach (JProperty track in jtrack.Children())
            {
                for (int j = 0; j < ((JArray)jtrack[track.Name]).Count; j++)
                {
                    var snote = (JObject)jtrack[track.Name][j];
                    if ((int)snote["type"] == 1)
                    {
                        int position = (int)snote["position"];
                        notes.Add(new Note()
                        {
                            position = position,
                            type = (int)snote["type"],
                            id = (int)snote["id"],
                            vol = (int)snote["vol"],
                            pan = (int)snote["pan"],
                            bpm = 0,
                            length = (int)snote["length"]
                        });
                    }
                }
            }
        }

        NoteList = notes.ToArray();
        Note temp;
        for (int i = 0; i < NoteList.Length - 1; i++)
        {
            for (int j = 0; j < NoteList.Length - 1 - i; j++)
            {
                if (NoteList[j].position > NoteList[j + 1].position)
                {
                    temp = NoteList[j];
                    NoteList[j] = NoteList[j + 1];
                    NoteList[j + 1] = temp;
                }
            }
        }
        //foreach (var note in NoteList)
        //{
        //    Debug.Log(JObject.FromObject(note).ToString(Newtonsoft.Json.Formatting.None));
        //}
    }

    void PlaySound()
    {
        FMODUnity.RuntimeManager.LowlevelSystem.getDSPBufferSize(out uint bl, out int nb);
        Debug.Log(bl + "," + nb);

        Task.Run(() =>
        {
            //当前位置
            double pos = 0d;
            //bpm
            int bpm = 120;
            //每小节划分
            int measureLength = 192;
            int index = 0;

            FMODUnity.RuntimeManager.LowlevelSystem.createChannelGroup("main", out FMOD.ChannelGroup main);

            timeBeginPeriod(1);
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            long lastTime = stopwatch.ElapsedTicks;

            while (index < NoteList.Length)
            {
                if (TaskExitFlag == 1) return;

                while (NoteList[index].position <= pos)
                {
                    int id = NoteList[index].id;
                    float vol = Mathf.Pow(NoteList[index].vol / 127f, 5.5f);
                    float pan = (NoteList[index].pan - 64) / 127f;

                    message = string.Format("[{0}] {1} sound: {2}\n[vol: {3}] [pan: {4}]",
                        (int)(pos / measureLength),
                        NoteList[index].position,
                        (string)PatternData["soundList"][Mathf.Max(0, id)]["filename"],
                        (int)(vol * 100), (int)(pan * 100)
                    );

                    if (NoteList[index].bpm > 0) bpm = NoteList[index].bpm;
                    if (SoundList.ContainsKey(id))
                    {
                        FMODUnity.RuntimeManager.LowlevelSystem.playSound(SoundList[id], main, true, out FMOD.Channel channel);
                        //FMODUnity.RuntimeManager.LowlevelSystem.playSound(SoundList[id], main, false, out FMOD.Channel channel);
                        channel.setVolume(vol);
                        channel.setPan(pan);
                        channel.setPaused(false);
                        roundCount.Add(id);
                    }

                    index++;
                    if (index >= NoteList.Length) break;
                }

                Thread.Sleep(1);

                long now = stopwatch.ElapsedTicks;
                deltaTime = (now - lastTime) / 10000000d;
                lastTime = now;

                pos += deltaTime * ((new Mehroz.Fraction(bpm, 4) / 60) * measureLength).ToDouble();
            }

            stopwatch.Stop();
            timeEndPeriod(1);
        });
    }

    IEnumerator roundFade(GameObject round)
    {
        float a = 0;
        SpriteRenderer spriteRenderer = round.GetComponent<SpriteRenderer>();
        for (; ; )
        {
            round.transform.localScale = Vector3.Lerp(
                new Vector3(0.6f, 0.6f, 1),
                 new Vector3(1.25f, 1.25f, 1),
                1 - Mathf.Pow(1 - a, 8)
            );
            if (a > 1)
            {
                spriteRenderer.color = Color.black;
                break;
            }
            spriteRenderer.color = Color.Lerp(
                Color.yellow,
                Color.black,
                1 - Mathf.Pow(1 - a, 8)
            );
            a += Time.deltaTime;
            yield return null;
        }
    }

    void OnApplicationQuit()
    {
        TaskExitFlag = 1;
    }
}