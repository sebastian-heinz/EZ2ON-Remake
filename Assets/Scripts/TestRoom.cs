using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

public class TestRoom : MonoBehaviour
{
    Dictionary<int, FMOD.Sound> SoundList;
    Note[] NoteList;
    JObject PatternData;
    double deltaTime;
    public Text text;
    string message = "";

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TestRoom");
        LoadPatternData();
        PlaySound();
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

    void OnGUI()
    {
        text.text = message + "\ndelta time: " + deltaTime;
    }

    void PlaySound()
    {
        FMODUnity.RuntimeManager.LowlevelSystem.createChannelGroup("main", out FMOD.ChannelGroup main);
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

            double lastTime = new TimeSpan(DateTime.Now.Ticks).TotalSeconds;

            while (index < NoteList.Length)
            {
                while (NoteList[index].position <= pos)
                {
                    message = string.Format("[{0}] {1} sound: {2}",
                        (int)(pos / measureLength),
                        NoteList[index].position,
                        (string)PatternData["soundList"][Mathf.Max(0, NoteList[index].id)]["filename"]
                        );

                    if (NoteList[index].bpm > 0) bpm = NoteList[index].bpm;
                    if (SoundList.ContainsKey(NoteList[index].id))
                    {
                        FMODUnity.RuntimeManager.LowlevelSystem.playSound(SoundList[NoteList[index].id], main, false, out FMOD.Channel channel);
                    }
                    index++;
                }

                Thread.Sleep(1);

                double now = new TimeSpan(DateTime.Now.Ticks).TotalSeconds;
                deltaTime = now - lastTime;
                lastTime = now;

                pos += deltaTime * ((new Mehroz.Fraction(bpm, 4) / 60) * measureLength).ToDouble();
                //debugText.text = Convert.ToInt32(pos / measureLength).ToString();
                //debugText.text = pos.ToString();
            }
        });
    }

    //读取曲谱
    void LoadPatternData()
    {
        string songsPath = Path.Combine(EZR.Master.GameResourcesFolder, "EZ2ON\\Songs\\frozen");
        //读json
        string patternFileName = Path.Combine(songsPath, "8-frozen-shd.json");
        Debug.Log(patternFileName);

        PatternData = PatternUtils.Parse(File.ReadAllText(patternFileName));
        Debug.Log(PatternData.ToString());

        //FMODUnity.RuntimeManager.LowlevelSystem.createChannelGroup("bgm", out FMOD.ChannelGroup bgm);
        //FMODUnity.RuntimeManager.LowlevelSystem.createChannelGroup("keySound", out FMOD.ChannelGroup keySound);

        SoundList = new Dictionary<int, FMOD.Sound>();

        for (int i = 0; i < ((JArray)PatternData["soundList"]).Count; i++)
        {
            var jsound = PatternData["soundList"][i];
            string filename = (string)jsound["filename"];
            int type = jsound["type"] != null ? (int)jsound["type"] : 0;
            string fullName;
            if (filename != null)
            {
                fullName = Path.Combine(songsPath, Path.ChangeExtension(filename, "ogg"));
                var soundResult = FMODUnity.RuntimeManager.LowlevelSystem.createSound(fullName, FMOD.MODE._2D | FMOD.MODE.CREATECOMPRESSEDSAMPLE, out FMOD.Sound sound);
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
}
