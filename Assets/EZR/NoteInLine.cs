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
    public object LongNoteSound;
    public bool isInvalidLongNote = false;

    // TODO 保存Rect
}
