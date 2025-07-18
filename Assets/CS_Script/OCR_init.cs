using UnityEngine;
using TMPro;
using System.Diagnostics;
using System.Collections;
using System.IO;

public class OCR_init : MonoBehaviour
{
    public string pythonPath; // Absolute path to your Python intepreter
    public string scriptPath = @"PythonScripts/text_detection.py";
    public TMP_Text statusText;
    private Process process;
    public string selectedSet;

    void Start()
    {
        scriptPath = Path.Combine(Application.dataPath, scriptPath);
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
