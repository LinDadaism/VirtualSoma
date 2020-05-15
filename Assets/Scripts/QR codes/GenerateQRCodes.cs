using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;
//using Vuforia;
using ZXing;
using ZXing.QrCode;
using System.IO;

public class GenerateQRCodes: MonoBehaviour
{
    public int numQRCodes = 10;

    void Start()
    {
        //VuforiaARController.Instance.RegisterVuforiaStartedCallback(createImageTargetsFromQRGenerator);
        
        // generate and save QR codes as assets
        for (int i = 0; i < numQRCodes; i++)
        {       
            var encodingText = "puzzle_" + i;
            Texture2D myQR = generateQR(encodingText);
            byte[] bitmapQR = myQR.EncodeToJPG();
            var codeName = "QRCode_" + (i + 1) + ".jpg";
            File.WriteAllBytes("C:/Users/Linda/Senior Project/Virtual Soma/Assets/Textures/" + codeName, bitmapQR);   
        }
    }

    public Texture2D generateQR(string text)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = Encode(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
        return encoded;
    }

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

    // APPROACH BELOW NOT WORKING! WHY?
    // directly create an image target from the generated QR code 
    /*   void createImageTargetsFromQRGenerator()
       {
           var objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

           // generate the QR code
           var texture = generateQR("test");

           // get the runtime image source and set the texture
           var runtimeImageSource = objectTracker.RuntimeImageSource;
           runtimeImageSource.SetImage(texture, 0.15f, "myTargetName");

           // create a new dataset and use the source to create a new trackable
           var dataset = objectTracker.CreateDataSet();
           var trackableBehaviour = dataset.CreateTrackable(runtimeImageSource, "myTargetName");

           // add the DefaultTrackableEventHandler to the newly created game object
           trackableBehaviour.gameObject.AddComponent<DefaultTrackableEventHandler>();

           // activate the dataset
           objectTracker.ActivateDataSet(dataset);

           // TODO: add virtual content as child object(s)
       }*/
}
