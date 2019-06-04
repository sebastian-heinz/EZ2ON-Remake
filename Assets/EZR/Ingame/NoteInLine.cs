using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EZR
{
    public class NoteInLine : MonoBehaviour
    {
        [HideInInspector]
        public int Index;
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
        public JudgmentType LongNoteJudgment;
        [HideInInspector]
        public FMOD.Channel? NoteSound;
        [HideInInspector]
        public float NoteScale;
        [HideInInspector]
        public float NoteHeight;

        RectTransform rect;
        float initX;
        DisplayLoop displayLoop;

        // 初始化音符
        public void Init(int index, int position, float scale, int length, float x, DisplayLoop loop)
        {
            this.Index = index;
            Position = position;

            NoteScale = scale;
            transform.localScale = new Vector3(NoteScale, NoteScale, 1);
            rect = (RectTransform)transform;
            NoteHeight = rect.sizeDelta.y;

            if (length > 6)
                NoteLength = length;

            initX = x;
            displayLoop = loop;

            updateNote();
        }

        void Update()
        {
            updateNote();

            if (IsDestroy || Position + NoteLength - PlayManager.Position < -(JudgmentDelta.Miss + 2))
            {
                Destroy(gameObject);
                return;
            }

            // 长音符按下
            if (IsLongPressed)
            {
                updateLongNote();
            }
        }

        // 更新位置和长度
        void updateNote()
        {
            if (PlayManager.IsAutoPlay || !PlayManager.IsSimVSync)
            {
                transform.localPosition = new Vector3(
                    initX,
                    (float)(Position - displayLoop.Position) * PlayManager.GetSpeed(),
                    0
                );
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, NoteLength * PlayManager.GetSpeed() / NoteScale + NoteHeight);
            }
            else
            {
                transform.localPosition = new Vector3(
                    initX,
                    (float)((Position - displayLoop.Position + PlayManager.SimVsyncDelta) * PlayManager.GetSpeed()),
                    0
                );
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, NoteLength * PlayManager.GetSpeed() / NoteScale + NoteHeight);
            }
        }

        void updateLongNote()
        {
            if (PlayManager.IsAutoPlay || !PlayManager.IsSimVSync)
            {
                transform.localPosition = new Vector3(
                    initX,
                    0,
                    0
                );
                rect.sizeDelta = new Vector2(
                    rect.sizeDelta.x,
                    (float)((Position + NoteLength - displayLoop.Position) * PlayManager.GetSpeed() / NoteScale + NoteHeight)
                );
            }
            else
            {
                transform.localPosition = new Vector3(
                    initX,
                    0,
                    0
                );
                rect.sizeDelta = new Vector2(
                    rect.sizeDelta.x,
                    (float)(((Position + PlayManager.SimVsyncDelta) + NoteLength - displayLoop.Position) * PlayManager.GetSpeed() / NoteScale + NoteHeight)
                );
            }
        }
    }
}
