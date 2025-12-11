using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class UnityServiceManager : MonoBehaviour
{
    public string environment = "production";

    public bool unityServicesInitialized = false;

    private async void InitializeUnityServices()
    {
        try
        {
            var options = new InitializationOptions().SetEnvironmentName(environment);
            await UnityServices.InitializeAsync(options);
            unityServicesInitialized = true;
            IAPManager.Instance.InitializePurchasing();
        }
        catch (System.Exception exception)
        {
            Debug.Log("Failed to initialize Unity Gaming Services: " + exception.Message);
        }
    }
}
