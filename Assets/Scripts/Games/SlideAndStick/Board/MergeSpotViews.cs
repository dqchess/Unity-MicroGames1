using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
    /// NOTE: This class is UNUSED! But the code exists here, in case we need to make version where TileViewBody DOESN'T have MergeSpotView, and they're all here instead.
    public class MergeSpotViews : MonoBehaviour {
        // Components
        private List<MergeSpotView> views;
        // References
        [SerializeField] private BoardView boardView;
        [SerializeField] private Sprite s_arrow;// TEMP! For debugging.
        
        
        
        // ----------------------------------------------------------------
        //  Game Doers
        // ----------------------------------------------------------------
        public void UpdateVisualsPostMove() {
            DestroyMergeSpotViews(); // We wanna nix any MergeSpotViews for finished-animation.
        }
        public void GoToValues(float loc) {
            for (int i=0; i<views.Count; i++) {
                views[i].GoToValues(loc);
            }
        }
        public void SetValues_To(Board simBoard) {
            RemakeMergeSpotViews(simBoard);
        }
        private void RemakeMergeSpotViews(Board simBoard) {
            DestroyMergeSpotViews(); // Just in case.
            
            for (int i=0; i<simBoard.LastMergeSpots.Count; i++) {
                MergeSpot ms = simBoard.LastMergeSpots[i];
                AddMergeSpotView(ms);
            }
        }


        // ----------------------------------------------------------------
        //  Adding / Removing
        // ----------------------------------------------------------------
        private void DestroyMergeSpotViews() {
            if (views != null) {
                for (int i=0; i<views.Count; i++) {
                    Destroy(views[i].gameObject);
                }
            }
            views = new List<MergeSpotView>();
            // HACK TEMP TEST
            for (int i=transform.childCount-1; i>=0; --i) {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        private void AddMergeSpotView(MergeSpot mergeSpot) {
            // NOTE: DISABLED! This class doesn't do anything right now.
            //// TEMP! For testing!
            //Image newObj = new GameObject().AddComponent<Image>();
            //GameUtils.ParentAndReset(newObj.gameObject, this.transform);
            //newObj.sprite = s_arrow;
            //newObj.color = new Color(1,0,1);
            //newObj.rectTransform.anchorMin = new Vector2(0,1);
            //newObj.rectTransform.anchorMax = new Vector2(0,1);
            //newObj.rectTransform.sizeDelta = new Vector2(boardView.UnitSize*0.4f,boardView.UnitSize*0.4f);
            //newObj.rectTransform.anchoredPosition = boardView.BoardToLocal(mergeSpot.pos);
            //float rotation = -(MathUtils.GetSide(mergeSpot.dir)) * 90;
            //newObj.transform.localEulerAngles = new Vector3(0,0, rotation);
        
            ////MergeSpotView obj = Instantiate(ResourcesHandler.Instance.slideAndStick_mergeSpotView).GetComponent<MergeSpotView>();
            ////obj.Initialize(myTileView, this, mergeSpot);
            ////views.Add(obj);
        }
    }
}
