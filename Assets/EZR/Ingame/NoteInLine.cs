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
        public void Init(int index, int position, int length, float x, DisplayLoop loop)
        {
            this.Index = index;
            Position = position;

            displayLoop = loop;
            rect = (RectTransform)transform;
            if (displayLoop.NoteUseScale)
                NoteScale = displayLoop.NoteSize;
            else
            {
                NoteScale = 1;
                rect.sizeDelta = new Vector2(displayLoop.NoteSize, rect.sizeDelta.y);
            }
            transform.localScale = new Vector3(NoteScale, NoteScale, 1);

            NoteHeight = rect.sizeDelta.y;


            if (length > 6)
                NoteLength = length;

            initX = x;

            updateNote();
        }

        void Update()
        {
            if (IsLongPressed)
            {
                updateLongNote();
            }
            else
            {
                updateNote();
            }

            if (IsDestroy || Position + NoteLength - PlayManager.Position < -(JudgmentDelta.Miss + 2))
            {
                Destroy(gameObject);
            }
        }

        // 更新位置和长度
        void updateNote()
        {
            if (PlayManager.IsAutoPlay || !PlayManager.IsSimVSync)
            {
                transform.localPosition = new Vector3(
                    initX,
                    (float)((Position - displayLoop.Position) * PlayManager.GetSpeed()),
                    0
                );
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, (float)((double)NoteLength * PlayManager.GetSpeed() / NoteScale) + NoteHeight);
            }
            else
            {
                transform.localPosition = new Vector3(
                    initX,
                    (float)((Position - displayLoop.Position + PlayManager.SimVsyncDelta) * PlayManager.GetSpeed()),
                    0
                );
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, (float)((double)NoteLength * PlayManager.GetSpeed() / NoteScale) + NoteHeight);
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
                    (float)((Position + NoteLength - displayLoop.Position) * PlayManager.GetSpeed() / NoteScale) + NoteHeight
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
                    (float)((Position + NoteLength - displayLoop.Position + PlayManager.SimVsyncDelta) * PlayManager.GetSpeed() / NoteScale) + NoteHeight
                );
            }
        }
    }
}
