using UnityEngine;
using System.Collections;

public class GameManagers {
	// Managers
    private DataManager dataManager;
    private EventManager eventManager;
    private SoundManager soundManager;
	// Getters
    public DataManager DataManager { get { return dataManager; } }
    public EventManager EventManager { get { return eventManager; } }
    public SoundManager SoundManager { get { return soundManager; } }
	// Properties
	private static bool isInitializing = false;

	public static bool IsInitializing { get { return isInitializing; } }


    /// Only for recompiling during runtime! I'm bonked after scripts recompile. Call this to hard-reset me.
    public static void Reinitialize() {
        isInitializing = false;
        instance = null;
    }


	// Constructor / Initialize
	private GameManagers () {
        dataManager = new DataManager ();
        eventManager = new EventManager ();
        soundManager = new SoundManager ();
	}



	// Instance
	static private GameManagers instance;
	static public GameManagers Instance {
		get {
			if (instance==null) {
				// We're ALREADY initializing?? Uh-oh. Return null, or we'll be caught in an infinite loop of recursion!
				if (isInitializing) {
					Debug.LogError ("GameManagers access loop infinite recursion error! It's trying to access itself before it's done being initialized.");
					return null; // So the program doesn't freeze.
				}
				else {
					isInitializing = true;
					instance = new GameManagers();
				}
			}
			else if (isInitializing) {
				isInitializing = false; // Don't HAVE to update this value at all, but it's nice to for accuracy.
			}
			return instance;
		}
	}

}
