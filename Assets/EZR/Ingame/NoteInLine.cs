using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteInLine : MonoBehaviour
{
    [HideInInspector]
    public int index;
    [HideInInspector]
    public int Position;
    [HideInInspector]
    public bool IsDestroy = false;
    [HideInInspector]
    public bool IsLongPressed = false;
    [HideInInspector]
    public int NoteLength = 0;
    [HideInInspector]
    public int LongNoteCombo = 0;
    [HideInInspector]
    public EZR.JudgmentType LongNoteJudgment;
    [HideInInspector]
    public object NoteSound;
    [HideInInspector]
    public float NoteScale;
    [HideInInspector]
    public float NoteHeight;

    RectTransform rect;
    float initX;

    // 初始化音符
    public void Init(int index, int position, float scale, int length, float x)
    {
        this.index = index;
        this.Position = position;

        NoteScale = scale;
        transform.localScale = new Vector3(NoteScale, NoteScale, 1);
        rect = GetComponent<RectTransform>();
        NoteHeight = rect.sizeDelta.y;

        if (length > 6)
            NoteLength = length;

        initX = x;

        updateNote();
    }

    void Update()
    {
        updateNote();

        if (IsDestroy || Position + NoteLength - EZR.PlayManager.Position < -(EZR.JudgmentDelta.Miss + 1))
        {
            Destroy(gameObject);
            return;
        }

        // 长音符按下
        if (IsLongPressed)
        {
            transform.localPosition = new Vector3(
                initX,
                (float)(EZR.PlayManager.Position * EZR.PlayManager.GetSpeed()),
                0
            );
            rect.sizeDelta = new Vector2(
                rect.sizeDelta.x,
                (float)((Position + NoteLength - EZR.PlayManager.Position) * EZR.PlayManager.GetSpeed() / NoteScale + NoteHeight)
            );
        }
    }

    // 更新位置和长度
    void updateNote()
    {
        transform.localPosition = new Vector3(
            initX,
            Position * EZR.PlayManager.GetSpeed(),
            0
        );
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, NoteLength * EZR.PlayManager.GetSpeed() / NoteScale + NoteHeight);
    }
}
