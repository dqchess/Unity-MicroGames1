using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
    public class PackButton : MonoBehaviour {
        // Components
        [SerializeField] private Image i_bottom=null;
        [SerializeField] private Image i_top=null;
        [SerializeField] private RectTransform myRectTransform=null;
        [SerializeField] private TextMeshProUGUI t_packName=null;
        // References
        private PackData myPackData;
        private PackSelectMenu packSelectMenu;
        
        
        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize(PackSelectMenu _psm, Transform tf_parent) {
            this.packSelectMenu = _psm;
            
            // Parent jazz!
            GameUtils.ParentAndReset(this.gameObject, tf_parent);
        }
        
        
        // ----------------------------------------------------------------
        //  Spawn / Despawn
        // ----------------------------------------------------------------
        public void Despawn() {
            this.gameObject.SetActive(false);
        }
        public void Spawn(PackData _packData) {
            this.gameObject.SetActive(true);
            this.myPackData = _packData;
            
            this.name = "Pack_" + myPackData.MyAddress.pack;
            t_packName.text = myPackData.PackName;
            
            i_top.color = packSelectMenu.CurrentPackColor;
            i_bottom.color = Color.Lerp(packSelectMenu.CurrentPackColor, Color.black, 0.3f); // darker bottom image for depth.
        }
        public void SetPosSize(Vector2 _pos, Vector2 _size) {
            myRectTransform.anchoredPosition = _pos;
            myRectTransform.sizeDelta = _size;
        }
        
        
        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnClick() {
            packSelectMenu.OnClickPackButton(myPackData.MyAddress);
        }
    }
}