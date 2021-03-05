using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{

    public static bool IsConnected()
    {
        return (!String.IsNullOrEmpty(CacheController.userConfig.last_update) && CacheController.userConfig.token != null && CacheController.userConfig.token.access_token != "");
    }
}
