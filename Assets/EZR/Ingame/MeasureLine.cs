using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace EZR
{
    public class MeasureLine : MonoBehaviour
    {
        int index = 0;
        float Index => index * JudgmentDelta.MeasureScale;

        public static List<MeasureLine> MeasureLines = new List<MeasureLine>();

        Image image;
        Animator anim;
        DisplayLoop displayLoop;
        ObjectPool pool;

        void Awake()
        {
            image = GetComponent<Image>();
            anim = GetComponent<Animator>();
        }

        public void Init(int index, DisplayLoop loop, ObjectPool pool)
        {
            enabled = true;
            image.enabled = true;

            this.index = index;

            displayLoop = loop;
            this.pool = pool;

            MeasureLines.Add(this);

            updateMeasure();
        }

        void Update()
        {
            updateMeasure();

            if ((Index * PatternUtils.Pattern.TickPerMeasure) - PlayManager.Position < -(JudgmentDelta.Miss + 2))
            {
                Recycle();
            }
        }

        void updateMeasure()
        {
            if (PlayManager.IsAutoPlay)
            {
                transform.localPosition = new Vector3(
                    0,
                    (float)((Index * PatternUtils.Pattern.TickPerMeasure - displayLoop.Position) * PlayManager.GetSpeed()) + (int)PlayManager.TargetLineType,
                    0
                );
            }
            else
            {
                transform.localPosition = new Vector3(
                    0,
                    (float)((Index * PatternUtils.Pattern.TickPerMeasure - displayLoop.Position) * PlayManager.GetSpeed()) + (int)PlayManager.TargetLineType - PlayManager.JudgmentOffset,
                    0
                );
            }
        }

        public void PlayAnim()
        {
            anim.Play("MeasureLine", 0, 0);
        }

        void OnDestroy()
        {
            if (MeasureLines.Contains(this))
                MeasureLines.Remove(this);
        }

        public void Recycle()
        {
            if (MeasureLines.Contains(this))
                MeasureLines.Remove(this);
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