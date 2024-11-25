using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class AssetBundleVideoLoader : MonoBehaviour
{
    private string assetBundleUrl = "http://joongly.com/assetbundles/test/video";

    [Header("Video Clip")] 
    public VideoClip videoClip;
    
    [Header("Video Player")]
    public VideoPlayer videoPlayer;
    public RawImage rawTexture;

    private string assetBundlePath;
    private string extractedVideoPath;


    private void Awake()
    {
        Caching.compressionEnabled = false;
        Caching.ClearCache();
    }

    void Start()
    {
        if (videoClip == null)
            StartCoroutine(DownloadAndPlayVideo());
        else
        {
            videoPlayer.clip = videoClip;
            StartCoroutine(PlayVideoPlayer());
        }
    }

    private IEnumerator PlayVideoPlayer()
    {

        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);
        videoPlayer.Play();
        
        rawTexture.texture = videoPlayer.texture;
    }

    IEnumerator DownloadAndPlayVideo()
    {
        Debug.Log("Start loading AssetBundle...");
        Debug.Log("assetBundleUrl "+assetBundleUrl);
        
        UnityWebRequest request = UnityWebRequest.Get(assetBundleUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("AssetBundle downloaded!");
            byte[] bundleData = request.downloadHandler.data;
            
            assetBundlePath = Path.Combine(Application.persistentDataPath, "downloadedAssetBundle");
            File.WriteAllBytes(assetBundlePath, bundleData);
            
            AssetBundle bundle = AssetBundle.LoadFromFile(assetBundlePath);
            if (bundle == null)
            {
                Debug.LogError("Can't load AssetBundle!");
                yield break;
            }
            
            string videoAssetName = bundle.GetAllAssetNames()[1];
            Debug.Log($"Video found in AssetBundle: {videoAssetName}");
            
            // string normalizedVideoName = Path.GetFileName(videoAssetName);
            
            VideoClip videoClip = bundle.LoadAsset<VideoClip>(videoAssetName);
            if (videoClip == null)
            {
                Debug.LogError("Can't load VideoClip from AssetBundle!");
                yield break;
            }
            
            // extractedVideoPath = Path.Combine(Application.persistentDataPath, normalizedVideoName);
            // File.WriteAllBytes(extractedVideoPath, File.ReadAllBytes(assetBundlePath));
            
            // videoPlayer.url = extractedVideoPath;
            videoPlayer.clip = videoClip;
            StartCoroutine(PlayVideoPlayer());

            Debug.Log("Video is playing!");
            
            bundle.Unload(false);
        }
        else
        {
            Debug.LogError($"Error loading AssetBundle: {request.error}");
        }
    }
}
