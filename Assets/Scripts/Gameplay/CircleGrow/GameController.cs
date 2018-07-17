using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
    public enum GameStates { Playing, GameOver, PostLevel }

    public class GameController : BaseGameController {
        // Components
        [SerializeField] private Image i_levelBounds=null;
        private List<Circle> circles;
        // Properties
        private GameStates gameState;
        private float timeWhenLevelEnded;
        private Rect r_levelBounds;
        // References
        [SerializeField] private Canvas canvas=null;
        [SerializeField] private GameUI ui=null;


        // Getters (Public)
        //public Rect r_LevelBounds { get { return r_levelBounds; } }
        // Getters (Private)
        private Vector2 GetBestPosForNewCircle() {
            Vector2 randPos;
            // Kinda sloppy! Brute-force-y.
            randPos = GetOpenPosForNewCircle(200);
            if (randPos.x != Mathf.NegativeInfinity) { return randPos; }
            randPos = GetOpenPosForNewCircle(100); // TODO: CLean up this negative infinity awkwardness checksings.
            if (randPos.x != Mathf.NegativeInfinity) { return randPos; }
            randPos = GetOpenPosForNewCircle(50);
            if (randPos.x != Mathf.NegativeInfinity) { return randPos; }
            randPos = GetOpenPosForNewCircle(10);
            if (randPos.x != Mathf.NegativeInfinity) { return randPos; }
            Debug.Log("Couldn't find open pos for new circle!");
            return Vector2.negativeInfinity;
        }
        private Vector2 GetOpenPosForNewCircle(float newRadius) {
            Vector2 pos;
            int safetyCount=0;
            do {
                pos = new Vector2(Random.Range(r_levelBounds.xMin,r_levelBounds.xMax), Random.Range(r_levelBounds.yMin,r_levelBounds.yMax));
                if (CanAddCircleAtPos(pos, newRadius)) { break; }
                if (safetyCount++>499) {
                    //Debug.Log("Couldn't find open pos for new circle!");
                    return Vector2.negativeInfinity;
                }
            }
            while(true);
            return pos;
        }
        private bool IsCircleIllegalOverlap(Circle circle) {
            return IsCircleAtPos(circle.Pos, circle.Radius) || !IsCircleInBounds(circle.Pos, circle.Radius);
        }
        private bool CanAddCircleAtPos(Vector2 pos, float radius) {
            return !IsCircleAtPos(pos, radius) && IsCircleInBounds(pos, radius);
        }
        private bool IsCircleInBounds(Vector2 pos, float radius) {
            if (pos.x-radius < r_levelBounds.xMin) { return false; }
            if (pos.x+radius > r_levelBounds.xMax) { return false; }
            if (pos.y-radius < r_levelBounds.yMin) { return false; }
            if (pos.y+radius > r_levelBounds.yMax) { return false; }
            return true;
        }
        private bool IsCircleAtPos(Vector2 pos, float radius) {
            foreach (Circle c in circles) {
                if (c.Pos==pos && c.Radius==radius) { continue; } // Skip itself.
                if (DoCirclesOverlap(c.Pos,c.Radius, pos,radius)) { return true; }
            }
            return false; // We're good!
        }
        //private bool IsCircleOverlappingAnother(Circle circle) {
        //    foreach (Circle c in circles) {
        //        if (c == circle) { continue; } // Skip itself of course.
        //        if (DoCirclesOverlap(c, circle)) { return true; }
        //    }
        //    return false; // We're good!
        //}
        private bool DoCirclesOverlap(Circle circleA, Circle circleB) {
            return DoCirclesOverlap(circleA.Pos,circleA.Radius, circleB.Pos,circleB.Radius);
        }
        private bool DoCirclesOverlap(Vector2 posA,float radiusA, Vector2 posB,float radiusB) {
            float dist = Vector2.Distance(posA,posB);
            return dist < radiusA+radiusB;
        }



        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        override protected void Start () {
            base.Start();

            //levelBounds = new Rect(-300,-300, 600,600);
            //i_levelBounds.anchoredPosition = levelBounds.position;
            //i_levelBounds.sizeDelta = levelBounds.size;
            r_levelBounds = i_levelBounds.rectTransform.rect;

            RestartLevel();
        }


        private void DestroyCircles() {
            if (circles != null) {
                for (int i=circles.Count-1; i>=0; --i) {
                    Destroy(circles[i].gameObject);
                }
            }
            circles = new List<Circle>();
        }



        // ----------------------------------------------------------------
        //  Game Doers
        // ----------------------------------------------------------------
        private void AddNewCircle() {
            float startingRadius = 5f;
            Vector2 randPos = GetBestPosForNewCircle();
            // Can't find a suitable position to put this circle? We're outta room!
            if (randPos.x == Mathf.NegativeInfinity) {
                SetGameOver();
            }
            // We DID find a suitable pos for this new circle! Add it!
            else {
                Circle newCircle = Instantiate(resourcesHandler.circleGrow_circle).GetComponent<Circle>();
                newCircle.Initialize(this, canvas.transform, randPos, startingRadius);
                circles.Add(newCircle);
                newCircle.SetIsOscillating(true); // Start it out oscillating up up!
            }
            // Update the score now! :)
            UpdateScore();
        }
        private void SolidifyOscillatingCircles() {
            // Make 'em stop oscillating!
            for (int i=circles.Count-1; i>=0; --i) {
                if (circles[i].IsOscillating) {
                    SolidifyCircle(circles[i]);
                }
            }
            // If we didn't lose, add a new circle!
            if (gameState != GameStates.GameOver) {
                AddNewCircle();
            }
        }
        private void SolidifyCircle(Circle circle) {
            circle.SetIsOscillating(false);
            //if (IsCir(circle)) {
            //    circle.OnIllegalOverlap();
            //    SetGameOver();
            //}
        }
        private void OnCircleIllegalOverlap(Circle circle) {
            circle.OnIllegalOverlap();
            SetGameOver();
            //SolidifyCircle(circle);
            //AddNewCircle();
        }






        // ----------------------------------------------------------------
        //  Game Flow
        // ----------------------------------------------------------------
        private void RestartLevel() {
            // Set basics!
            SetIsPaused(false);
            timeWhenLevelEnded = -1;
            gameState = GameStates.Playing;
            //Camera.main.backgroundColor = new Color(0.99f,0.99f,0.99f);

            DestroyCircles();

            AddNewCircle();

            // Tell the people!
            ui.OnStartLevel();
        }

        private void SetGameOver() {
            gameState = GameStates.GameOver;
            timeWhenLevelEnded = Time.time;
            // Tell people!
            //ui.OnGameOver();
        }

        private void UpdateScore() {
            float score = 0;
            foreach (Circle c in circles) {
                float area = Mathf.PI * c.Radius * c.Radius;
                score += area;
            }
            score /= 100f; // HARDcoded.
            // Update the UI!
            ui.SetScoreText(score);
        }

        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        override protected void Update () {
            base.Update();

            RegisterMouseInput();

            CheckOscillatingCirclesOverlaps();
        }
        private void CheckOscillatingCirclesOverlaps() {
            // Make 'em stop oscillating!
            for (int i=circles.Count-1; i>=0; --i) {
                if (circles[i].IsOscillating) {
                    if (IsCircleIllegalOverlap(circles[i])) {
                        OnCircleIllegalOverlap(circles[i]);
                    }
                }
            }
        }

        private void RegisterMouseInput() {
            if (Input.GetMouseButtonDown(0)) {
                OnMouseDown();
            }
        }



        // ----------------------------------------------------------------
        //  Input
        // ----------------------------------------------------------------
        private void OnMouseDown() {
            OnTapScreen();
        }
        public void OnRetryButtonClick() {
            RestartLevel();
        }
        private void OnTapScreen() {
            // Paused? Ignore input.
            if (Time.timeScale == 0f) { return; }

            if (gameState == GameStates.GameOver) {
                // Make us wait a short moment so we visually register what's happened.
                if (Time.time>timeWhenLevelEnded+2f) { // NOTE: Idk why 2 doesn't feel like actual 2 seconds. :P
                    RestartLevel();
                    return;
                }
            }
            else if (gameState == GameStates.PostLevel) {
                //// Make us wait a short moment so we visually register what's happened.
                //if (Time.time>timeWhenLevelEnded+0.2f) {
                //             StartNextLevel();
                //             return;
                //}
            }
            else {
                SolidifyOscillatingCircles();
            }
        }
        override protected void RegisterButtonInput() {
            base.RegisterButtonInput();

            if (Input.GetKeyDown(KeyCode.Space)) { OnTapScreen(); }
            if (Input.GetKeyDown(KeyCode.Return)) { RestartLevel(); }
        }







    }
}

