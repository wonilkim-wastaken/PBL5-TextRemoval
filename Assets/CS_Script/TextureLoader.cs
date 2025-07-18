using UnityEngine;
using System.IO;

public class TextureLoader : MonoBehaviour
{
    public string baseFolderPath =  @"ImageTextures";

    public string selectedSet;

    void Start()
    {
        baseFolderPath = Path.Combine(Application.dataPath, baseFolderPath);
        string fullFolderPath = Path.Combine(baseFolderPath, selectedSet);

        ObjectTextureWatcher[] watchers = FindObjectsOfType<ObjectTextureWatcher>();

        foreach (var watcher in watchers)
        {
            string objectName = watcher.name;
            string textureFileName = objectName + ".png";
            string fullPath = Path.Combine(fullFolderPath, textureFileName);

            if (File.Exists(fullPath))
            {
                byte[] data = File.ReadAllBytes(fullPath);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(data);
                Renderer renderer = watcher.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.mainTexture = tex;
                    Debug.Log($"[TextureLoader] {objectName} Texture Applied!: {textureFileName}");
                }
            }
            else
            {
                Debug.LogWarning($"[TextureLoader] {objectName} No Texture!: {fullPath}");
            }
        }
    }
}
