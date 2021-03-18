using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{

    public static bool IsConnected()
    {
        bool hasLastUpdate = !String.IsNullOrEmpty(CacheController.GetInstance().userConfig.last_update);

        bool hasValidToken = CacheController.GetInstance().userConfig.token != null && CacheController.GetInstance().userConfig.token.access_token != "" && DateTime.Parse(CacheController.GetInstance().userConfig.token.expiration_date) > DateTime.Now;

        return hasLastUpdate && hasValidToken;
    }
}
