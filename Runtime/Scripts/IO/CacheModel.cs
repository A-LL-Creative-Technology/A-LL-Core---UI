using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CacheModel
{
    ///
    ///
    /// 
    /// CACHE STRUCTURES
    ///
    ///
    /// 
    public class BaseConfig
    {
        public string last_update = ""; // to be used to verify the validity of the cache and last update time

        public event EventHandler OnCacheConfigUpdated; // when we replaced the cache with updated content
        public event EventHandler OnCacheConfigExtended; // when we add new content to the existing cache (but don't update the rest)

        protected string filePath;

        protected enum CacheType
        {
            App,
            Permanent,
            Standard
        }

        protected CacheType cacheType = CacheType.Standard;

        public enum SaveType
        {
            NoEvent,
            Updated,
            Extended
        }

        public void Save(SaveType saveType = SaveType.Updated)
        {
            last_update = DateTime.Now.ToString("o");

            CacheController.SaveConfigToDisk(this);

            switch (saveType)
            {
                case SaveType.Updated:

                    // fires the event to notify who is interested in the update
                    if (OnCacheConfigUpdated != null)
                        OnCacheConfigUpdated(this, EventArgs.Empty);

                    break;

                case SaveType.Extended:

                    // fires the event to notify who is interested in the extension
                    if (OnCacheConfigExtended != null)
                        OnCacheConfigExtended(this, EventArgs.Empty);

                    break;
            }
        }

        public void Clear()
        {
            last_update = "";

            File.Delete(GetFullFilePath());

        }

        public string GetFullFilePath()
        {

            switch (cacheType)
            {
                case CacheType.App:

                    return Path.Combine(CacheController.appCacheLocation, filePath);

                case CacheType.Permanent:

                    return Path.Combine(CacheController.permanentSceneCacheLocation, filePath);

                default: // Standard

                    return Path.Combine(CacheController.standardConfigCacheLocation, filePath);

            }
        }
    }

    [Serializable]
    public class AppConfig : BaseConfig
    {
        // Serialized fields for Json
        public string lang = "";

        public AppConfig()
        {
            filePath = "AppConfig.json";
            cacheType = CacheType.App;
        }
    }

    [Serializable]
    public class UserConfig : BaseConfig
    {
        // Serialized fields for Json
        public string name;
        public string email;
        public string activity;
        public int privilege_level;
        public bool isOnboardingAlreadySeen = false;
        public string api_token = null;
        public string api_token_expires_at = null;

        public UserConfig()
        {
            filePath = "UserConfig.json";
            cacheType = CacheType.Permanent;
        }
    }



    /// 
    /// CACHE ITEM
    /// Classes that will be serialised into the cache
    ///
    [Serializable]
    public class BaseItem : IEquatable<BaseItem>
    {
        public int id;
        public string created_at;
        public string updated_at;



        // for Find, Sort, and other methods on a list
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            BaseItem objAsPart = obj as BaseItem;

            if (objAsPart == null)
                return false;
            else
                return Equals(objAsPart);
        }
        public bool Equals(BaseItem other)
        {
            if (other == null) return false;
            return (id.Equals(other.id));
        }


        public override int GetHashCode()
        {
            return id;
        }

    }


}
