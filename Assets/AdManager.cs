
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager : MonoBehaviour {
    public static AdManager instance;
    public string ironsourceKey = "82837f8d";
    private void Awake()
    {

        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        ////Sets this to not be destroyed when reloading scene
        //DontDestroyOnLoad(gameObject); Note: #brettadded Don't need this; AdManager GameObject is in PersistentHouse.
    }
    public void showBanner() {}
    public void showInterstitial() {}
}
/*QQQ Commented all out.
public class AdManager : MonoBehaviour {
    public static AdManager instance;
    public string ironsourceKey = "82837f8d";

    private void Awake()
    {

        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);


    }

    // Use this for initialization
    void Start () {
        IronSource.Agent.init(ironsourceKey);
        IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
        IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
        IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
        IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
        IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
        IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
        IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAdLeftApplicationEvent;
    }

    //show reward ad
    public void showRewardVideo()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
    }

    //show baner ad
    public void showBanner()
    {
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
        IronSource.Agent.displayBanner();
    }

    //hide banner ad
    public void hideBanner()
    {
        IronSource.Agent.hideBanner();
    }

    //show interstitial
    public void showInterstitial()
    {
        if (IronSource.Agent.isInterstitialReady())
        {
            IronSource.Agent.showInterstitial();
        } else
        {
            loadInterstitialAd();
        }
    }

    //load interstitial 1 level before need to show
    public void loadInterstitialAd()
    {
        IronSource.Agent.loadInterstitial();
    }

    void onRewardedVideoAvailabilityChangedEvent(bool available)
    {
        //Change the in-app 'Traffic Driver' state according to availability.
        bool rewardedVideoAvailability = available;
    }

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    void OnEnable()
    {
        IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
    }

    //Invoked once the banner has loaded
    void BannerAdLoadedEvent()
    {
    }
    //Invoked when the banner loading process has failed.
    //@param description - string - contains information about the failure.
    void BannerAdLoadFailedEvent(IronSourceError error)
    {
    }
    // Invoked when end user clicks on the banner ad
    void BannerAdClickedEvent()
    {
    }
    //Notifies the presentation of a full screen content following user click
    void BannerAdScreenPresentedEvent()
    {
    }
    //Notifies the presented screen has been dismissed
    void BannerAdScreenDismissedEvent()
    {
    }
    //Invoked when the user leaves the app
    void BannerAdLeftApplicationEvent()
    {
    }


    //Invoked when the initialization process has failed.
    //@param description - string - contains information about the failure.
    void InterstitialAdLoadFailedEvent(IronSourceError error)
    {
    }
    //Invoked right before the Interstitial screen is about to open.
    void InterstitialAdShowSucceededEvent()
    {
    }
    //Invoked when the ad fails to show.
    //@param description - string - contains information about the failure.
    void InterstitialAdShowFailedEvent(IronSourceError error)
    {
    }
    // Invoked when end user clicked on the interstitial ad
    void InterstitialAdClickedEvent()
    {
    }
    //Invoked when the interstitial ad closed and the user goes back to the application screen.
    void InterstitialAdClosedEvent()
    {
    }
    //Invoked when the Interstitial is Ready to shown after load function is called
    void InterstitialAdReadyEvent()
    {
    }
    //Invoked when the Interstitial Ad Unit has opened
    void InterstitialAdOpenedEvent()
    {
    }

    //Invoked when the RewardedVideo ad view has opened.
    //Your Activity will lose focus. Please avoid performing heavy 
    //tasks till the video ad will be closed.
    void RewardedVideoAdOpenedEvent()
    {
    }
    //Invoked when the RewardedVideo ad view is about to be closed.
    //Your activity will now regain its focus.
    void RewardedVideoAdClosedEvent()
    {
    }
    //Invoked when there is a change in the ad availability status.
    //@param - available - value will change to true when rewarded videos are available. 
    //You can then show the video by calling showRewardedVideo().
    //Value will change to false when no videos are available.
    void RewardedVideoAvailabilityChangedEvent(bool available)
    {
        //Change the in-app 'Traffic Driver' state according to availability.
        bool rewardedVideoAvailability = available;
    }
    //  Note: the events below are not available for all supported rewarded video 
    //   ad networks. Check which events are available per ad network you choose 
    //   to include in your build.
    //   We recommend only using events which register to ALL ad networks you 
    //   include in your build.
    //Invoked when the video ad starts playing.
    void RewardedVideoAdStartedEvent()
    {
    }
    //Invoked when the video ad finishes playing.
    void RewardedVideoAdEndedEvent()
    {
    }
    //Invoked when the user completed the video and should be rewarded. 
    //If using server-to-server callbacks you may ignore this events and wait for the callback from the  ironSource server.
    //
    //@param - placement - placement object which contains the reward data
    //
    void RewardedVideoAdRewardedEvent(IronSourcePlacement placement)
    {
    }
    //Invoked when the Rewarded Video failed to show
    //@param description - string - contains information about the failure.
    void RewardedVideoAdShowFailedEvent(IronSourceError error)
    {
    }

    // Update is called once per frame
    void Update () {
		
	}
}
*/