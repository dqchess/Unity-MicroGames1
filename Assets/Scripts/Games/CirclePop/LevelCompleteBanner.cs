using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CirclePop {
    public class LevelCompleteBanner : MonoBehaviour {
        // Components
        [SerializeField] private CanvasGroup myCanvasGroup=null;
        [SerializeField] private RectTransform myRectTransform=null;
        [SerializeField] private ParticleSystem ps_up=null;
        [SerializeField] private ParticleSystem ps_down=null;


        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        private void SetMyCanvasGroupAlpha(float _alpha) {
            myCanvasGroup.alpha = _alpha;
        }
        //public void Hide() {
        //    this.gameObject.SetActive(false);
        //}
        public void AnimateIn() {
            myCanvasGroup.alpha = 0;
            this.gameObject.SetActive(true);
            GameUtils.SetParticleSystemEmissionEnabled(ps_up, false);
            GameUtils.SetParticleSystemEmissionEnabled(ps_down, false);

            StartCoroutine(Coroutine_AnimateIn());
        }
        private IEnumerator Coroutine_AnimateIn() {
            myRectTransform.anchoredPosition = new Vector2(-400,-400);
            yield return new WaitForSeconds(0.2f);

            GameUtils.SetParticleSystemEmissionEnabled(ps_up, true);
            GameUtils.SetParticleSystemEmissionEnabled(ps_down, true);
            ps_up.Play();
            ps_down.Play();


            LeanTween.value(this.gameObject, SetMyCanvasGroupAlpha, 0,1, 0.3f);
            LeanTween.moveX(this.gameObject, 0, 0.4f).setEaseOutQuint();
        }


    }
}