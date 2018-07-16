using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
    public class Circle : MonoBehaviour {
        // Components
        [SerializeField] private Image i_body=null;
        [SerializeField] private RectTransform myRectTransform=null;
        // Properties
        private bool isOscillating;
        private float growSpeed = 0.8f;
        private float radius;
        //private Rect levelBounds;
        // References
        private GameController gameController;

        // Getters (Public)
        public bool IsOscillating { get { return isOscillating; } }
        public float Radius { get { return radius; } }
        public Vector2 Pos {
            get { return myRectTransform.anchoredPosition; }
            set { myRectTransform.anchoredPosition = value; }
        }
        // Getters (Private)
        private Color bodyColor {
            get { return i_body.color; }
            set { i_body.color = value; }
        }


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize(GameController _gameController, Transform tf_parent, Vector2 _pos, float _radius) {
            gameController = _gameController;
            this.transform.SetParent(tf_parent);
            this.transform.localScale = Vector3.one;
            this.transform.localPosition = Vector3.zero;
            this.transform.localEulerAngles = Vector3.zero;

            //levelBounds = gameController.r_LevelBounds;

            Pos = _pos;
            SetRadius(_radius);
        }


        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        private void SetRadius(float _radius) {
            radius = _radius;
            myRectTransform.sizeDelta = new Vector2(radius*2, radius*2);
        }
        public void SetIsOscillating(bool _isOscillating) {
            isOscillating = _isOscillating;
            bodyColor = isOscillating ? Color.blue : Color.black;
        }

        public void OnIllegalOverlap() {
            SetIsOscillating(false);
            bodyColor = Color.red;
        }


        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        private void Update() {
            if (isOscillating) {
                SetRadius(radius + growSpeed);
            }
        }

    }
}