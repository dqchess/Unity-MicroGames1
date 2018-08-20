using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WaveTap {
	public class LevelUI : MonoBehaviour {
        // Components
        [SerializeField] private Image i_playerBounds=null; // the "backing" of the level, essentially.
		[SerializeField] private TextMeshProUGUI t_levelName=null;
		[SerializeField] public  TextMeshProUGUI t_moreLevelsComingSoon=null;
		[SerializeField] private ParticleSystem ps_winLevelA=null;
		[SerializeField] private ParticleSystem ps_winLevelB=null;
		// References
		[SerializeField] private Level myLevel=null;

		// Getters (Private)
		private int levelIndex { get { return myLevel.LevelIndex; } }



		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize() {
			// LevelIndex
			t_levelName.text = "LEVEL " + levelIndex.ToString();
            // PlayerBounds image. Make top/bottom fit flush to where Player turns around!
            Player player = myLevel.Player;
            float top = player.PosYMax + player.Radius;
            float bottom = player.PosYMin - player.Radius;
            i_playerBounds.rectTransform.sizeDelta = new Vector2(600, top-bottom);
            i_playerBounds.rectTransform.anchoredPosition = new Vector2(0, (top+bottom)*0.5f); // put its center at the midpoint between top and bottom bounds.
		}


		// ----------------------------------------------------------------
		//  Game Flow Events
		// ----------------------------------------------------------------
		public void OnLoseLevel(LoseReasons loseReason) {
			
		}
		public void OnWinLevel() {
			// Emit winning burst!!
			ps_winLevelA.Emit(40);
			ps_winLevelB.Emit(8);
		}



		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		ParticleSystem.Particle[] winParticles;
		private Color cacheColor;
		private void Update() {
			UpdateFinalBounceParticles();
		}
		private void UpdateFinalBounceParticles() {
			if (ps_winLevelA.particleCount > 0) {
				if (winParticles == null || winParticles.Length < ps_winLevelA.main.maxParticles) {
					winParticles = new ParticleSystem.Particle[ps_winLevelA.main.maxParticles];
				}

				// GetParticles is allocation free because we reuse the m_Particles buffer between updates
				int numParticlesAlive = ps_winLevelA.GetParticles(winParticles);

				// Change only the particles that are alive
				for (int i=0; i<numParticlesAlive; i++) {
					//m_Particles[i].velocity += Vector3.up * m_Drift;
					cacheColor = winParticles[i].GetCurrentColor(ps_winLevelA);
					//float alpha = -0.2f + MathUtils.Sin01(i+Time.unscaledTime*(28f+i*0.5f))*6f; // glittery flicker!
					float alpha = -0.2f + MathUtils.Sin01(i+Time.unscaledTime*(30f))*5f; // glittery flicker!
					//alpha = Random.Range(Time.timeScale,1f);
					alpha = Mathf.Lerp(alpha, 1, Time.timeScale*0.6f); // Scale the effect based on the current time scale.
					winParticles[i].startColor = new Color(cacheColor.r,cacheColor.g,cacheColor.b, alpha);
				}

				// Apply the particle changes to the particle system
				ps_winLevelA.SetParticles(winParticles, numParticlesAlive);
			}
		}


	}
}
