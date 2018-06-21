using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BouncePaint {
    public class PaintSpace : MonoBehaviour {
        // Components
        [SerializeField] private Image i_body=null;
        [SerializeField] private Image i_hitBox=null;
        //[SerializeField] private RectTransform myRectTransform=null;
        // Properties
        private bool isPainted=false;
        private Rect hitRect;
        private Rect bodyRect;

        // Getters
        public bool IsPainted { get { return isPainted; } }
        public Rect HitRect { get { return hitRect; } }

        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize(RectTransform rt_parent, Vector2 _pos, Vector2 _size) {
            this.transform.SetParent(rt_parent);
            this.transform.localScale = Vector3.one;
            this.transform.localEulerAngles = Vector3.zero;

            bodyRect = new Rect(_pos, _size);
            hitRect = new Rect(bodyRect);
            hitRect.size += new Vector2(0, -20);
            hitRect.position += new Vector2(0, 55);

            i_body.rectTransform.sizeDelta = bodyRect.size;
            i_body.rectTransform.anchoredPosition = bodyRect.position;
            i_hitBox.rectTransform.sizeDelta = hitRect.size;
            i_hitBox.rectTransform.anchoredPosition = hitRect.position;

        }



        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void PaintMe() {
            isPainted = true;
            i_body.color = Color.yellow;
        }

    }
}