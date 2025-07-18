/*
using UnityEngine;
using TMPro;
using System.Diagnostics;

public class OCR_init : MonoBehaviour
{
    public string pythonPath = @"C:\Users\is0646ep\AppData\Local\anaconda3\envs\textremoval\python.exe";
    public string scriptPath = @"C:\Users\is0646ep\Desktop\vr_env\Assets\PythonScripts\text_detection.py";
    public TMP_Text statusText;  // ← UI TextMeshPro 연결용

    void Start()
    {
        if (statusText != null)
            statusText.text = "Initializing...";

        RunOCRScript();
    }

    public void RunOCRScript()
    {
        var psi = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"\"{scriptPath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        var process = new Process();
        process.StartInfo = psi;

        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                UnityEngine.Debug.Log($"[PYTHON OUT] {e.Data}");
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                UnityEngine.Debug.LogError($"[PYTHON ERR] {e.Data}");
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        UnityEngine.Debug.Log("[Unity] OCR Python script launched internally.");
    }
}
*/

using UnityEngine;
using TMPro;
using System.Diagnostics;
using System.Collections;

public class OCR_init : MonoBehaviour
{
    public string pythonPath = @"C:\Users\is0646ep\AppData\Local\anaconda3\envs\textremoval\python.exe";
    public string scriptPath = @"C:\Users\is0646ep\Desktop\vr_env\Assets\PythonScripts\text_detection.py";
    public TMP_Text statusText;
    private Process process;
    public string selectedSet;

    void Start()
    {
        StartCoroutine(RunOCRScriptCoroutine());
    }

    IEnumerator RunOCRScriptCoroutine()
    {
        VRPanelBlockerController.Instance?.ShowPanel();
        statusText.text = "Initializing...";

        var psi = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"\"{scriptPath}\" --set {selectedSet}",  
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        process = new Process();
        process.StartInfo = psi;
        process.EnableRaisingEvents = true;

        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                UnityEngine.Debug.Log($"[PYTHON OUT] {e.Data}");
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                UnityEngine.Debug.LogError($"[PYTHON ERR] {e.Data}");
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        UnityEngine.Debug.Log("[Unity] OCR Python script launched internally.");

        while (!process.HasExited)
            yield return null;

        if (statusText != null)
        {
            if (process.ExitCode == 0)
                statusText.text = "Initialization Completed!";
            else
                statusText.text = "Failed!";
        }

        StartCoroutine(ClearStatusTextAfterDelay(3f));
    }
    
    IEnumerator ClearStatusTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (statusText != null)
            statusText.text = "";
    }
}
