using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GraphQlClient.Core;
using Defective.JSON;
using WalletConnectSharp.Unity;

public class setMessageToSign : MonoBehaviour
{
    public GraphApi lensReference;
    public string messageToSign;
    // Start is called before the first frame update
    void Start()
    {
        getChallenge();
        setChallenge();
    }

    public async void getChallenge(){
        GraphApi.Query query = lensReference.GetQueryByName("getChallenge", GraphApi.Query.Type.Query);
        var signerAddress = WalletConnect.ActiveSession.Accounts[0];
        query.SetArgs(new{request = new{address = signerAddress}});
        UnityWebRequest request = await lensReference.Post(query);
        string data = request.downloadHandler.text;
        JSONObject json = new JSONObject(data);
        messageToSign = json.GetField("data").GetField("challenge").GetField("text").stringValue;
        MessageSigner signer = GameObject.Find("== Sign Message ==").GetComponent<MessageSigner>();
        signer.messageToSign = messageToSign;
        Debug.Log(data);
    }

    public async void setChallenge() {
        MessageSigner signer = GameObject.Find("== Sign Message ==").GetComponent<MessageSigner>();
        signer.messageToSign = messageToSign;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
