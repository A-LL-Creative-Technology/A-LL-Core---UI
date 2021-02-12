using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{

    public static bool IsConnected()
    {
        return (!String.IsNullOrEmpty(CacheController.authCompanyConfig.last_update) && CacheController.authCompanyConfig.token != null && CacheController.authCompanyConfig.token.access_token != "");
    }
}
