using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TriLibCore.SFB;

public class AssetLoader : MonoBehaviour
{
    public Button LoadAsset;
    public VideoHelper videoHelper;
    public ModelHelper modelHelper;

    void Start()
    {
        LoadAsset.onClick.AddListener(OpenAsset);

        string path = SystemConfig.Instance.GetData<string>("LastPath");
        Debug.Log("last path: " + path);
        if(!string.IsNullOrWhiteSpace(path)){
            LoadAssetAction(path);
        }
    }

    void LoadAssetAction(string filePath){
        Debug.Log($"Read File:{filePath}");

        System.IO.FileInfo fi = new System.IO.FileInfo(filePath);  
        
        Debug.Log("File Type " + fi.Extension);

        if(fi.Extension == ".webm" || fi.Extension == ".mp4"){
            SystemConfig.Instance.SaveData("LastPath", filePath);
            Debug.Log("save path: " + filePath);

            modelHelper.CloseModel();

            videoHelper.SetupVideo(filePath);
        }

        if(fi.Extension == ".fbx"){
            SystemConfig.Instance.SaveData("LastPath", filePath);

            videoHelper.CloseVideo();

            modelHelper.SetupModel(filePath);
        }
    }

    void OpenAsset(){
        var extensions = new [] {
                new ExtensionFilter("supported video", "webm", "mp4"),
                new ExtensionFilter("3d model", "fbx"),
            };
        var result = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

        if (result != null)
        {
            var hasFiles = result != null && result.Count > 0 && result[0].HasData;

            if(result.Count > 0){
                string filePath = result[0].Name;
                
                LoadAssetAction(filePath);
            }
        }
    }
}
