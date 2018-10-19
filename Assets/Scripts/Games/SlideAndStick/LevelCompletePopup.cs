using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
    public class LevelCompletePopup : MonoBehaviour {
        // Constants
        private const int totalStripes = 50;
        // Components
        [SerializeField] private Button b_nextLevel=null;
        [SerializeField] private RectTransform rt_stripes=null;
        [SerializeField] private RectTransform myRectTransform=null;
        //[SerializeField] private TextMeshProUGUI t_levelName=null;
        private Image[] stripesL;
        private Image[] stripesR;
        // Properties
        public bool DidPressNextButton { get; private set; }



        // ----------------------------------------------------------------
        //  Adding Things
        // ----------------------------------------------------------------
        private void MakeStripes() {
            stripesL = new Image[Mathf.CeilToInt(totalStripes/2)];
            stripesR = new Image[totalStripes-stripesL.Length];
            Vector2 mySize = myRectTransform.rect.size;
            float stripeHeight = mySize.y / (float)totalStripes;
            for (int i=0; i<stripesL.Length; i++) {
                Image newObj = new GameObject().AddComponent<Image>();
                GameUtils.ParentAndReset(newObj.gameObject, rt_stripes);
                newObj.transform.SetAsFirstSibling(); // put behind everything
                newObj.name = "BackingStripe"+i;
                newObj.rectTransform.anchorMax = newObj.rectTransform.anchorMin = new Vector2(0,1);
                newObj.rectTransform.sizeDelta = new Vector2(mySize.x, stripeHeight*2); //HACK TEMP TEST give a lil' extra bloat so there's def no empty space.
                newObj.rectTransform.pivot = new Vector2(0, 1);
                newObj.rectTransform.anchoredPosition = new Vector2(0, -(i*2)*stripeHeight);
                stripesL[i] = newObj;
            }
            for (int i=0; i<stripesR.Length; i++) {
                Image newObj = new GameObject().AddComponent<Image>();
                GameUtils.ParentAndReset(newObj.gameObject, rt_stripes);
                if (i%2==0) {
                    newObj.transform.SetAsFirstSibling(); // put behind everything
                }
                newObj.name = "BackingStripe"+i;
                newObj.rectTransform.anchorMax = newObj.rectTransform.anchorMin = new Vector2(1,1);
                newObj.rectTransform.pivot = new Vector2(1, 1);
                newObj.rectTransform.sizeDelta = new Vector2(mySize.x, stripeHeight*2); //HACK TEMP TEST give a lil' extra bloat so there's def no empty space.
                newObj.rectTransform.anchoredPosition = new Vector2(0, -(i*2+1)*stripeHeight);
                stripesR[i] = newObj;
            }
        }

        // ----------------------------------------------------------------
        //  Appear / Disappear!
        // ----------------------------------------------------------------
        public void Hide() {
            this.gameObject.SetActive(false);
        }
        public void Appear() {
            this.gameObject.SetActive(true);
            // Add stripes if we haven't yet!
            if (stripesL == null) { MakeStripes(); }
            
            DidPressNextButton = false; // We'll set to true when we press Next button!
    
            //this.transform.localScale = new Vector3(1, 0, 1);
            //LeanTween.scale (this.gameObject, Vector3.one, 0.5f).setEaseOutBack();
            //this.transform.localEulerAngles = new Vector3(0, 0, -16);
            //LeanTween.rotateZ(this.gameObject, 0f, 2.0f).setEaseOutElastic();
            
            //Vector2 mySize = myRectTransform.rect.size;
            //float stripeHeight = mySize.y / (float)totalStripes;
            Color colorL = new Color( 98/255f,188/255f,177/255f, 155/255f);
            Color colorR = new Color( 98/255f,112/255f,188/255f, 155/255f);
            Color finalColor = new Color(131/255f,165/255f,199f/255f, 255/255f);
            //Color finalColor = new Color(0.5f,0.5f,0.5f);
            for (int i=0; i<stripesL.Length; i++) {
                float delay = i*0.02f+Random.Range(-0.04f,0.04f);
                stripesL[i].transform.localScale = new Vector3(0, 1, 1);
                stripesL[i].color = colorL;
                LeanTween.scale(stripesL[i].gameObject, Vector3.one, 0.6f).setEaseOutQuart().setDelay(delay);
                LeanTween.color(stripesL[i].rectTransform, finalColor, 2f).setEaseOutQuart().setDelay(delay);
            }
            for (int i=0; i<stripesR.Length; i++) {
                float delay = i*0.02f+Random.Range(-0.04f,0.04f);
                stripesR[i].transform.localScale = new Vector3(0, 1, 1);
                stripesR[i].color = colorR;
                LeanTween.scale(stripesR[i].gameObject, Vector3.one, 0.6f).setEaseOutQuart().setDelay(delay);
                LeanTween.color(stripesR[i].rectTransform, finalColor, 2f).setEaseOutQuart().setDelay(delay);
            }
        }
    
    
        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnPressNextButton() {
            DidPressNextButton = true;
        }
    }
}