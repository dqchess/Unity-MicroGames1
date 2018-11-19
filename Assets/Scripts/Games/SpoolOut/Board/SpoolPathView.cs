using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpoolOut {
	public class SpoolPathView : MonoBehaviour {
		// Components
		[SerializeField] private ImageLines il_body=null;
		[SerializeField] private Image i_pathEnd=null;
		[SerializeField] private RectTransform myRectTransform=null;
		// Properties
		private float diameterStart, diameterEnd;
		// References
		[SerializeField] private SpoolView mySpoolView=null;

		// Getters (Private)
		private BoardView myBoardView { get { return mySpoolView.MyBoardView; } }
		private Color BodyColor { get { return mySpoolView.PathColor; } }
		private Spool mySpool { get { return mySpoolView.MySpool; } }
		private List<Vector2Int> pathSpaces { get { return mySpool==null ? null : mySpool.PathSpaces; } }
		private int NumPathPoses { get { return pathSpaces==null ? 0 : pathSpaces.Count; } }
		private float UnitSize { get { return myBoardView.UnitSize; } }


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize() {
			this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, -0.1f); // render the line over everything.
			myRectTransform.anchoredPosition = Vector2.zero;
			il_body.Initialize();

			// Size me riiight.
			float lineWidth = UnitSize * 0.72f;
			il_body.SetThickness(lineWidth);
			i_pathEnd.rectTransform.sizeDelta = new Vector2(UnitSize*0.64f, UnitSize*0.64f);
			// Color me goood.
			SetEndsColors(BodyColor);
			SetLineColor(BodyColor);

			Hide();
		}



		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		//	private void UpdateCanSubmitPath() {
		////		canSubmitPath = boardViewRef!=null && boardViewRef.MyLevel.CanSubmitPath(path);
		//		canSubmitPath = path.CanSubmit;
		//	}

		private void Hide() { SetComponentsEnabled(false); }
		private void Show() {
			WholesaleRemakeVisuals();
			SetComponentsEnabled(true);
		}
		private void SetComponentsEnabled(bool isEnabled) {
			il_body.enabled = isEnabled;
			i_pathEnd.enabled = isEnabled;
		}
		private void SetEndsColors(Color color) {
			i_pathEnd.color = color;
		}
		private void SetLineColor(Color color) {
			il_body.SetColor(color);
		}
		private void SetLineAlpha(float alpha) {
			il_body.SetAlpha(alpha);
		}

		public void SetEndHighlightAlpha(float alpha) {
			i_pathEnd.enabled = alpha > 0.05f; // Hide end if no alpha.
			i_pathEnd.color = Color.Lerp(BodyColor,Color.black, 0.2f + alpha*0.3f);
		}


		public void WholesaleRemakeVisuals() {
			// Positions!
			int numPoses = NumPathPoses;
			SetComponentsEnabled(numPoses > 0);

			il_body.SetNumPoints(numPoses);
			for (int i=0; i<numPoses; i++) {
				Vector3 thisPos = new Vector3(myBoardView.BoardToX(pathSpaces[i].x), myBoardView.BoardToY(pathSpaces[i].y), 0);
				il_body.SetPoint(i, thisPos);
			}
			if (numPoses > 0) {
				Vector3 posEnd = il_body.GetPointLast();
				i_pathEnd.rectTransform.anchoredPosition = posEnd;
			}

			UpdateColors();
		}
		private void UpdateColors() {
//			// Level's over!
//			if (myLevel.IsLevelOver) {
//				SetEndsColors(Colors.pathNeutralEnds);
//				SetLineColor(Colors.pathWon);
//			}
//			// CAN submit!
//			else if (CanSubmitPath) {
//				//			i_pathEndRing.enabled = true;
//				SetEndsColors(Colors.pathNeutralEnds);
//				SetLineColor(Colors.pathSubmittable);
//			}
//			// CANNOT submit!
//			else {
//				//			i_pathEndRing.enabled = false;
//				// We've already made this word!
//				if (path!=null && path.AlreadyMadeWord) {
//					SetEndsColors(Colors.pathAlreadySubmittedThisWordEnd);
//					SetLineColor(Colors.pathAlreadySubmittedThisWord);
//				}
//				// EDITING; can't submit!
//				else if (IsEditingPath) {//myLevel.IsEditingPath(path)) {
//					SetEndsColors(Colors.pathNeutralEnds);
//					SetLineColor(Colors.pathEditing);
//				}
//				// Not editing; can't submit.
//				else {
//					SetEndsColors(Colors.pathNeutralEnds);
//					SetLineColor(Colors.pathNeutral);
//				}
//			}
		}


		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
//		private void Update() {
//			if (path == null) { return; } // No path? No dice.
//
//			UpdateEndsDiameter();
//			UpdateLineAlphaDiameter();
//		}
//		private void UpdateLineAlphaDiameter() {
//			// CAN submit...!
//			if (CanSubmitPath) {
//				float alpha = 0.8f + Mathf.Sin(Time.time*7f)*0.2f;
//				SetLineAlpha(Colors.pathWon.a * alpha);
//			}
//		}
//		private void UpdateEndsDiameter() {
//			diameterStart = UnitSize*0.4f;
//			bool isEndOnPathTarget = IsEndOnPathTarget();
//			// We're on a PathTarget!
//			if (isEndOnPathTarget) {
//				if (IsEditingPath) { // Editing! Make big, no movement.
//					diameterEnd = UnitSize * 0.94f;
//				}
//				else if (CanSubmitPath) { // CAN submit!
//					diameterEnd = UnitSize * 0.92f;
//				}
//				else { // CAN'T submit. Slow diameter oscillation.
//					diameterEnd = UnitSize * MathUtils.SinRange(0.92f, 0.98f, Time.time*4.5f);
//				}
//			}
//			// End is in empty space.
//			else {
//				if (IsEditingPath) { // Editing! Make big, no movement.
//					diameterEnd = UnitSize * 0.75f;
//				}
//				else if (CanSubmitPath) { // CAN submit!
//					diameterEnd = UnitSize * 0.7f;
//				}
//				else { // CAN'T submit. Slow diameter oscillation.
//					diameterEnd = UnitSize * MathUtils.SinRange(0.58f, 0.61f, Time.time*4.5f);
//				}
//			}
//			// Apply diameter.
//			GameUtils.SizeUIGraphic(i_pathStart, diameterStart,diameterStart);
//			GameUtils.SizeUIGraphic(i_pathEnd, diameterEnd,diameterEnd);
//			GameUtils.SizeUIGraphic(i_pathEndRing, diameterEnd*0.8f,diameterEnd*0.8f);
//		}



	}
}

