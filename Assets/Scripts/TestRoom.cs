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
using PatternUtils;

public class TestRoom : MonoBehaviour
{
    Dictionary<int, FMOD.Sound> SoundList;
    Note[] NoteList;
    Pattern PatternData;
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
            //round.transform.localScale = new Vector3(1.25f, 1.25f, 1);
            round.GetComponent<SpriteRenderer>().color = Color.white;
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
                2f + (i % 12) * 0.4f,
                -4f + (i / 12) * 0.4f
            );
            roundList[i].transform.localScale = new Vector3(1.25f, 1.25f, 1);
            roundList[i].GetComponent<SpriteRenderer>().color = Color.black;
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
        public float vol;
        public float pan;
        public float bpm;
        public int length;
    }

    //读取曲谱
    void LoadPatternData()
    {
        string songsPath = Path.Combine(EZR.Master.GameResourcesFolder, "EZ2DJ\\Songs\\7bc");
        //读json
        string patternFileName = Path.Combine(songsPath, "7streetmix1p-7bc-shd.json");
        Debug.Log(patternFileName);

        PatternData = PatternUtils.Pattern.Parse(File.ReadAllText(patternFileName));

        SoundList = new Dictionary<int, FMOD.Sound>();

        for (int i = 0; i < PatternData.SoundList.Count; i++)
        {
            var soundData = PatternData.SoundList[i];
            string filename = soundData.filename;
            int type = soundData.type;

            if (filename != "")
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
                        byte[] soundBytes = File.ReadAllBytes(fullName);

                        var exinfo = new FMOD.CREATESOUNDEXINFO();
                        exinfo.cbsize = Marshal.SizeOf(exinfo);
                        exinfo.length = (uint)soundBytes.Length;

                        var soundResult = FMODUnity.RuntimeManager.LowlevelSystem.createSound(soundBytes, FMOD.MODE._2D | FMOD.MODE.OPENMEMORY, ref exinfo, out FMOD.Sound sound);

                        if (soundResult == FMOD.RESULT.OK)
                        {
                            //Debug.Log(fullName);
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
        for (int i = 0; i < PatternData.BPMList.Count; i++)
        {
            var bpm = PatternData.BPMList[i];
            if (bpm.type == 3)
            {
                notes.Add(new Note()
                {
                    position = bpm.position,
                    type = bpm.type,
                    id = -1,
                    vol = 0,
                    pan = 0,
                    bpm = bpm.bpm,
                    length = 0
                });
            }
        }

        for (int i = 0; i < PatternData.TrackList.Count; i++)
        {
            var track = PatternData.TrackList[i];
            for (int j = 0; j < track.Notes.Count; j++)
            {
                var note = track.Notes[j];
                if (note.type == 1)
                {
                    int position = note.position;
                    notes.Add(new Note()
                    {
                        position = note.position,
                        type = note.type,
                        id = note.id,
                        vol = note.vol,
                        pan = note.pan,
                        bpm = 0,
                        length = note.length,
                    });
                }
            }
        }

        // 排序
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
            float bpm = 120;
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

                    if (NoteList[index].bpm > 0) bpm = NoteList[index].bpm;

                    if (SoundList.ContainsKey(id))
                    {
                        FMODUnity.RuntimeManager.LowlevelSystem.playSound(SoundList[id], main, true, out FMOD.Channel channel);
                        //FMODUnity.RuntimeManager.LowlevelSystem.playSound(SoundList[id], main, false, out FMOD.Channel channel);
                        channel.setVolume(NoteList[index].vol);
                        channel.setPan(NoteList[index].pan);
                        channel.setPaused(false);
                        roundCount.Add(id);
                    }

                    message = string.Format("[{0}] {1} sound: {2}\n[vol: {3}] [pan: {4}]",
                        (int)(pos / measureLength),
                        NoteList[index].position,
                        PatternData.SoundList[Mathf.Max(0, id)].filename,
                        (int)(NoteList[index].vol * 100), (int)(NoteList[index].pan * 100)
                    );

                    index++;
                    if (index >= NoteList.Length) break;
                }

                Thread.Sleep(16);

                long now = stopwatch.ElapsedTicks;
                deltaTime = (now - lastTime) / 10000000d;
                lastTime = now;

                pos += deltaTime * ((bpm / 4d / 60d) * measureLength);
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

    void OnApplicationQuit()
    {
        TaskExitFlag = 1;
    }
}