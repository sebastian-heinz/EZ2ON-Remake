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
        public float Position;
        [HideInInspector]
        public bool IsDestroy = false;
        [HideInInspector]
        public bool IsLongPressed = false;
        [HideInInspector]
        public float NoteLength = 0;
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
        public void Init(int index, float position, float length, float x, DisplayLoop loop)
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
            if (PlayManager.IsAutoPlay)
            {
                transform.localPosition = new Vector3(
                    initX,
                    (float)((Position - displayLoop.Position) * PlayManager.RealFallSpeed) + (int)PlayManager.TargetLineType,
                    0
                );
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, (float)((double)NoteLength * PlayManager.RealFallSpeed / NoteScale) + NoteHeight);
            }
            else
            {
                transform.localPosition = new Vector3(
                    initX,
                    (float)((Position - displayLoop.Position) * PlayManager.RealFallSpeed) + (int)PlayManager.TargetLineType - PlayManager.JudgmentOffset,
                    0
                );
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, (float)((double)NoteLength * PlayManager.RealFallSpeed / NoteScale) + NoteHeight);
            }
        }

        void updateLongNote()
        {
            if (PlayManager.IsAutoPlay)
            {
                transform.localPosition = new Vector3(
                    initX,
                    (int)PlayManager.TargetLineType,
                    0
                );
                rect.sizeDelta = new Vector2(
                    rect.sizeDelta.x,
                    (float)((Position + NoteLength - displayLoop.Position) * PlayManager.RealFallSpeed / NoteScale) + NoteHeight
                );
            }
            else
            {
                transform.localPosition = new Vector3(
                    initX,
                    (int)PlayManager.TargetLineType,
                    0
                );
                rect.sizeDelta = new Vector2(
                    rect.sizeDelta.x,
                    (float)(((Position + NoteLength - displayLoop.Position) * PlayManager.RealFallSpeed - PlayManager.JudgmentOffset) / NoteScale) + NoteHeight
                );
            }
        }
    }
}
