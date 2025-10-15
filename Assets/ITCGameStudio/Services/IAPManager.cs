using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using System.Collections.Generic;
using UnityEngine.Purchasing.MiniJSON;
//using AdjustSdk;
using UnityEngine.Purchasing.Security;
//using GPUInstancer;

[System.Serializable]
public class ProductInit
{
    public string Id;
    public ProductType Type;
    public ProductInit(string id, ProductType type)
    {
        Id = id;
        Type = type;
    }
}
public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
    public static IAPManager Instance;
    public IStoreController m_StoreController; 

    public IExtensionProvider m_StoreExtensionProvider;
    public List<ProductInit> ProductIds = new List<ProductInit>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        if (Instance == this) return;
        Destroy(gameObject);
    }

    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //Add products that will be purchasable and indicate its type.
        foreach (ProductInit product in ProductIds)
        {
            builder.AddProduct(product.Id, product.Type);
        }
        UnityPurchasing.Initialize(this, builder);
    }
    public void Buy(string ProductID)
    {
        m_StoreController.InitiatePurchase(ProductID);
    }
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("In-App Purchasing successfully initialized");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;

#if UNITY_ANDROID
        RestoringPurchases();
#elif UNITY_IOS
        
#endif

    }
    public string GetPriceString(string idProduct)
    {
        Product product = m_StoreController.products.WithID(idProduct);
        if (product != null)
        {
            return product.metadata.localizedPriceString;
        }
        return null;
    }
    public float GetPrice(string idProduct)
    {
        Product product = m_StoreController.products.WithID(idProduct);
        if (product != null)
        {
            return (float)product.metadata.localizedPrice;
        }
        return 0f;
    }
    public void RestoringPurchases()
    {
        if (!IsInitialized())
        {
            Debug.Log("Initialize fail, cannnot restore purchases");
            return;
        }
        var extensions = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
        extensions.RestoreTransactions((result, error) =>
        {
            if (result)
            {
                
            }
        });
    }
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        OnInitializeFailed(error, null);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

        if (message != null)
        {
            errorMessage += $" More details: {message}";
        }

        Debug.Log(errorMessage);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        var product = args.purchasedProduct;
        bool validPurchase = false;
#if UNITY_EDITOR
        validPurchase = true;
#endif
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
        /*if (!validPurchase)
        {
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
                AppleTangle.Data(), Application.identifier);
            try
            {
                validPurchase = true;
                var result = validator.Validate(args.purchasedProduct.receipt);
                Debug.Log("ITCGameStudioPurchase Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in result)
                {
                    Debug.Log("ITCGameStudioPurchase: " + productReceipt.productID);
                    Debug.Log("ITCGameStudioPurchase: " + productReceipt.purchaseDate);
                    Debug.Log("ITCGameStudioPurchase: " + productReceipt.transactionID);
                }
            }
            catch (IAPSecurityException ex)
            {
                Debug.Log("ITCGameStudioPurchase Invalid receipt, not unlocking content: " + ex.Message);
                //if (UIManager.instance != null)
                //{
                //    UIManager.instance.ShowProcessingPurchasePopup(false);
                //    UIManager.instance.ShowPurchaseFailPopup();
                //}
                return PurchaseProcessingResult.Complete;
            }
        }*/
#endif
        //var id = product.definition.id;
        //if (UIManager.Instance != null)
        //{
        //    UIManager.Instance.ShopManager.RewardProduct(product.definition.id, product.definition.type);
        //}

        var price = args.purchasedProduct.metadata.localizedPrice;
        double lPrice = decimal.ToDouble(price);
        var currencyCode = args.purchasedProduct.metadata.isoCurrencyCode;

    
        //We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");

    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
            $" Purchase failure reason: {failureDescription.reason}," +
            $" Purchase failure details: {failureDescription.message}");
    }

}