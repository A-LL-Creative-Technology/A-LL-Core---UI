using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public class NotificationController : MonoBehaviour
{
    private static NotificationController instance;

    public static NotificationController GetInstance()
    {
        return instance;
    }

#if UNITY_ANDROID
    AndroidNotificationChannel channel;
#endif

    private void Awake()
    {
        instance = this;
    }

    private bool isCurrentlyPaused = false;
    private Firebase.FirebaseApp app; //Used by Firebase
    private List<String> topics = new List<string>();

    private void Start()
    {
        
        // for push notification
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                Firebase.Messaging.FirebaseMessaging.TokenReceived += OnPushNotificationTokenReceived;
                Firebase.Messaging.FirebaseMessaging.MessageReceived += OnPushNotificationReceived;

                // Set a flag here to indicate whether Firebase is ready to use by your app.

                //Subscribe to Guest Topic (Topic 0)
                Firebase.Messaging.FirebaseMessaging.SubscribeAsync("/topics/0");

                ///////////////////////////////////////////////////////////////////
                ///         /!\ STAGING RELEASE AND LIVE TESTS /!\              ///
                ///    Comment following lines before productoin release        ///
                ///////////////////////////////////////////////////////////////////

                //Firebase.Messaging.FirebaseMessaging.SubscribeAsync("/topics/staging");    //Comment for live release, Uncomment for staging release.
                //Debug.LogWarning("/!\\ PUSH NOTIFICATIONS FOR STAGING RELEASE ACTIVATED /!\\ \r\nDon't forget to unsubscribe to topic staging before production release.");
                //Firebase.Messaging.FirebaseMessaging.SubscribeAsync("/topics/local");      //Comment for live or staging release, Uncomment for local tests.
                //Debug.LogWarning("/!\\ PUSH NOTIFICATIONS FOR LOCAL TESTS ACTIVATED /!\\ \r\nDon't forget to unsubscribe to topic local before staging or production release.");

                ///////////////////////////////////////////////////////////////////
                ///         /!\ STAGING RELEASE AND LIVE TESTS /!\              ///
                ///    Comment preceding lines before production release        ///
                ///////////////////////////////////////////////////////////////////
            }
            else
            {
                GlobalController.LogMe(String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });       
    }

    public void Subscribe()
    {
        for (int i = 1; i <= CacheController.authCompanyConfig.privilege_level; i++)
        {
            string topic = "/topics/" + i;
            Firebase.Messaging.FirebaseMessaging.SubscribeAsync(topic);
            topics.Add(topic);
        }
    }

    public void Unsubscribe()
    {
        foreach(string topic in topics)
        {
            Firebase.Messaging.FirebaseMessaging.SubscribeAsync(topic);
        }
        topics.Clear();
    }


    private void OnApplicationPause(bool isPaused)
    {
        if (!isCurrentlyPaused && isPaused)
        {
            // we just entered background
            GlobalController.LogMe("Application just entered background");

#if UNITY_IOS
            // synchronize the badge number
            SyncBadgeCount();
#endif
        }

        if (isCurrentlyPaused && !isPaused)
        {
            // we just entered foreground
            GlobalController.LogMe("Application just entered foreground");
        }

        isCurrentlyPaused = isPaused;
    }

#if UNITY_IOS
    private void SyncBadgeCount()
    {
        // adjust badge number
        iOSNotificationCenter.ApplicationBadge = iOSNotificationCenter.GetDeliveredNotifications().Length;
    }
#endif

    public void OnPushNotificationTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        GlobalController.LogMe("Received the push notification registration token: " + token.Token);
    }

    public void OnPushNotificationReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs pushNotificationMessage)
    {

        Debug.Log("OnPushNotificationReceived");

        //if notification has been opened
        if (pushNotificationMessage.Message.NotificationOpened)
        {
            GlobalController.LogMe("Notifications Data : ");
            // we then extract the data from the notification
            if (pushNotificationMessage.Message.Data.Count > 0)
            {
                
            }
        }
    }

    private void PrepareRequest(CacheModel.NewsItem item, string id, string releaseDate, out string oldestItemId, out string oldestItemReleaseDate)
    {
        oldestItemReleaseDate = item.released_at;
        oldestItemId = item.id.ToString();
        if (DateTime.Parse(item.released_at).CompareTo(DateTime.Parse(releaseDate)) > 0)
        {
            oldestItemReleaseDate = releaseDate;
            oldestItemId = id;
        }
    }

    private void PrepareRequest(CacheModel.EventItem item, string id, string beginDate, out string oldestItemId, out string oldestItemBeginDate)
    {
        oldestItemBeginDate = item.begin_date;
        oldestItemId = item.id.ToString();

        if (DateTime.Parse(item.begin_date).CompareTo(DateTime.Parse(beginDate)) < 0)
        {
            oldestItemBeginDate = beginDate;
            oldestItemId = id;
        }
    }

    private void NotificationRouting(string parentView, string routingView)
    {
        NavigationController.GetInstance().OnDisplayOpen(parentView);
        NavigationController.GetInstance().OnStackOpen(routingView);
        GlobalController.LogMe("Routing to view: " + routingView);

    }

}