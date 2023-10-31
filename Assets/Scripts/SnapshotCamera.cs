using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; // for folder creation

[RequireComponent(typeof(Camera))]

public class SnapshotCamera : MonoBehaviour
{

    Camera snapCam;
    int resWidth = 256;
    int resHeight = 256;
    // Start is called before the first frame update
    void Awake()
    {
        snapCam = GetComponent<Camera>();
        if (snapCam.targetTexture == null)
        {
            snapCam.targetTexture = new RenderTexture(resWidth, resHeight, 24);
        }
        else
        {
            resWidth = snapCam.targetTexture.width;
            resHeight = snapCam.targetTexture.height;
        }
        snapCam.gameObject.SetActive(false);
    }

 /*   // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CallTakeSnapshot();
        }
    }
 */
    public void CallTakeSnapshot()
    {
        snapCam.gameObject.SetActive(true);
    }

    private void LateUpdate()
    {
        if (snapCam.gameObject.activeInHierarchy)
        {
            Texture2D snapshot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            snapCam.Render();
            RenderTexture.active = snapCam.targetTexture;
            snapshot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            byte[] bytes = snapshot.EncodeToPNG();
            string fileName = SnapshotName();
            System.IO.File.WriteAllBytes(fileName, bytes);
            Debug.Log("Snapshot taken!" + fileName);    /// just simple output to check if picture is taken
            snapCam.gameObject.SetActive(false);
        }
    }

    string SnapshotName()
    {   // Option 1, needs to create Snapshots folder manually
      //  return string.Format(Application.persistentDataPath + "/Snapshots/snap_{0}x{1}_{2}.png",
        //    resWidth, resHeight, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));

        // Option 2
        /* return string.Format("{0}/Snapshots/snap_{1}x{2}_{3}.png", Application.dataPath,
             resWidth, resHeight, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));    
        // do Application.persistentDataPath if it needs to be stored somewhere else */

        // Option 3, creates directory, then adds pics
        string path = Application.persistentDataPath + "/Snapshots1";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return string.Format(Application.persistentDataPath + "/Snapshots1/snap_{0}x{1}_{2}.png",
            resWidth, resHeight, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));


    }
}
