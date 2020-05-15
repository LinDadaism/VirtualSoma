using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadQRcodes: MonoBehaviour
{

    private IReader QRReader;
    public Text resultText;
    public RawImage image;


    void Awake()
    {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
    }

    // Use this for initialization
    private void Start()
    {
        QRReader = new QRCodeReader();
        QRReader.Camera.Play();

        QRReader.OnReady += StartReadingQR;

        QRReader.StatusChanged += QRReader_StatusChanged;
    }

    private void QRReader_StatusChanged(object sender, System.EventArgs e)
    {
        resultText.text = "Status: " + QRReader.Status;
    }

    private void StartReadingQR(object sender, System.EventArgs e)
    {
        image.transform.localEulerAngles = QRReader.Camera.GetEulerAngles();
        image.transform.localScale = QRReader.Camera.GetScale();
        image.texture = QRReader.Camera.Texture;

        RectTransform rectTransform = image.GetComponent<RectTransform>();
        float height = rectTransform.sizeDelta.x * (QRReader.Camera.Height / QRReader.Camera.Width);
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
    }

    // Update is called once per frame
    void Update()
    {

        if (QRReader == null)
        {
            return;
        }

        QRReader.Update();
    }

    public void StartScanning()
    {
        if (QRReader == null)
        {
            Debug.LogWarning("No valid camera - Click Start");
            return;
        }

        // Start Scanning
        QRReader.Scan((barCodeType, barCodeValue) => {
            QRReader.Stop();
            resultText.text = "Found: [" + barCodeType + "] " + "<b>" + barCodeValue + "</b>";

#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        });
    }
}

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ZXing;
using ZXing.QrCode;

public class ReadQRcodes : MonoBehaviour
{
    private WebCamTexture camTexture;
    private Rect screenRect;

    void Start()
    {
        screenRect = new Rect(0, 0, Screen.width, Screen.height);
        camTexture = new WebCamTexture();
        camTexture.requestedHeight = Screen.height;
        camTexture.requestedWidth = Screen.width;
        if (camTexture != null)
        {
            camTexture.Play();
        }

        // display the QR code as a button
        Texture2D myQR = generateQR("test");
        if (GUI.Button(new Rect(300, 300, 256, 256), myQR, GUIStyle.none)) { }
    }

    void OnGUI()
    {
        // drawing the camera on screen
        GUI.DrawTexture(screenRect, camTexture, ScaleMode.ScaleToFit);

        // create a reader
        IBarcodeReader barcodeReader = new BarcodeReader();
        
        // do the reading — you might want to attempt to read less often
        // than you draw on the screen for performance sake
        try
        {
            // decode the current frame
            var result = barcodeReader.Decode(camTexture.GetPixels32(),
              camTexture.width, camTexture.height);
            if (result != null)
            {
                Debug.Log("DECODED TEXT FROM QR: " + result.Text);
            }
        }
        catch {  }
    }

    // rendering QR codes from text
    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }

    // generate a 2D texture and display ii in the GUI
    public Texture2D generateQR(string text)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = Encode(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
        return encoded;
    }
}
*/
