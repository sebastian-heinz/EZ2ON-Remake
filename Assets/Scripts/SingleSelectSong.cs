using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleSelectSong : MonoBehaviour
{
    public GameObject SelectSongUI;

    void Start()
    {
        var canvas = GameObject.Find("Canvas").transform;
        var selectSongUI = Instantiate(SelectSongUI);
        selectSongUI.transform.SetParent(canvas, false);
    }
}
