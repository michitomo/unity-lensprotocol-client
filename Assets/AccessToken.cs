using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GraphQlClient.Core;
using Defective.JSON;
using WalletConnectSharp.Unity;

public class AccessToken : MonoBehaviour
{
    public GraphApi lensReference;
    public string accessToken;
    // Start is called before the first frame update
    void Start()
    {
        getAccessToken();

    }

    public async void getAccessToken()
    {
        GraphApi.Query query = lensReference.GetQueryByName("authenticate", GraphApi.Query.Type.Query);
        var signerAddress = WalletConnect.ActiveSession.Accounts[0];
        var signature = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
