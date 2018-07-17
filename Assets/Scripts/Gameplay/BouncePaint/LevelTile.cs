using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BouncePaint {
    public class LevelTile : MonoBehaviour {
        // Components
        [SerializeField] private Button myButton=null;
        [SerializeField] private Image i_backing=null;
        [SerializeField] private Text t_levelNumber=null;
        [SerializeField] private RectTransform myRectTransform=null;
        // Properties
        private bool isLocked;
        private int levelIndex;
        private Vector2 pos;
        // References
        private LevelSelectController levelSelectController;

        // Getters (Public)
        public int LevelIndex { get { return levelIndex; } }


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize(LevelSelectController _levelSelectController, Transform tf_parent, int _levelIndex, bool _isLocked) {
            levelSelectController = _levelSelectController;
            levelIndex = _levelIndex;
            isLocked = _isLocked;

            this.transform.SetParent(tf_parent);
            this.transform.localScale = Vector3.one;
            this.transform.localPosition = Vector3.zero;
            this.transform.localEulerAngles = Vector3.zero;
            this.gameObject.name = "LevelTile " + levelIndex;

            // Visuals!
            UpdateLockedVisuals();
            t_levelNumber.text = levelIndex.ToString();
        }
        private void UpdateLockedVisuals() {
            myButton.interactable = !isLocked;
            t_levelNumber.color = isLocked ? new Color(0,0,0, 0.5f) : new Color(0,0,0, 0.9f);
        }


        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void SetPosSize(Vector2 _pos, Vector2 _size) {
            pos = _pos;
            myRectTransform.anchoredPosition = pos;
            myRectTransform.sizeDelta = _size;
        }
        private void StartGameAtLevel() {
            SaveStorage.SetInt(SaveKeys.BouncePaint_LastLevelPlayed, levelIndex);
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.BouncePaint_Gameplay);
        }



        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnClick() {
            if (!isLocked) {
                StartGameAtLevel();
            }
        }


        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        float colorOscLoc;
        private void Update() {
            if (isLocked) { return; }

            // Oscillate my color, bro!
            float oscSpeed = 1f;//MathUtils.Sin01(Time.time);
            colorOscLoc += (Time.deltaTime * oscSpeed) * 0.04f;
            //float posScaleX = 1f + MathUtils.Sin01(Time.time*0.3f+2);
            //float posScaleY = 1f + MathUtils.Sin01(Time.time*0.9f+3);
            //float h = (MathUtils.Sin01(colorOscLoc) + Mathf.Abs(pos.x*posScaleX+pos.y*posScaleY)*0.001f) % 1f;
            float posY = pos.y + levelSelectController.ScrollY*0.8f;
            float h = MathUtils.Sin01(colorOscLoc + posY*0.002f);
            h = (h + Mathf.Max(0, Mathf.PerlinNoise(pos.x*2+colorOscLoc, pos.y+colorOscLoc))*0.4f) % 1f;
            i_backing.color = new ColorHSB(h, 0.6f, 1f).ToColor();
        }


        // ----------------------------------------------------------------
        //  Debug
        // ----------------------------------------------------------------
        public void Debug_UnlockMe() {
            isLocked = false;
            UpdateLockedVisuals();
        }

    }
}