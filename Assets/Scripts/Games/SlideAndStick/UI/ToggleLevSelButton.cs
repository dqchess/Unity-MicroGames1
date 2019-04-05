using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
    public class ToggleLevSelButton : MonoBehaviour {
        // Constants
        private readonly Color colorStack = new Color(1,1,1, 0.94f);//new Color(0.7f,0.7f,0.7f);
        private readonly Color colorX = new Color(1,1,1, 0.94f);
        // Components
        [SerializeField] private Image i_bar0=null;
        [SerializeField] private Image i_bar1=null;
        [SerializeField] private Image i_bar2=null;
        // Properties
        //private Color colorStack; // color of bar images when in stack position. Set in Start.
        private Vector2 bar0PosStack;
        private Vector2 bar2PosStack;


        private void Start() {
            //colorStack = i_bar0.color;
            bar0PosStack = i_bar0.rectTransform.anchoredPosition;
            bar2PosStack = i_bar2.rectTransform.anchoredPosition;
        }


        public void SetOpenLoc(float loc) {
            
            Color color = Color.Lerp(colorStack, colorX, loc);
            i_bar0.color = color;
            i_bar2.color = color;
            
            
            GameUtils.SetUIGraphicColorWithCompoundAlpha(i_bar1, color, 1-loc);
            i_bar0.transform.localEulerAngles = new Vector3(0,0, Mathf.Lerp(0, 45, loc));
            i_bar2.transform.localEulerAngles = new Vector3(0,0, Mathf.Lerp(0,-45, loc));
            i_bar0.rectTransform.anchoredPosition = Vector2.Lerp(bar0PosStack, Vector2.zero, loc);
            i_bar2.rectTransform.anchoredPosition = Vector2.Lerp(bar2PosStack, Vector2.zero, loc);
            
            
            
            //LeanTween.cancel(i_bar0.gameObject);
            //LeanTween.cancel(i_bar1.gameObject);
            //LeanTween.cancel(i_bar2.gameObject);
            
            //float duration = 0.5f;
            //if (isX) {
            //    LeanTween.rotateZ(i_bar0.gameObject,  45, duration).setEaseOutBack();
            //    LeanTween.rotateZ(i_bar2.gameObject, -45, duration).setEaseOutBack();
            //    LeanTween.alpha(i_bar1.rectTransform, 0, duration);
            //}
            //else {
            //    LeanTween.rotateZ(i_bar0.gameObject, 0, duration).setEaseOutBack();
            //    LeanTween.rotateZ(i_bar2.gameObject, 0, duration).setEaseOutBack();
            //    LeanTween.alpha(i_bar1.rectTransform, 1, duration);
            //}
        }
        
        
    }
}