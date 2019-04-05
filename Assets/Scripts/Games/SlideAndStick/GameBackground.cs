using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {


    // TODO: Move into its own class.
    /** Exactly like Color, but instead of 0-1 values, they're 0-255. */
    public struct Color255 {
        public float r,g,b, a;
        public Color255(float r,float g,float b) {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = 255;
        }
        public Color255(float r,float g,float b, float a) {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        
        public Color ToColor() { return new Color(r/255f,g/255f,b/255f, a/255f); }
    }
    
    public class GameBackground : MonoBehaviour {
        // Components
        [SerializeField] private Image i_backFill=null;
        [SerializeField] private Image i_backGradient=null; // this is the one that rotates.
        // Properties
        private float gradRotSpeed; // how fast the gradient rotates.
        
        // Getters (Private)
        static private List<Color255> GetBGColors(int diff) {
            switch (diff) {
                case Difficulties.Beginner:     return new List<Color255>{
                    new Color255(242,251,139), new Color255(133,185,255) };
                case Difficulties.Easy:         return new List<Color255>{
                    new Color255(255,174,166), new Color255(205,240,180) };
                case Difficulties.Med:          return new List<Color255>{
                    new Color255(133,255,176), new Color255(255,114,160) };
                case Difficulties.Hard:         return new List<Color255>{
                    new Color255(255,213,133), new Color255(201,177,255) };
                case Difficulties.DoubleHard:   return new List<Color255>{
                    new Color255(255,166,193), new Color255(220,174,255) };
                case Difficulties.Impossible:   return new List<Color255>{
                    new Color255( 78, 14, 89), new Color255(138,202,191) };
                case Difficulties.DoubleImpossible: return new List<Color255>{
                    new Color255(175,175,175), new Color255(181, 88,156) };
                default: return new List<Color255>{ // Hmm.
                    new Color255(255,255,255), new Color255(128,128,128) };
            }
        }
    
    
        // ----------------------------------------------------------------
        //  Awake / Start / Destroy
        // ----------------------------------------------------------------
        private void Awake() {
            // Add event listeners!
            GameManagers.Instance.EventManager.SlideAndStick_StartLevelEvent += OnStartLevel;
        }
        private void OnDestroy() {
            // Remove event listeners!
            GameManagers.Instance.EventManager.SlideAndStick_StartLevelEvent -= OnStartLevel;
        }
        private void Start() {
            // Size my gradient right-o!
            float canvasHeight = FindObjectOfType<Canvas>().GetComponent<RectTransform>().rect.height;
            i_backGradient.rectTransform.sizeDelta = new Vector2(canvasHeight,canvasHeight)*1.11f; // add bloat so it still covers screen while rotating.
        }


        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        private void OnStartLevel(Level level) {
            // Set gradient rotation direction.
            gradRotSpeed = 0.06f;
            gradRotSpeed *= level.Board.Difficulty%2==0 ? -1 : 1; // every other difficulty rotates CW/CCW.
            
            // Animate bg colors into place!
            List<Color255> colors = GetBGColors(level.Board.Difficulty);
            Color colorA = colors[0].ToColor();
            Color colorB = colors[1].ToColor();
            LeanTween.cancel(i_backFill.gameObject);
            LeanTween.cancel(i_backGradient.gameObject);
            LeanTween.color(i_backFill.rectTransform, colorA, 0.8f);
            LeanTween.color(i_backGradient.rectTransform, colorB, 0.8f);
        }


        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        private void Update() {
            // Rotate gradient!
            i_backGradient.rectTransform.Rotate(new Vector3(0, 0, gradRotSpeed*Time.timeScale));
        }



    }
}