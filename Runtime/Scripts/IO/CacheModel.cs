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

        public static event EventHandler OnCacheConfigUpdated; // when we replaced the cache with updated content
        public static event EventHandler OnCacheConfigExtended; // when we add new content to the existing cache (but don't update the rest)

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
    public class SceneConfig : BaseConfig
    {
        // Serialized fields for Json
        public bool isCompanyPasswordAlreadyGenerated = false;

        public SceneConfig()
        {
            filePath = "SceneConfig.json";
            cacheType = CacheType.Permanent;
        }
    }

    [Serializable]
    public class UserConfig : BaseConfig
    {
        // Serialized fields for Json
        public string name;
        public string activity;
        public int privilege_level;
        public bool isOnboardingAlreadySeen = false;
        public Token token = null;

        public UserConfig()
        {
            filePath = "UserConfig.json";
            cacheType = CacheType.Permanent;
        }
    }

    [Serializable]
    public class FullPageConfig<T> : BaseConfig
    {
        //Serialized fields for Json
        public T highlightedItem = default;
        public List<T> items = null;
    }

    [Serializable]
    public class SimplePageConfig<T> : BaseConfig
    {
        //Serialized fields for Json
        public List<T> items = null;
    }

    [Serializable]
    public class NewsConfig : FullPageConfig<NewsItem>
    {
      
        public NewsConfig()
        {
            filePath = "NewsConfig.json";
        }
    }

    [Serializable]
    public class InnovationsConfig : FullPageConfig<InnovationItem>
    {
        

        public InnovationsConfig()
        {
            filePath = "InnovationsConfig.json";
        }

    }

    [Serializable]
    public class EventsConfig : FullPageConfig<EventItem>
    {
        
        public EventsConfig()
        {
            filePath = "EventsConfig.json";
        }

    }

    [Serializable]
    public class FilteredEventsConfig : BaseConfig
    {
        //Serialized fields for Json
        public List<EventItem> items = new List<EventItem>();

        public FilteredEventsConfig()
        {
            filePath = "FilteredEventsConfig.json";
        }

    }

    [Serializable]
    public class MyEventsIDsConfig : BaseConfig
    {
        // Serialized fields for Json
        public List<int> myEventsIDs = new List<int>();

        public MyEventsIDsConfig()
        {
            filePath = "MyEventsIDsConfig.json";
            cacheType = CacheType.Permanent;
        }

    }

    [Serializable]
    public class TagsConfig : BaseConfig
    {

        // Serialized fields for Json
        public List<TagItem> tags = null;

        public TagsConfig()
        {
            filePath = "TagsConfig.json";
        }

    }

    [Serializable]
    public class SurveyRelationalsConfig : BaseConfig
    {

        // Serialized fields for Json
        public List<SurveyRelationalItem> surveyRelationals = new List<SurveyRelationalItem>();

        public SurveyRelationalsConfig()
        {
            filePath = "SurveyRelationalsConfig.json";
        }

    }

    [Serializable]
    public class ContactsConfig : BaseConfig
    {

        // Serialized fields for Json
        public List<ContactItem> contacts = null;

        public ContactsConfig()
        {
            filePath = "ContactsConfig.json";
        }

    }

    [Serializable]
    public class ProvidersConfig : BaseConfig
    {

        // Serialized fields for Json
        public List<ProviderItem> providers = null;

        public ProvidersConfig()
        {
            filePath = "ProvidersConfig.json";
        }

    }

    [Serializable]
    public class ServicesConfig : SimplePageConfig<ServiceItem>
    {
        // Serialized fields for Json
        public ServicesHeader header = null;

        public ServicesConfig()
        {
            filePath = "ServicesConfig.json";
        }

    }

    [Serializable]
    public class CreaTechsConfig : BaseConfig
    {
        // Serialized fields for Json
        public List<CreaTechItem> items = new List<CreaTechItem>();

        public CreaTechsConfig()
        {
            filePath = "CreaTechsConfig.json";
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

    [Serializable]
    public class SingleItem : BaseItem
    {
        public DocumentsItem[] documents;
        public SurveyItem survey;
    }

    [Serializable]
    public class NewsItem : SingleItem
    {
        public string released_at;

        public string title;
        public string summary;
        public string image_uri;
        public string image_uri_small;
        public int category;
        public int tag;
        public bool highlighted;
        public string content;
        
        public int [] contacts;
    }

    [Serializable]
    public class InnovationItem : NewsItem
    {
        // an innovation is a news with a specific category
    }

    [Serializable]
    public class EventItem : SingleItem, IComparable<EventItem>
    {
        
        public string title;
        public string begin_date;
        public string end_date;
        public int slots;
        public string address;
        public string location;
        public string locality;
        public int npa;
        public bool is_localization_set;
        public float latitude;
        public float longitude;
        public int provider_id = -1;
        public bool with_registration;
        public bool with_invitation;
        public string description;
        public string image_uri;
        public string url_prefix;
        public string url_suffix;

        public int CompareTo(EventItem other)
        {
            if (other == null)
                return 1;
            else
                return begin_date.CompareTo(other.begin_date);
        }


    }

    [Serializable]
    public class ServicesHeader
    {
        public string title;
        public string content;

        public string created_at;
        public string updated_at;
    }

    [Serializable]
    public class ServiceItem : BaseItem
    {
        public string title;
        public string uri;
    }

    [Serializable]
    public class TagItem : BaseItem
    {
        public string name;
    }

    [Serializable]
    public class ContactItem : BaseItem
    {
        public string name;
        public string email;
        public string phone_number;
        public string position;
    }

    [Serializable]
    public class ProviderItem : BaseItem
    {
        public string name;
        public string path;
        public string website;
    }

    [Serializable]
    public class DocumentsItem
    {
        public string title;
        public string name;
        public string uri;
    }

    [Serializable]
    public class SurveyItem: BaseItem
    {
        public string title;
    }

    [Serializable]
    public class SurveyRelationalItem
    {
        public int id;
        public int response; // -1 is no, +1 is yes

        public SurveyRelationalItem(int id, int response)
        {
            this.id = id;
            this.response = response;
        }
    }

    [Serializable]
    public class CreaTechItem : BaseItem
    {
        public string theme;
        public string title;
        public int client_id;
        public string notification_title;
        public string notification_text;
        public string begin_date;
        public string end_date;
        public string scene;
    }

    [Serializable]
    public class Token
    {
        public string access_token;
        public string expiration_date;
    }

    [Serializable]
    public class Company
    {
        public string name;
        public string activity;
        public int privilege_level;
    }

    /// 
    /// API
    /// Classes following API response structure
    /// 
    [Serializable]
    public class ItemsAPI
    {
        public int[] existing_ids;
    }

    [Serializable]
    public class NewsAPI
    {
        public List<NewsItem> news;
        public int[] existing_ids;
    }

    [Serializable]
    public class InnovationsAPI
    {
        public List<InnovationItem> innovations;
        public int[] existing_ids;
    }

    [Serializable]
    public class EventsAPI
    {
        public List<EventItem> events;
        public int[] existing_ids;
    }

    [Serializable]
    public class EventAPI
    {
        public EventItem eventItem;
    }

    [Serializable]
    public class TagsAPI
    {
        public List<TagItem> tags;
        public int[] existing_ids;
    }

    [Serializable]
    public class ContactsAPI
    {
        public List<ContactItem> contacts;
        public int[] existing_ids;
    }

    [Serializable]
    public class ProvidersAPI
    {
        public List<ProviderItem> providers;
        public int[] existing_ids;
    }

    [Serializable]
    public class ServicesAPI
    {
        public ServicesHeader header;

        public List<ServiceItem> services;
        public int[] existing_ids;
    }

    [Serializable]
    public class LoginAPI
    {
        public Token token;
        public Company auth_company;
    }

    [Serializable]
    public class CreaTechAPI
    {
        public List<CreaTechItem> crea_techs;
        public CreaTechItem notification;
    }

}
