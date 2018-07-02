using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BouncePaint {
    public class Block : MonoBehaviour {
        // Components
        [SerializeField] private Image i_body=null;
        [SerializeField] private Image i_hitBox=null;
        [SerializeField] private RectTransform myRectTransform=null;
        // Properties
        private bool isPainted=false;
        private Rect hitRect;
        private Rect bodyRect;
        // References
        private GameController gameController;

        // Getters (Public)
        public bool IsPainted { get { return isPainted; } }
        public Rect HitRect { get { return hitRect; } }
        // Getters (Private)
        private Color bodyColor {
            get { return i_body.color; }
            set { i_body.color = value; }
        }
        private Vector2 bodyPos {
            get { return i_body.rectTransform.anchoredPosition; }
            set { i_body.rectTransform.anchoredPosition = value; }
        }


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize(GameController _gameController, RectTransform rt_parent, Vector2 _pos, Vector2 _size) {
            gameController = _gameController;
            this.transform.SetParent(rt_parent);
            this.transform.localScale = Vector3.one;
            this.transform.localEulerAngles = Vector3.zero;
            myRectTransform.anchoredPosition = Vector2.zero;

            bodyRect = new Rect(_pos, _size);
            hitRect = new Rect(bodyRect);
            // Offset and shrink the hitRect!
            hitRect.size += new Vector2(0, -12);
            hitRect.position += new Vector2(0, 52);
            // Bloat the hitRect's width a bit (in case of high-speed x-movement).
            hitRect.size += new Vector2(20, 0);
            hitRect.position += new Vector2(-10, 0);

            bodyColor = new Color(0,0,0, 0.4f);

            i_body.rectTransform.sizeDelta = bodyRect.size;
            i_body.rectTransform.anchoredPosition = bodyRect.position;
            i_hitBox.rectTransform.sizeDelta = hitRect.size;
            i_hitBox.rectTransform.anchoredPosition = hitRect.position;
        }



        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        /// Paints me! :)
        public void OnPlayerBounceOnMe(Color playerColor) {
            // Paint me!
            if (!isPainted) {
                isPainted = true;
                bodyColor = playerColor;
            }
            // Push me down!
            bodyPos += new Vector2(0, -16f);
        }
        //public void OnPlayerBounceOnMe() {
        //}



        // ----------------------------------------------------------------
        //  FixedUpdate
        // ----------------------------------------------------------------
        private void FixedUpdate() {
            UpdatePos();
        }
        private void UpdatePos() {
            float yOffset = 0;
            if (gameController.IsLevelComplete) {
                yOffset = Mathf.Sin(Time.time*4f+bodyPos.x*0.016f) * 9f;
            }

            bodyPos += new Vector2(
                (bodyRect.position.x-bodyPos.x) * 0.3f,
                (bodyRect.position.y+yOffset-bodyPos.y) * 0.3f);
        }

    }
}