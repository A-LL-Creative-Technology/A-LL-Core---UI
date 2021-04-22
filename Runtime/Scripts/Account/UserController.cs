using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{

    public static bool IsConnected()
    {
        bool hasLastUpdate = !String.IsNullOrEmpty(CacheController.GetInstance().userConfig.last_update);

        bool hasValidToken = CacheController.GetInstance().userConfig.api_token != null && CacheController.GetInstance().userConfig.api_token != "" && DateTime.Parse(CacheController.GetInstance().userConfig.api_token_expires_at) > DateTime.Now;

        return hasLastUpdate && hasValidToken;
    }
}
