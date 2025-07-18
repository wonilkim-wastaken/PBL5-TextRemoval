using UnityEngine;
using System.IO;

public class ObjectTextureWatcher : MonoBehaviour
{
    public string textureFileName;
    public string folderPath = @"C:\Users\is0646ep\Desktop\vr_env\Assets\inpaint_out";

    private string fullPath;
    private Renderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<Renderer>();
        fullPath = Path.Combine(folderPath, textureFileName);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            ReloadTexture();
        }
    }

    void ReloadTexture()
    {
        if (!File.Exists(fullPath))
        {
            Debug.LogWarning($"[Reloader] No texture file: {fullPath}");
            return;
        }

        byte[] data = File.ReadAllBytes(fullPath);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(data);
        meshRenderer.material.mainTexture = tex;

        Debug.Log($"[Reloader] {name} New texture applied: {textureFileName}");
    }
}
