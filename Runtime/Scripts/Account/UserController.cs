using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{

    public static bool IsConnected(string customAPIToken = "", string customAPITokenExpiresAt = "")
    {
        bool hasLastUpdate = !String.IsNullOrEmpty(CacheController.GetInstance().userConfig.last_update);

        string apiToken = String.IsNullOrEmpty(customAPIToken) ? CacheController.GetInstance().userConfig.api_token : customAPIToken;
        string apiTokenExpiresAt = String.IsNullOrEmpty(customAPITokenExpiresAt) ? CacheController.GetInstance().userConfig.api_token_expires_at : customAPITokenExpiresAt;

        bool hasValidToken = !String.IsNullOrEmpty(apiToken) && DateTime.Parse(apiTokenExpiresAt) > DateTime.Now;

        return hasLastUpdate && hasValidToken;
    }
}
