using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EZR
{
    public class NoteInLine : MonoBehaviour
    {
        public enum NoteType
        {
            None = -1,
            A,
            B,
            C,
            D
        }

        public NoteType Type = NoteType.A;
        [HideInInspector]
        public int Index;
        int position = 0;
        public float Position => position * JudgmentDelta.MeasureScale;
        [HideInInspector]
        public bool IsDestroy = false;
        [HideInInspector]
        public bool IsLongPressed = false;
        int noteLength = 0;
        public float NoteLength => noteLength * JudgmentDelta.MeasureScale;
        [HideInInspector]
        public int LongNoteCount { get; private set; }
        [HideInInspector]
        public int LongNoteCombo = 0;
        [HideInInspector]
        public JudgmentType LongNoteJudgment;
        [HideInInspector]
        public FMOD.Channel? NoteSound;

        float noteScale;
        float noteHeight;
        Image image;
        RectTransform rect;
        float initX;
        DisplayLoop displayLoop;
        ObjectPool pool;

        void Awake()
        {
            image = GetComponent<Image>();
            rect = (RectTransform)transform;
            noteHeight = rect.sizeDelta.y;
        }

        // 初始化音符
        public void Init(int index, int position, int length, float x, DisplayLoop loop, ObjectPool pool)
        {
            IsDestroy = false;
            enabled = true;
            image.enabled = true;

            this.Index = index;
            this.position = position;

            displayLoop = loop;
            this.pool = pool;

            if (displayLoop.NoteUseScale)
                noteScale = displayLoop.NoteSize;
            else
            {
                noteScale = transform.localScale.y;
                rect.sizeDelta = new Vector2(displayLoop.NoteSize / noteScale, rect.sizeDelta.y);
            }
            transform.localScale = new Vector3(noteScale, noteScale, 1);

            IsLongPressed = false;
            if (length > 6)
            {
                noteLength = length;
                LongNoteCount = Mathf.Max(1, noteLength / Judgment.LongNoteComboStep);
            }
            else
            {
                noteLength = 0;
                LongNoteCount = 0;
            }
            LongNoteCombo = 0;

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
                Recycle();
            }
        }

        // 更新位置和长度
        void updateNote()
        {
            if (PlayManager.IsAutoPlay)
            {
                transform.localPosition = new Vector3(
                    initX,
                    (float)((Position - displayLoop.Position) * PlayManager.GetSpeed()) + (int)PlayManager.TargetLineType,
                    0
                );
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, (float)((double)NoteLength * PlayManager.GetSpeed() / noteScale) + noteHeight);
            }
            else
            {
                transform.localPosition = new Vector3(
                    initX,
                    (float)((Position - displayLoop.Position) * PlayManager.GetSpeed()) + (int)PlayManager.TargetLineType - PlayManager.JudgmentOffset,
                    0
                );
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, (float)((double)NoteLength * PlayManager.GetSpeed() / noteScale) + noteHeight);
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
                    (float)((Position + NoteLength - displayLoop.Position) * PlayManager.GetSpeed() / noteScale) + noteHeight
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
                    (float)(((Position + NoteLength - displayLoop.Position) * PlayManager.GetSpeed() - PlayManager.JudgmentOffset) / noteScale) + noteHeight
                );
            }
        }

        public void Recycle()
        {
            enabled = false;
            image.enabled = false;
            pool.Put(gameObject);
        }
        public void Recycle(ObjectPool pool)
        {
            enabled = false;
            image.enabled = false;
            pool.Put(gameObject);
        }
    }
}
