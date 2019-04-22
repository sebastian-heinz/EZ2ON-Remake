using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;

public class TestRoom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TestRoom");
        LoadPatternData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //读取曲谱
    void LoadPatternData()
    {
        string patternFileName = Path.Combine(EZR.Master.GameResourcesFolder, "EZ2ON\\Songs\\frozen", "8-frozen-shd.json");
        Debug.Log(patternFileName);

        JObject patternData = PatternUtils.Parse(File.ReadAllText(patternFileName));
        Debug.Log(patternData.ToString());
    }
}
