using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ExtrudeMatch {
	public class GameUI : MonoBehaviour {
        // Components
        [SerializeField] private Button b_retry=null;
        [SerializeField] private Text t_scoreValue=null;
        [SerializeField] private Text t_bestValue=null;
        // Properties
        private Vector2 retryButtonPosDefault;


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
		private void Start () {
            UpdateScoreTexts(0);
            retryButtonPosDefault = b_retry.transform.localPosition;
            HideRetryButton();
		}



        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnStartLevel() {
            t_scoreValue.color = new Color(1,1,1, 0.8f);
            t_bestValue.color = new Color(1,1,1, 0.8f);
            HideRetryButton();
        }
        public void OnGameOver() {
            t_scoreValue.color = new ColorHSB(94/360f, 0.83f, 1f).ToColor();
            StartCoroutine(Coroutine_AnimateInRetryButton());
        }



        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        private void HideRetryButton() { b_retry.gameObject.SetActive(false); }
        private void ShowRetryButton() { b_retry.gameObject.SetActive(true); }

        public void UpdateScoreTexts(int score) {
            // Current score!
            t_scoreValue.text = score.ToString();
            // Best score!
            int bestScore = SaveStorage.GetInt(SaveKeys.ExtrudeMatch_BestScore, 0);
            t_bestValue.text = bestScore.ToString();
            if (score >= bestScore) { // Current best score!!
                t_bestValue.color = new Color(1, 0.8f, 0f);
            }
        }

        private IEnumerator Coroutine_AnimateInRetryButton() {
            yield return new WaitForSecondsRealtime(0.09f); // wait a moment so we can first register the loss.

            ShowRetryButton();
            float duration = 0.2f;
            b_retry.transform.localPosition = retryButtonPosDefault + new Vector2(0, -50f);
            LeanTween.moveLocal(b_retry.gameObject, retryButtonPosDefault, duration).setEaseOutBack();

            //sm changes
            float a = 0;
            while (a < 0.95f) {
                b_retry.image.color = new Color(1,1,1, a);
                a += (1-a) / 4f;
                yield return null;
            }
            a = 1;
            b_retry.image.color = new Color(1,1,1, a);
        }

	}
}