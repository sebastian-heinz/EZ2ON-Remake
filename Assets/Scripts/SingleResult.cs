using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class SingleResult : MonoBehaviour
{
    public GameObject SingleResultUI;
    void Start()
    {
        var canvas = GameObject.Find("Canvas").transform;
        var singleResultUI = Instantiate(SingleResultUI);
        singleResultUI.transform.SetParent(canvas, false);
        EZR.MemorySound.PlayStream(Path.Combine(EZR.Master.GameResourcesFolder, "BGM", "bgm_result.ogg"), false);
        StartCoroutine(waitEnd());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            End();
    }

    IEnumerator waitEnd()
    {
        yield return new WaitForSecondsRealtime(9);
        End();
    }

    void End()
    {
        SceneManager.LoadScene("SelectSongs");
        EZR.MemorySound.StopStream();
    }
}
