using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteInLine : MonoBehaviour
{
    public GameObject Flare;
    public GameObject LongFlare;

    [HideInInspector]
    public int index;
    [HideInInspector]
    public float Position;
    [HideInInspector]
    public bool IsDestroy = false;
    [HideInInspector]
    public bool IsLongPressed = false;
    [HideInInspector]
    public int NoteLength = 0;
    [HideInInspector]
    public int LongNoteCombo = 0;
    [HideInInspector]
    public string LongNoteJudgment;
    [HideInInspector]
    public object NoteSound;
    [HideInInspector]
    public float NoteScale;
    [HideInInspector]
    public float NoteHeight;

    RectTransform rect;

    public void Init(float scale)
    {
        NoteScale = scale;
        transform.localScale = new Vector3(NoteScale, NoteScale, 1);
        rect = GetComponent<RectTransform>();
        NoteHeight = rect.sizeDelta.y;
    }
    void Update()
    {
        if (IsDestroy || Position + NoteLength - EZR.PlayManager.Position < -(EZR.JudgmentDelta.MISS + 1))
        {
            Destroy(gameObject);
            return;
        }

        if (IsLongPressed)
        {
            // 需要改成视觉position
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                (float)(EZR.PlayManager.Position * EZR.PlayManager.MeasureScale * EZR.PlayManager.FallSpeed),
                0
            );
            rect.sizeDelta = new Vector2(
                rect.sizeDelta.x,
                (float)((Position + NoteLength - EZR.PlayManager.Position) *
                EZR.PlayManager.MeasureScale * EZR.PlayManager.FallSpeed / NoteScale + NoteHeight)
            );
        }
    }
}
