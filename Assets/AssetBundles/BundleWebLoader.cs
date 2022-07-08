using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BundleWebLoader : MonoBehaviour
{
    public string bundleUrl = "https://firebasestorage.googleapis.com/v0/b/fir-project-8768e.appspot.com/o/test?alt=media&token=7ea7aa91-74f4-47f5-9f68-d046770ba08e";
    public string assetName = "Cube";
    public Text message;

    // Start is called before the first frame update
    public void Start()
    {
        message.text = "Started";
    }
    public void Cube()
    {
        StartCoroutine(AssetBundleCube());
    }

    public void Scene()
    {
        StartCoroutine(AssetBundleScene());
    }
    public IEnumerator AssetBundleCube()
    {
        using (WWW web = new WWW(bundleUrl))
        {
            yield return web;
            AssetBundle remoteAssetBundle = web.assetBundle;
            message.text = "It has been called";
            if (remoteAssetBundle == null)
            {
                message.text = "Failed to download AssetBundle Cube!";
                Debug.LogError("Failed to download AssetBundle!");
                yield break;
            }
            Instantiate(remoteAssetBundle.LoadAsset(assetName));
            remoteAssetBundle.Unload(false);
            message.text = "It has been finished";
        }
    }

    public IEnumerator AssetBundleScene()
    {
        using (WWW web = new WWW(bundleUrl))
        {
            yield return web;
            AssetBundle remoteAssetBundle = web.assetBundle;
            message.text = "It has been called";
            if (remoteAssetBundle == null)
            {
                message.text = "Failed to download AssetBundle Scene!";
                Debug.LogError("Failed to download AssetBundle!");
                yield break;
            }
            string[] scenePath = remoteAssetBundle.GetAllScenePaths();
            SceneManager.LoadScene(scenePath[0]);
            message.text = "It has been finished";
        }
    }


}