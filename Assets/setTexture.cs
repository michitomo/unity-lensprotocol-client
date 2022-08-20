using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class setTexture : MonoBehaviour
{
    public string thumbnailUrl;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(setThumbnailTexture());
    }

    IEnumerator setThumbnailTexture()
    {
        while (string.IsNullOrWhiteSpace(thumbnailUrl))
        {
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("setThumbnailTexture" + thumbnailUrl);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(thumbnailUrl);
        www.SendWebRequest();
        while (!www.isDone)
        {
            yield return null;
        }
        Debug.Log(www.error + " " + www.downloadHandler.text + thumbnailUrl);
        Texture texture = DownloadHandlerTexture.GetContent(www);
        GetComponent<Renderer>().material.mainTexture = texture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
