using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LetterClear {
    public class GameController : BaseGameController {
        // Properties
        private int currentLevelIndex;
        // Components
        [SerializeField] private Image i_letterOverHighlight;
        [SerializeField] private Image i_letterSelectedHighlight;
        private List<WordTile> wordTiles;
        // References
        [SerializeField] private Canvas myCanvas;
        [SerializeField] private RectTransform rt_letterTiles=null;
        private LetterTile letterOver;
        private LetterTile letterSelected;

        // Getters (Private)
        private bool IsMatch(LetterTile letterA, LetterTile letterB) {
            if (letterA.IsVowel || letterB.IsVowel) { return true; } // TESTing mechanics!
            return letterA!=letterB && letterA.MyCharLower == letterB.MyCharLower;
        }
        private LetterTile GetLetterMouseOver() {
            Vector2 mousePos = Input.mousePosition;
            mousePos /= myCanvas.scaleFactor;
            mousePos -= new Vector2(0, rt_letterTiles.rect.height); // a little sloppy with this alignment business...
            mousePos += new Vector2(0,120); // blatant HACK for centering 'em.
            foreach (WordTile wordTile in wordTiles) {
                LetterTile tileHere = wordTile.GetAvailableLetterAtPoint(mousePos);
                if (tileHere != null) { return tileHere; }
            }
            return null;
        }



        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        override protected void Start () {
            base.Start();

            SetCurrentLevel(0);
        }


        private void InitializeLevel(string levelSentence) {
            DestroyWordTiles(); // Just in case

            // Initialize 'em!
            string[] words = levelSentence.Split(' ');
            wordTiles = new List<WordTile>();
            foreach (string word in words) {
                WordTile newObj = Instantiate(resourcesHandler.letterClear_wordTile).GetComponent<WordTile>();
                newObj.Initialize(this, rt_letterTiles, word);
                wordTiles.Add(newObj);
            }

            UpdateWordsPositions();
        }
        private void UpdateWordsPositions() {
            // Position 'em!
            Rect availableRect = new Rect();
            Vector2 padding = new Vector2(40, 40);
            availableRect.size = rt_letterTiles.rect.size - padding*2;
            availableRect.position = new Vector2(padding.x, -padding.y);
                
            float tempX = availableRect.xMin;
            float tempY = availableRect.yMin;
            const int fontSize = 100;
            float spaceSize = fontSize*0.4f;
            float lineHeight = fontSize;
            foreach (WordTile tile in wordTiles) {
                // Set the font size!
                tile.SetFontSize(fontSize);
                // Determine if this word should spill over to the next line.
                float tileWidth = tile.GetWidth();
                if (tempX+tileWidth>availableRect.xMax && tileWidth<availableRect.width) { // If this word spills over AND it's not exceedingly big (in which case, just let it be)?
                    tempX = availableRect.xMin;
                    tempY -= lineHeight;
                }
                tile.Pos = new Vector2(tempX, tempY);
                tempX += tileWidth + spaceSize;
            }
        }
        private void DestroyWordTiles() {
            if (wordTiles!=null) {
                foreach (WordTile obj in wordTiles) {
                    Destroy(obj.gameObject);
                }
            }
            wordTiles = null;
        }


        // ----------------------------------------------------------------
        //  Game Flow
        // ----------------------------------------------------------------
        private void RestartLevel() { SetCurrentLevel(currentLevelIndex); }
        private void StartPreviousLevel() { SetCurrentLevel(Mathf.Max(1, currentLevelIndex-1)); }
        private void StartNextLevel() { SetCurrentLevel(currentLevelIndex+1); }
        private void SetCurrentLevel(int _levelIndex) {
            currentLevelIndex = _levelIndex;

            SetIsPaused(false);

            // Set basics!
            i_letterOverHighlight.enabled = false;
            i_letterSelectedHighlight.enabled = false;

            // Tell the UI!
            //ui.UpdateLevelName(currentLevelIndex);

            // Initialize everything!
            string sentence = availableSentences[currentLevelIndex];
            InitializeLevel(sentence);//"start with a sentence. match letters.");// against each other to clear them.");
        }
        private void SetGameOver() {
            //gameState = GameStates.GameOver;
            //timeWhenLevelEnded = Time.time;
            //Camera.main.backgroundColor = new Color(0.6f,0.1f,0.1f);
        }

        private void OnCompleteLevel() {
            //gameState = GameStates.PostLevel;
            //timeWhenLevelEnded = Time.time;
            //Camera.main.backgroundColor = new Color(0.1f,0.8f,0.1f);
        }



        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        override protected void Update () {
            base.Update();

            UpdateLetterOver();
            RegisterMouseInput();
        }

        private void UpdateLetterOver() {
            LetterTile pletterOver = letterOver;
            letterOver = GetLetterMouseOver();

            // Put the highlight where it belongs, yo-yo.
            ShowHighlightOverLetter(i_letterOverHighlight, letterOver);

            // It's changed!?
            if (pletterOver != letterOver) {
                if (pletterOver != null) {
                    pletterOver.OnMouseOut();
                }
                if (letterOver != null) {
                    letterOver.OnMouseOver();
                }
            }
        }
        private void ShowHighlightOverLetter(Image highlight, LetterTile letter) {
            highlight.enabled = letter != null;
            if (letter != null) {
                highlight.rectTransform.sizeDelta = letter.MyRect.size;
                highlight.rectTransform.anchoredPosition = letter.MyRect.position;
                //highlight.rectTransform.anchoredPosition += new Vector2(0, rt_letterTiles.rect.height); // a little sloppy with this alignment business...
            }
        }
        private void MatchLetters(LetterTile letterA, LetterTile letterB) {
            letterA.OnMatched();
            letterB.OnMatched();
            UpdateWordsPositions();
            SetLetterSelected(null);
        }
        private void SetLetterSelected(LetterTile letter) {
            letterSelected = letter;
            ShowHighlightOverLetter(i_letterSelectedHighlight, letterSelected);
        }



        // ----------------------------------------------------------------
        //  Input
        // ----------------------------------------------------------------
        private void RegisterMouseInput() {
            if (Input.GetMouseButtonDown(0)) {
                OnMouseDown();
            }
        }
        private void OnMouseDown() {
            //if (gameState == GameStates.GameOver) {
            //    // Make us wait a short moment so we visually register what's happened.
            //    if (Time.time>timeWhenLevelEnded+0.2f) {
            //        RestartLevel();
            //        return;
            //    }
            //}
            //else if (gameState == GameStates.PostLevel) {
            //    StartNextLevel();
            //    return;
            //}
            //else {
            //}

            // We're over a letter?!
            if (letterOver != null) {
                // We DO have a letter selected AND it's a match!?
                if (letterSelected != null && IsMatch(letterOver, letterSelected)) {
                    MatchLetters(letterOver, letterSelected);
                }
                // We DON'T have a letter selected...
                else {
                    SetLetterSelected(letterOver);
                }
            }
            // We're NOT over a letter...
            else {
                SetLetterSelected(null);
            }
        }
        override protected void RegisterButtonInput() {
            base.RegisterButtonInput();
            if (Input.GetKeyDown(KeyCode.LeftBracket)) { StartPreviousLevel(); }
            if (Input.GetKeyDown(KeyCode.RightBracket)) { StartNextLevel(); }
            if (Input.GetKeyDown(KeyCode.W)) { Debug_WinLevel(); }
        }




        // ----------------------------------------------------------------
        //  Debug
        // ----------------------------------------------------------------
        private void Debug_WinLevel() {
            //foreach (PaintSpace space in paintSpaces) {
            //    space.OnPlayerBounceOnMe(Player.GetRandomHappyColor());
            //}
        }


        private string[] availableSentences = new string[]{
            "Hum drum",
            "I rent tents",
            "Eleven elves",
            "Groggy puppy",
            "No inhibition",
            "Catch the cat",
            "Twitter tweet",
            "Free the referee",
            "An assassin sins",
            "Beekeepers keep bees",

            // Bluth Ipsum!
            "Those are balls.",
            "Turn this skiff around!",
            "You're Killing Me, Buster.",
            "Heyyyy uncle father Oscar.",
            "Go ahead, touch the Cornballer.",
            "I'm sure Egg is a great person.",
            "If you don't start pulling your weight around here, it's going to be shape up... or ship up.",
            "I could use a leather jacket for when I'm on my hog and have to go into a controlled slide.",
            "Look at us, crying like a bunch of girls on the last day of camp.",
            "What, so the guy we are meeting with can't even grow his own hair? Come on!",
            "Look what the homosexuals have done to me! You can't just comb that out and reset it?",
            "No borders, no limits... Go ahead, touch the Cornballer... You know best?",
            "The Man Inside Me seems well reviewed.",
            "I run a pretty tight ship around here. With a pool table.",
            "It's, like, Hey, you want to go down to the whirlpool? Yeah, I don't have a husband.",
            "There are dozens of us! Dozens!",
            "You don't want a hungry dove down your pants.",
            "Can't a guy call his mother pretty without it seeming strange?",
            "And how about that little piece of tail on her? Cute!",
            "Stop licking my hand, you horse's ass!",
            "¡Soy loco por los Cornballs!",
            "You just made a fool out of yourself in front of T-Bone.",
            "You burned down the storage unit? Oh, most definitely.",
            "My brother wasn't optimistic it could be done, but I didn't take 'wasn't optimistic it could be done' for an answer.",
            "If you're suggesting I play favorites, you're wrong. I love all of my children equally. [earlier] I don't care for Gob.",
            "So did you see the new Poof? His name's Gary, and we don't need anymore lawsuits.",
            "Teamocil! One for the ladies. I call it Swing City. Say goodbye to THESE!",
            "I've been in the film business for a while but I just cant seem to get one in the can.",
            "I need a fake passport, preferably to France... I like the way they think.",
            "Oh, yeah, the guy in the the $4,000 suit is holding the elevator for a guy who doesn't make that in three months. Come on!",
            "Actually, that was a box of Oscar's legally obtained medical marijuana. Primo bud. Real sticky weed.",
            "You know, your average American male is in a perpetual state of adolescence, you know, arrested development. (Hey. That's the name of the show!)",
            "Let's make Ann the backup, okay? Very good way to think about her, as a backup.",
            "Everything they do is so dramatic and flamboyant. It just makes me want to set myself on fire.",
            "I'm gonna build me an airport, put my name on it. Why, Michael? So you can fly away from your feelings?",
        };

    }
}
