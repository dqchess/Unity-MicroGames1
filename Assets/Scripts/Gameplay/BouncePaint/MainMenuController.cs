using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BouncePaint {
    public class MainMenuController : MonoBehaviour {
        // Components
        [SerializeField] private Button b_play;
        [SerializeField] private TextMeshProUGUI t_headerBounce;
        [SerializeField] private TextMeshProUGUI t_headerPaint;



        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        private void Update () {
            float h;
            float s = 1f;
            float b = 0.92f;

            h = (Time.time*0.07f) % 1f;
            t_headerBounce.color = new ColorHSB(h,s,b).ToColor();
            h = (Time.time*0.03f+0.3f) % 1f;
            t_headerPaint.color = new ColorHSB(h,s,b).ToColor();
            h = 1 + (-Time.time*0.05f) % 1f;
            b_play.GetComponent<Image>().color = new ColorHSB(h,s,b).ToColor();
        }



        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        protected void OpenScene (string sceneName) {
            UnityEngine.SceneManagement.SceneManager.LoadScene (sceneName);
        }
        public void OpenGameplayScene() {
            OpenScene(SceneNames.BouncePaint_Gameplay);
        }



    }
}