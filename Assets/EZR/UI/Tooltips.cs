using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EZR
{
    public class Tooltips : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Multiline]
        public string TipsText;
        Transform canvas;
        Coroutine fade;
        Vector2? startPos;

        void Start()
        {
            canvas = GameObject.Find("PersistentCanvas").transform;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (fade != null) StopCoroutine(fade);
            fade = StartCoroutine(fadeIn(eventData.position));
            startPos = null;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (fade != null) StopCoroutine(fade);
            fade = null;
            Master.Tooltips.transform.SetParent(null, false);
            startPos = null;
        }

        void Update()
        {
            if (fade == null || startPos == null) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)canvas,
                (Vector2)startPos,
                null,
                out Vector2 p1
            );
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)canvas,
                Input.mousePosition,
                null,
                out Vector2 p2
            );
            if (Input.anyKeyDown || (p1 - p2).magnitude >= 20)
            {
                if (fade != null) StopCoroutine(fade);
                fade = null;
                Master.Tooltips.transform.SetParent(null, false);
                startPos = null;
            }
        }

        IEnumerator fadeIn(Vector2 pos)
        {
            Master.Tooltips.transform.Find("Text").GetComponent<Text>().text = TipsText;
            var contentSizeFitter = Master.Tooltips.GetComponent<ContentSizeFitter>();
            contentSizeFitter.enabled = false;
            yield return null;
            contentSizeFitter.enabled = true;
            yield return new WaitForSeconds(0.2f);
            startPos = Input.mousePosition;
            Master.Tooltips.transform.SetParent(canvas, false);
            ((RectTransform)Master.Tooltips.transform).position = pos;
            var anim = Master.Tooltips.GetComponent<Animation>();
            anim["Tooltips"].time = 0;
            anim.Play("Tooltips");
        }
    }
}
