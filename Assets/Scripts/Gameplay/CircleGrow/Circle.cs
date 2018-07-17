using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
    public class Circle : MonoBehaviour {
        // Constants
        private readonly Color color_oscillating = new Color(250/255f, 200/255f, 110/255f);
        private readonly Color color_solid = new Color(37/255f, 166/255f, 170/255f);
        private readonly Color color_illegal = new Color(255/255f, 132/255f, 118/255f);
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
            bodyColor = isOscillating ? color_oscillating : color_solid;
        }

        public void OnIllegalOverlap() {
            SetIsOscillating(false);
            bodyColor = color_illegal;
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