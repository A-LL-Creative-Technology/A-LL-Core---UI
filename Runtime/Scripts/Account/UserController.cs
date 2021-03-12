using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{

    public static bool IsConnected()
    {
        return (!String.IsNullOrEmpty(CacheController.GetInstance().userConfig.last_update) && CacheController.GetInstance().userConfig.token != null && CacheController.GetInstance().userConfig.token.access_token != "");
    }
}
