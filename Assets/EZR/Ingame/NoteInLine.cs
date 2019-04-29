using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteInLine : MonoBehaviour
{
    public int index;
    public float Position;
    public bool isDestroy = false;
    public bool isLongPressed = false;
    public int NoteLength = 0;
    public double LongNoteRate = 0;
    public object LongNoteSound;
    public float NoteScaleY;
    public float NoteHeight;
    void Update()
    {
        if (isDestroy || Position + NoteLength - EZR.PlayManager.Position < -(EZR.JudgmentDelta.MISS + 1))
        {
            Destroy(gameObject);
        }
    }
    // TODO 保存Rect
}
