using UnityEngine;
using System.Collections.Generic;

namespace EZR
{
    public class MeasureLine : MonoBehaviour
    {
        float index = 0;

        public static List<MeasureLine> MeasureLines = new List<MeasureLine>();

        Animator anim;
        DisplayLoop displayLoop;

        public void Init(float index, DisplayLoop loop)
        {
            this.index = index;
            displayLoop = loop;
            anim = GetComponent<Animator>();
            MeasureLines.Add(this);

            updateMeasure();
        }

        void Update()
        {
            updateMeasure();

            if ((index * PatternUtils.Pattern.TickPerMeasure) - PlayManager.Position < -(JudgmentDelta.Miss + 2))
            {
                Destroy(gameObject);
            }
        }

        void updateMeasure()
        {
            if (PlayManager.IsAutoPlay)
            {
                transform.localPosition = new Vector3(
                    0,
                    (float)((index * PatternUtils.Pattern.TickPerMeasure - displayLoop.Position) * PlayManager.RealFallSpeed) + (int)PlayManager.TargetLineType,
                    0
                );
            }
            else
            {
                transform.localPosition = new Vector3(
                    0,
                    (float)((index * PatternUtils.Pattern.TickPerMeasure - displayLoop.Position) * PlayManager.RealFallSpeed) + (int)PlayManager.TargetLineType - PlayManager.JudgmentOffset,
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
    }
}