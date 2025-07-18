using UnityEngine;
using TMPro;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MultiStepTextRemoval : MonoBehaviour
{
    public string pythonPath; //Absolute path to your Python intepreter
    public string scriptPath = "PythonScripts/text_removal_mult.py";
    public string peakPath = "ocr_out/peak.txt";

    public TMP_Text statusText;
    private List<int> heightSteps = new List<int>();
    private int currentIndex = 0;
    private bool hasLoadedPeak = false;
    private bool isRunning = false;
    public string selectedSet;

    void Start()
    {
        scriptPath = Path.Combine(Application.dataPath, scriptPath);
        peakPath = Path.Combine(Application.dataPath, peakPath);

        if (statusText != null)
            statusText.text = "";
    }

    void LoadPeak()
    {
        if (File.Exists(peakPath))
        {
            string content = File.ReadAllText(peakPath).Trim();
            if (int.TryParse(content, out int peak))
            {
                peak = peak + 20;
                heightSteps.Clear();

                heightSteps.Add(1000);
                heightSteps.Add(peak);
                heightSteps.Add(0);

                UnityEngine.Debug.Log($"[Unity] Loaded peak = {peak}, generated steps: {string.Join(", ", heightSteps)}");
            }
            else
            {
                UnityEngine.Debug.LogWarning($"[Unity] Failed to parse peak.txt as integer. Content: '{content}'");
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning($"[Unity] peak.txt not found at: {peakPath}");
        }
    }

    void Update()
    {
        if (!hasLoadedPeak && File.Exists(peakPath))
        {
            LoadPeak();
            hasLoadedPeak = true;
        }

        if (Input.GetKeyDown(KeyCode.Slash) && !isRunning)
        {
            if (heightSteps.Count == 0)
            {
                UnityEngine.Debug.LogWarning("[Unity] Height list is empty.");
                return;
            }

            int height = heightSteps[currentIndex];
            RunPython(height);

            currentIndex = (currentIndex + 1) % heightSteps.Count;
        }

        if (statusText != null)
        {
            if (isRunning)
            {
                int level = currentIndex == 0 ? heightSteps.Count : currentIndex;
                statusText.text = $"({level} / {heightSteps.Count})";
            }
            else
            {
                statusText.text = "";
            }
        }

        void RunPython(int height)
        {
            StartCoroutine(RunPythonCoroutine(height));
        }

        IEnumerator RunPythonCoroutine(int height)
        {
            isRunning = true;
            VRPanelBlockerController.Instance?.ShowPanel();

            var psi = new ProcessStartInfo
            {
                FileName = pythonPath,
                Arguments = $"\"{scriptPath}\" --height {height} --set {selectedSet}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            var process = new System.Diagnostics.Process();
            process.StartInfo = psi;

            process.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    UnityEngine.Debug.Log($"[PYTHON OUT] {e.Data}");
            };
            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    UnityEngine.Debug.LogError($"[PYTHON ERR] {e.Data}");
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            while (!process.HasExited)
                yield return null;

            isRunning = false;
        }
    }
}
