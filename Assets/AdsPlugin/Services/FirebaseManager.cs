using Firebase;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Firebase.RemoteConfig;
using System.Linq;
using UnityEngine.Android;

namespace ITCGameStudio
{
    public class FirebaseManager : MonoBehaviour
    {
        public static FirebaseManager Instance;
        private bool _isFetchDone = false;

        public bool isFetchDOne => _isFetchDone;

        [SerializeField]
        private List<string> _keyConfigs = new List<string>();
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
                Destroy(gameObject);
        }
        private void Start()
        {
            Init();
            //Firebase.Installations.FirebaseInstallations.DefaultInstance.GetTokenAsync(forceRefresh: true).ContinueWith(
            //  task =>
            //  {
            //      if (!(task.IsCanceled || task.IsFaulted) && task.IsCompleted)
            //      {
            //          UnityEngine.Debug.Log(System.String.Format("ITCGameStudio Installations token {0}", task.Result));
            //      }
            //  });
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            }
#endif
        }
        public void Init()
        {
            Debug.Log("Firebase start Init!");
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                    InitializeFirebase();
                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0} <==", dependencyStatus));
                }
            });
        }
        void InitializeFirebase()
        {
            //
            System.Collections.Generic.Dictionary<string, object> defaults =
                new System.Collections.Generic.Dictionary<string, object>();
            FetchFireBase();

            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

        public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        {
            Debug.Log("Firebase Message Token: " + token);
        }
        public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs message)
        {
            Debug.Log("Firebase Message Received: " + message.Message.From);
            Debug.Log("Firebase Message Received: " + message.Message.MessageId);
        }
        public void FetchFireBase()
        {
            FetchDataAsync();
        }
        public Task FetchDataAsync()
        {
            Debug.Log("Fetching data...");
            System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                TimeSpan.Zero);
            return fetchTask.ContinueWith(FetchComplete);
        }
        void FetchComplete(Task fetchTask)
        {
            if (fetchTask.IsCanceled)
            {
                Debug.Log("Fetch canceled.");
            }
            else if (fetchTask.IsFaulted)
            {
                Debug.Log("Fetch encountered an error");
            }
            else if (fetchTask.IsCompleted)
            {
                Debug.Log("==> Fetch completed successfully!");
            }

            var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
            switch (info.LastFetchStatus)
            {
                case Firebase.RemoteConfig.LastFetchStatus.Success:
                    Task task = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                    task.ContinueWith(_result =>
                    {
                        if (!_result.IsCompleted)
                            return;

                        _isFetchDone = true;

                        _keyConfigs = FirebaseRemoteConfig.DefaultInstance.AllValues.Keys.ToList();
                        Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0})",
                            info.FetchTime));
                    });

                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Failure:
                    switch (info.LastFetchFailureReason)
                    {
                        case Firebase.RemoteConfig.FetchFailureReason.Error:
                            Debug.Log("Fetch failed for unknown reason ");
                            break;
                        case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                            Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                            break;
                    }
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Pending:
                    Debug.Log("Latest Fetch call still pending.");
                    break;
            }
        }
        public async Task GetValueRemote(string key, Action<ConfigValue> waitOnDone)
        {

            double countTime = 0;
            while (!_isFetchDone && countTime < 360000f)
            {
                countTime += 1000;
                await Task.Delay(1000);
            }

            if (countTime >= 360000f)
            {
                Debug.LogError(string.Format("Fetch data {0} fail, becuz time out! Check your network please!<==", key));
                return;
            }

            if (!_keyConfigs.Contains(key))
            {
                Debug.LogError(string.Format("Remote dont have key {0} ! ", key));
                return;
            }

            var obj = FirebaseRemoteConfig.DefaultInstance.GetValue(key);
            waitOnDone?.Invoke(obj);
        }

        public async Task<ConfigValue> GetConfigValueRemote(string key)
        {

            double countTime = 0;
            while (!_isFetchDone && countTime < 360000f)
            {
                countTime += 1000;
                await Task.Delay(1000);
            }

            if (countTime >= 360000f)
            {
                Debug.LogError(string.Format("Fetch data {0} fail, becuz time out! Check your network please!", key));
                return new ConfigValue();
            }

            if (!_keyConfigs.Contains(key))
            {
                Debug.LogError(string.Format("Remote dont have key {0} !", key));
                return new ConfigValue();
            }

            return FirebaseRemoteConfig.DefaultInstance.GetValue(key);

        }

        public ConfigValue GetConfigValueRemoteAsync(string key)
        {
            return GetConfigValueRemote(key).Result;
        }

        public void GetValueRemoteAsync(string key, Action<ConfigValue> waitOnDone)
        {
            _ = GetValueRemote(key, waitOnDone);
        }
        public async Task LogEventWithParameter(string event_name, Hashtable hash)
        {
            //if (AdsCD.instance.Cheating)
            //    return;
            double countTime = 0;
            while (!_isFetchDone && countTime < 360000f)
            {
                countTime += 1000;
                await Task.Delay(1000);
            }

            if (countTime >= 360000f)
            {
                Debug.LogError(string.Format("Logevent {0} fail, becuz time out! Check your network please!<==", event_name));
                return;
            }

            Firebase.Analytics.Parameter[] parameter = new Firebase.Analytics.Parameter[hash.Count];
            //List<Firebase.Analytics.Parameter> parameters = new List<Firebase.Analytics.Parameter>();
            if (hash != null && hash.Count > 0)
            {
                int i = 0;
                foreach (DictionaryEntry item in hash)
                {
                    if (item.Equals((DictionaryEntry)default)) continue;
                    string key = this.Checker(item.Key.ToString());
                    string value = this.Checker(item.Value.ToString());

                    parameter[i] = (new Firebase.Analytics.Parameter(key, value));
                    Debug.Log(" LogEvent " + event_name.ToString() + "- Key = " + key + " -  Value =" + value + " <==");
                    i++;
                }

                Firebase.Analytics.FirebaseAnalytics.LogEvent(
                           event_name,
                           parameter);
            }
        }
        public void LogEventWithOneParam(string eventName)
        {
            _ = this.LogEventWithParameter(eventName, new Hashtable() { { "value", 1 } });
        }
        public string Checker(string str)
        {
            str = str.Replace(" ", "_");
            return Regex.Replace(str, "[^0-9A-Za-z_+-]", "");
        }
    }
}
