using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BouncePaint {
	public class GameUI : MonoBehaviour {
        // Components
        [SerializeField] private Button b_retry=null;
        //[SerializeField] private Text t_levelName=null;
		[SerializeField] private GameObject go_debugUI;
        // Properties
        private Vector2 retryButtonPosDefault;


		// ----------------------------------------------------------------
		//  Start
		// ----------------------------------------------------------------
		private void Start () {
            retryButtonPosDefault = b_retry.transform.localPosition;

			HideDebugUI();
            HideRetryButton();
		}


		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
        public void OnStartLevel(int levelIndex) {
			//t_levelName.text = levelIndex.ToString();
            HideRetryButton();
		}
        public void OnGameOver() {
            StartCoroutine(Coroutine_AnimateInRetryButton());
        }
        private IEnumerator Coroutine_AnimateInRetryButton() {
            yield return new WaitForSecondsRealtime(0.25f); // wait a tight moment so we can first register the loss.

            ShowRetryButton();
            float duration = 0.28f;
            //b_retry.image.color = Color.clear;
            //LeanTween.alpha(b_retry.image.gameObject, 1f, duration);
            b_retry.transform.localPosition = retryButtonPosDefault + new Vector2(0, -60f);
            LeanTween.moveLocal(b_retry.gameObject, retryButtonPosDefault, duration).setEaseOutBack();

            // Manually animating :P
            float a = 0;
            while (a < 0.95f) {
                b_retry.image.color = new Color(1,1,1, a);
                a += (1-a) / 6f;
                yield return null;
            }
            a = 1;
            b_retry.image.color = new Color(1,1,1, a);

            //LeanTween.value(b_retry.image.color.a, 0, 1f, duration).setOnUpdate( (float val)=>(){
            //    UnityEngine.UI.RawImage r = gameObject.getComponent<UnityEngine.UI.RawImage>();
            //    Color c = r.color;
            //    c.a = val;
            //    r.color = c;
            //});
        }


        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void ToggleDebugUI() { go_debugUI.SetActive(!go_debugUI.activeSelf); }
        private void HideDebugUI() { go_debugUI.SetActive(false); }
        private void ShowDebugUI() { go_debugUI.SetActive(true); }

        private void HideRetryButton() { b_retry.gameObject.SetActive(false); }
        private void ShowRetryButton() { b_retry.gameObject.SetActive(true); }


	}
}