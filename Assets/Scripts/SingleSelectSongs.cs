using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleSelectSongs : MonoBehaviour
{
    public GameObject SelectSongsUI;

    void Start()
    {
        var canvas = GameObject.Find("Canvas").transform;
        var selectSongsUI = Instantiate(SelectSongsUI);
        selectSongsUI.transform.SetParent(canvas, false);
    }
}
