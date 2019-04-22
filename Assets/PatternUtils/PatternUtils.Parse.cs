using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public static partial class PatternUtils
{
    public static JObject Parse(string data)
    {
        JObject jobj = new JObject();
        try
        {
            jobj = JObject.Parse(data);
        }
        catch
        {
            //TODO
        }
        return jobj;
    }
}
