using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoLoopAnim : MonoBehaviour
{
    float time = 0;
    public RawImage leftImage;
    public RawImage rightImage;

    // Update is called once per frame
    void Update()
    {
        time = (time + Time.deltaTime) % 1;
        var color = Color.Lerp(Color.white, Color.gray, (time > 0.5f ? 1 - time : time) * 2);
        leftImage.color = color;
        rightImage.color = color;
        var y = Mathf.Lerp(1, 0, time);
        leftImage.uvRect = new Rect(leftImage.uvRect.x, y, leftImage.uvRect.width, leftImage.uvRect.height);
        rightImage.uvRect = new Rect(rightImage.uvRect.x, y, rightImage.uvRect.width, rightImage.uvRect.height);
    }
}
