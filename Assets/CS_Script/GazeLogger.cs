using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class GazeLogger : MonoBehaviour
{
    public float maxDistance = 20f;
    public LayerMask gazeLayer;

    private bool isLogging = false;
    private GameObject currentTarget = null;
    private float logInterval = 0.01f;
    private float logTimer = 0f;

    private List<GazeEntry> gazeLog = new List<GazeEntry>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(GazeRoutine());
        }
        
        Debug.DrawRay(transform.position, transform.forward * maxDistance, Color.red);

        if (!isLogging) return;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, gazeLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject == currentTarget)
            {
                logTimer += Time.deltaTime;
                if (logTimer >= logInterval)
                {
                    gazeLog.Add(new GazeEntry
                    {
                        timestamp = System.DateTime.Now,
                        objectName = hitObject.name,
                        hitPosition = hit.point
                    });
                    logTimer = 0f;
                }
            }
            else
            {
                currentTarget = hitObject;
                logTimer = 0f;
            }
        }
        else
        {
            currentTarget = null;
            logTimer = 0f;
        }
    }

    private IEnumerator GazeRoutine()
    {
        VRPanelBlockerController.Instance?.HidePanel();

        isLogging = true;
        Debug.Log("🔴 Gaze Logging Started");

        yield return new WaitForSeconds(8f);

        isLogging = false;
        Debug.Log("⚪ Gaze Logging Stopped!");

        ExportLogToCSV();

        VRPanelBlockerController.Instance?.ShowPanel();
    }

    private void ExportLogToCSV()
    {
        string folderPath = Application.dataPath + "/GazeLogs";
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filePath = folderPath + $"/gaze_timeline_{timestamp}.csv";

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Timestamp,ObjectName,HitX,HitY,HitZ");

        foreach (var entry in gazeLog)
        {
            Vector3 p = entry.hitPosition;
            sb.AppendLine($"{entry.timestamp:HH:mm:ss.fff},{entry.objectName},{p.x:F3},{p.y:F3},{p.z:F3}");
        }

        File.WriteAllText(filePath, sb.ToString());
        Debug.Log($"✅ Exported!!: {filePath}");
    }

    private class GazeEntry
    {
        public System.DateTime timestamp;
        public string objectName;
        public Vector3 hitPosition;
    }
}
