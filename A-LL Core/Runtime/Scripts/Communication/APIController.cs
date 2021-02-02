﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Proyecto26; // Rest Client
using UnityEngine;
//using UnityEngine.Localization.Settings;
using UnityEngine.Networking;

public class APIController : MonoBehaviour
{

    //private static APIController instance;
    public static APIController instance;

    public static APIController GetInstance()
    {
        return instance;
    }

    // Parameters
    public static readonly string serverURL = "https://staging.api.ccif-hikf.ch";          //address of the API to call
    //public Token token;     //Datas related to OAuth token.

    private void Awake()
    {
        instance = this;
    }

    private static void HandleError(RequestException requestException, string endpoint, Action errorCallback = null)
    {
        GlobalController.LogMe("Error in the API call: " + endpoint + " - " + requestException.Message + " - " + requestException.Response);

        switch (requestException.StatusCode)
        {
            case 401: // bad credentials, the server is sending the error message key

                NavigationController.GetInstance().OnNotificationOpen(false, false, "Erreur de connexion", requestException.Response);

                break;

            case 422: // bad syntax in input values

                NavigationController.GetInstance().OnNotificationOpen(false, false, "Erreur de connexion", "CODE_Invalid_Email");

                break;

            default:

                NavigationController.GetInstance().OnNotificationOpen(false, true, "Erreur de connexion", "CODE_General_Connection_Error");

                break;
        }

        errorCallback?.Invoke();
    }

    //GET a SINGLE element of type R
    public static void Get<R>(string endpoint, Dictionary<string,string> parameters, Action<R> successCallback, Action errorCallback = null, string customServerURL = null)
    {
        //Send the request
        RestClient.Get<R>(BuildRequest(endpoint, parameters, customServerURL))
           .Then(res =>
           {
               successCallback?.Invoke(res);
           })
           .Catch(err =>
           {
               HandleError((RequestException)err, endpoint, errorCallback);
           });
    }

    



    //POST a request with no particular types
    public static void Post(string endpoint, Dictionary<string, string> parameters, Action<ResponseHelper> successCallback, Action errorCallback = null, string customServerURL = null)
    {
        RestClient.Post(BuildRequest(endpoint, parameters, customServerURL))
        .Then(res =>
        {
            successCallback?.Invoke(res);
        })
        .Catch(err =>
        {
            HandleError((RequestException)err, endpoint, errorCallback);

        });
    }

    //POST a request with an object of type S to the servers and expects an element of type R from the server
    public static void Post<R>(string endpoint, Dictionary<string, string> parameters, Action<R> callback, Action errorCallback = null, string customServerURL = null)
    {
        RestClient.Post<R>(BuildRequest(endpoint, parameters, customServerURL))
        .Then(res =>
        {
            callback?.Invoke(res);
        })
        .Catch(err =>
        {
            HandleError((RequestException)err, endpoint, errorCallback);
        });
    }

    //POST a file to the server
    public static void Post<S, R>(S requestObject, string endpoint, Dictionary<string, string> parameters, List<File> files, Action<R> callback, Action errorCallback = null, string customServerURL = null)
    {
        RestClient.Post<R>(BuildRequest(requestObject, endpoint, parameters, files, customServerURL))
        .Then(res =>
        {
            callback?.Invoke(res);
        })
        .Catch(err =>
        {
            HandleError((RequestException)err, endpoint, errorCallback);
        });
    }

    public static void GetImage(string imageUri, Action<Texture2D> callback, Action errorCallback = null)
    {

        RestClient.Get(new RequestHelper
        {
            Uri = imageUri, // url is insecure as Kentico staging is not in https (an exception for that domain has been added to InfoPlistUpdater.cs in the Editor folder of Unity)
            DownloadHandler = new DownloadHandlerTexture(),
        }).Then(res => {
            Texture2D texture = ((DownloadHandlerTexture)res.Request.downloadHandler).texture;
            callback?.Invoke(texture);
        }).Catch(err =>
        {
            RequestException requestException = (RequestException)err;

            // if no network, we do not display the error but simply call the errorCallback
            if (requestException.IsNetworkError)
            {
                errorCallback?.Invoke();
            }
            else
            {
                HandleError(requestException, imageUri, errorCallback);
            }

        });
    }

    //Build request header and body
    private static RequestHelper BuildRequest(string endpoint, Dictionary<string, string> parameters, string customeServerURL = null)
    {
        AddHeaders();

        // add localization to header
        if (CacheController.globalConfig.lang != "")
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }

            parameters.Add("lang", CacheController.globalConfig.lang);

        }

        string uri = (customeServerURL == null) ? serverURL : customeServerURL; 
        return new RequestHelper
        {
            Uri = SecureURL(uri) + "/" + endpoint,
            Params = parameters,
            EnableDebug = false,
        };

    }

    private static RequestHelper BuildRequest<B>(B body, string endpoint, Dictionary<string, string> parameters, List<File> files, string customServerURL = null)
    {
        RequestHelper request = BuildRequest(endpoint, parameters, customServerURL);
        request.FormSections = GenerateFormData(body, files);
        return request;
    }

    //Add Custom headers to the request
    private static void AddHeaders()
    {
        RestClient.DefaultRequestHeaders["X-Requested-With"] = "XMLHttpRequest";
        AddAuthorizationHeader();
    }

    //Add access_token and token_type to the Authorization header.
    private static void AddAuthorizationHeader()
    {
        if (CacheController.authCompanyConfig.token == null)
            return;
                
        RestClient.DefaultRequestHeaders["Authorization"] = "Bearer " + CacheController.authCompanyConfig.token.access_token;
    }

   

    //Generates a form data for file upload.
    private static List<IMultipartFormSection> GenerateFormData<B>(B body, List<File> files)
    {


        //Generate Dictionnary from body
        Dictionary<string, object> bodyDictionary = body.GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.Public)
            .ToDictionary(prop => prop.Name, prop => prop.GetValue(body));


        //Generate form data
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        //Add sections
        foreach (KeyValuePair<string, object> entry in bodyDictionary)
        {
            formData.Add(new MultipartFormDataSection(entry.Key, entry.Value.ToString()));
        }

        //Add files
        foreach (File file in files)
        {
            formData.Add(new MultipartFormFileSection(file.field, file.data, file.fileName, file.contentType));
        }

        return formData;
    }

    // convert HTTP to HTTPS (iOS will crash by default)
    private static string SecureURL(string url)
    {
        int indexOfSlash = url.IndexOf("/");

        if (url.Length == 0 || indexOfSlash == -1)
        {
            GlobalController.LogMe("Erreur parsing the URL for security: " + url);
        }

        url = "https:" + url.Substring(indexOfSlash);

        return url;
    }

    //File structure
    public class File
    {
        public string field;
        public byte[] data;
        public string fileName;
        public string contentType;
    }
}