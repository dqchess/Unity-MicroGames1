using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
    public class LevelUI : MonoBehaviour {
		// Constants
		private readonly Color color_red = new Color(244/255f, 23/255f, 80/255f);
		private readonly Color color_green = Color.green;
        // Components
        [SerializeField] private GameObject go_scoreBar=null;
        //[SerializeField] private Image i_barBorder=null;
        [SerializeField] private Image i_barFillPossible=null;
        [SerializeField] private Image i_barFillSolidified=null;
        [SerializeField] private Image i_fullBacking=null; // the solid square image that's behind EVERYthing (gameplay AND UI)
        [SerializeField] private LevelCompleteBanner levelCompleteBanner=null;
        [SerializeField] private TextMeshProUGUI t_levelName=null;
        [SerializeField] public  TextMeshProUGUI t_moreLevelsComingSoon=null;
        [SerializeField] private TextMeshProUGUI t_score=null;
		[SerializeField] private TextMeshProUGUI t_scoreRequired=null;
		[SerializeField] private ParticleSystem ps_winLevelA=null;
		[SerializeField] private ParticleSystem ps_winLevelB=null;
        // References
        [SerializeField] private Level myLevel=null;
        // Properties
        //private readonly Color barColor_solid = Grower.color_solid;
        //private readonly Color barColor_possible = Grower.color_growing;
        private Vector2 scoreBarSize; // set in Initialize.

        // Getters (Private)
        private int levelIndex { get { return myLevel.LevelIndex; } }
        private int scoreRequired { get { return myLevel.ScoreRequired; } }



        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize() {
            // Define scoreBarSize! Make it exactly my size.
            scoreBarSize = go_scoreBar.GetComponent<RectTransform>().rect.size;
            // Color elements right-o.
            //i_barBorder.color = barColor_solid;
            //i_barFillPossible.color = barColor_possible;
            //i_barFillSolidified.color = barColor_solid;
            //t_scoreRequired.color = barColor_solid;
			i_fullBacking.color = Grower.color_solid(myLevel.LevelIndex);
			float canvasHeight = myLevel.Canvas.GetComponent<RectTransform>().rect.height;
			i_fullBacking.rectTransform.sizeDelta = new Vector2(i_fullBacking.rectTransform.rect.width, canvasHeight); // Fit full-backing flush with the screen.

            // LevelIndex
            t_levelName.text = "LEVEL " + levelIndex.ToString();
            // Score
            t_scoreRequired.text = TextUtils.AddCommas(scoreRequired);
            //UpdateScoreUI(0);
			// Not playable? Hide the score UI!
			bool isPlayable = levelIndex <= Level.LastLevelIndex;
			go_scoreBar.SetActive(isPlayable);
        }



        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void UpdateScoreUI(float scorePossible, float scoreSolidified) {
            t_score.text = TextUtils.AddCommas(scorePossible);
            // Update bars!
            float fillPossibleWidth = scoreBarSize.x * Mathf.Min(1, scorePossible/(float)scoreRequired);
            float fillSolidifiedWidth = scoreBarSize.x * Mathf.Min(1, scoreSolidified/(float)scoreRequired);
            i_barFillPossible.rectTransform.sizeDelta = new Vector2(fillPossibleWidth, scoreBarSize.y);
            i_barFillSolidified.rectTransform.sizeDelta = new Vector2(fillSolidifiedWidth, scoreBarSize.y);
            // Update color!
            if (scorePossible >= scoreRequired) { // We've potentially won already!!
                t_score.color = t_scoreRequired.color = color_green;
				SetBarFillColor(color_green);
            }
            else { // Haven't won yet...!
				t_score.color = t_scoreRequired.color = Color.white;
				SetBarFillColor(Grower.color_growing);
            }
        }
		/** NOTE! We are using ONE color for both bars!! Disabled multi-colors for now. */
		private void SetBarFillColor(Color color) {
			i_barFillPossible.color = i_barFillSolidified.color = color;
		}


        // ----------------------------------------------------------------
        //  Game Flow Events
        // ----------------------------------------------------------------
        public void OnLoseLevel(LoseReasons loseReason) {
            // Lost because not high enough score? Make score text red!
            if (loseReason == LoseReasons.InsufficientScore) {
				t_score.color = color_red;
				SetBarFillColor(Grower.color_illegal);
            }
        }
        public void OnWinLevel() {
            levelCompleteBanner.AnimateIn();
			// Emit winning burst!!
			ps_winLevelA.Emit(40);
			ps_winLevelB.Emit(8);
        }



		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		ParticleSystem.Particle[] finalBounceParticles;
		private Color cacheColor;
		private void Update() {
			UpdateFinalBounceParticles();
		}
		private void UpdateFinalBounceParticles() {
			if (ps_winLevelA.particleCount > 0) {
				if (finalBounceParticles == null || finalBounceParticles.Length < ps_winLevelA.main.maxParticles) {
					finalBounceParticles = new ParticleSystem.Particle[ps_winLevelA.main.maxParticles];
				}

				// GetParticles is allocation free because we reuse the m_Particles buffer between updates
				int numParticlesAlive = ps_winLevelA.GetParticles(finalBounceParticles);

				// Change only the particles that are alive
				for (int i=0; i<numParticlesAlive; i++) {
					//m_Particles[i].velocity += Vector3.up * m_Drift;
					cacheColor = finalBounceParticles[i].GetCurrentColor(ps_winLevelA);
					//float alpha = -0.2f + MathUtils.Sin01(i+Time.unscaledTime*(28f+i*0.5f))*6f; // glittery flicker!
					float alpha = -0.2f + MathUtils.Sin01(i+Time.unscaledTime*(30f))*5f; // glittery flicker!
					//alpha = Random.Range(Time.timeScale,1f);
					alpha = Mathf.Lerp(alpha, 1, Time.timeScale*0.6f); // Scale the effect based on the current time scale.
					finalBounceParticles[i].startColor = new Color(cacheColor.r,cacheColor.g,cacheColor.b, alpha);
				}

				// Apply the particle changes to the particle system
				ps_winLevelA.SetParticles(finalBounceParticles, numParticlesAlive);
			}
		}


    }
}
