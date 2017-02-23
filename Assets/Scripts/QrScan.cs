using System;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class QrScan : MonoBehaviour
{
    //public Transform textMeshObject;

    //Only used for demomode:
    bool demomode = true;
    GestureRecognizer recognizer;
    int whatid = 0;
    string[] id = new string[] { "qrint:blink", "qrint:sensor" };
    
    private void Start()
    {
        //this.textMesh = this.textMeshObject.GetComponent<TextMesh>();
        //this.OnReset();
        //Debug.Log("QR start");
        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        recognizer.TappedEvent += (source, tapCount, ray) =>
        {
            bogusFound();
        };
    }
    public void OnScan()
    {
        //this.textMesh.text = "scanning for 10s";
        Debug.Log("QR scanning");
        Backend.Instance.showNote("Scanning...");
        //show headsup.

        if (demomode) //demomode
        {
            recognizer.StartCapturingGestures();
        }
        else
        {

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
        } //end of demomode else
    }

    //used in demomode
    private void bogusFound()
    {
        recognizer.StopCapturingGestures();
        if (whatid >= id.Length)
        {
            whatid = 0;
        }
        onFound(id[whatid]);
        whatid++;
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
