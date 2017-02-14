using System;
using UnityEngine;

public class QrScan : MonoBehaviour
{
    //public Transform textMeshObject;
    
    private void Start()
    {
        //this.textMesh = this.textMeshObject.GetComponent<TextMesh>();
        //this.OnReset();
        //Debug.Log("QR start");
    }
    public void OnScan()
    {
        //this.textMesh.text = "scanning for 10s";
        Debug.Log("QR scanning");
        Backend.Instance.showNote("Scanning...");
        //show headsup.

#if !UNITY_EDITOR
    MediaFrameQrProcessing.Wrappers.ZXingQrCodeScanner.ScanFirstCameraForQrCode(
        result =>
        {
          UnityEngine.WSA.Application.InvokeOnAppThread(() =>
          {
            onFound(result?.Text);
          }, 
          false);
        },
        TimeSpan.FromSeconds(10));
#endif
    }
    public void OnReset()
    {
        //this.textMesh.text = "say scan to start";
        Debug.Log("QR reset");
    }
    //TextMesh textMesh;

    private void onFound(string result)
    {
        if (result != null)
        {
            string protocol = "qrint" + ":";
            if (result.StartsWith(protocol, StringComparison.OrdinalIgnoreCase))
            {
                string id = result.Substring(protocol.Length);
                Debug.Log("QR id found: " + id);
                Backend.Instance.createMenu(id);
            }
            else
            {
                Debug.Log("Unknown QR Code: " + result);
                //Display result?
                //Or error?
            }
        }
        else
        {
            Debug.Log("NO QR in result");
        }
    }
}
