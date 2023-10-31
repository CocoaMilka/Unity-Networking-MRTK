// definitely
using UnityEngine;  // basic Unity interfaces
using UnityEngine.Windows.WebCam;   // Windows/Unity webcam API
using System.Collections.Generic;   // provides generic data structures
using System.Linq;  // allows sorting of data
using Microsoft.MixedReality.Toolkit.UI;


namespace Hololens.Inspection.Utilities
{
    class PhotoCaptureTool : MonoBehaviour
    {
        #region Member Properties

        private PhotoCapture photoCaptureObject = null;

        // hologram opacity in photo. defaulted here to one (fully visible)
        //private float hologramOpacity = 1.0f;

        //[Range(0.0f, 1.0f)]
        public float HologramOpacity
        {
            get => hologramOpacity;
            set => hologramOpacity = value;
        }

        private float hologramOpacity = 1;

        // Some type of enum here for selecting. would need to update 
        // resolution references in OnPhotoCaptureCreated() and 
        // OnCapturedPhotoToMemory()
        
        // where to save file
        //public string folderPath;

        // hold photo while waiting for confirmation to save
        private PhotoCaptureFrame tempPhotoCaptureFrame = null;

        // whether image is previewed or not
        public bool PreviewEnabled = false;

        // main photo preview gameobject, should have photoDisplay somewhere beneath it
        public GameObject PhotoPreview = null;

        public GameObject MainButtons = null;

        // actual object with raw image component for displaying photo after capture
        public UnityEngine.UI.RawImage photoDisplay = null;

        // filePath variable so other scripts can access
        private string filePath = "";

        private Texture2D targetTexture = null;
        public Texture2D DefaultTexture = null;

        #endregion

        #region Member Methods

        // to be called when starting a capture session
        public void StartCapture()
        {
            PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
        }

        void OnPhotoCaptureCreated(PhotoCapture captureObject)
        {
            photoCaptureObject = captureObject;

            // select camera resolution (could add option to adjust, user select from menu)
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();


            // choose camera parameters, this should be adjusted by user before photo is taken
            CameraParameters c = new CameraParameters();
            c.hologramOpacity = hologramOpacity;
            c.cameraResolutionWidth = cameraResolution.width;
            c.cameraResolutionHeight = cameraResolution.height;
            c.pixelFormat = CapturePixelFormat.BGRA32;

            Debug.Log("Parameters: " + c.cameraResolutionWidth.ToString() + "x" + c.cameraResolutionHeight.ToString() + ", " + c.hologramOpacity + " opacity");

            photoCaptureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
        }

        void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
        {
            if (result.success)
            {
                Debug.Log("PhotoMode started successfully.");
                if (PreviewEnabled)
                {
                    // enable preview menu, hide main buttons
                    PhotoPreview.SetActive(true);
                    MainButtons.SetActive(false);

                    // take photo, OnCapturedPhotoToMemory handles display to preview
                    photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
                }
                else 
                {
                    // just take and save photo
                    
                    // filepath to save photo to 
                    string filename = string.Format(@"CapturedImage{0}_n.jpg", Time.time);
                    filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);

                    // take and save photo
                    photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
                }
            }
            else
            {
                Debug.LogError("Unable to start photo mode!");
            }
        }

        void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
        {
            if (result.success)
            {
                Debug.Log("Photo captured sucessfully, OnCapturedPhotoToMemory now running...");

                // Create our Texture2D for use and set the correct resolution
                Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
                targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

                // Copy the raw image data into our target texture
                photoCaptureFrame.UploadImageDataToTexture(targetTexture);

                // Apply texture to photoDisplay material
                photoDisplay.texture = targetTexture;
                //Renderer photoDisplayRenderer = photoDisplay.GetComponent<Renderer>();
                //photoDisplayRenderer.material.SetTexture("PhotoText", targetTexture);

                // save access to PhotoCaptureFrame so we can save it later
                tempPhotoCaptureFrame = photoCaptureFrame;
            }
            else
            {
                Debug.Log("OnCapturedPhotoToMemory did not run because PhotoCapture was not sucessful.");
            }

        }

        public void SavePhotoPreview()
        {
            if (photoCaptureObject != null)
            {
                // determine filePath
                string filename = string.Format(@"CapturedImage{0}_n.jpg", Time.time);
                filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);

                // save photo to location specified by filePath
                // List<byte> imageBufferList = new List<byte>();
                // tempPhotoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);
                // System.IO.File.WriteAllBytes(filePath, imageBufferList.ToArray());
                
                System.IO.File.WriteAllBytes(filePath, ImageConversion.EncodeToJPG(targetTexture));

                Debug.Log("Photo saved to " + filePath);

                // Clean up
                DiscardPhoto();  
            }
        }

        public void DiscardPhoto()
        {
            if (photoCaptureObject != null)
            {
                // stop photo mode and clean up
                photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);

                // if PhotoPreview object is active, deactivate it
                if (PhotoPreview.activeSelf)
                {
                    PhotoPreview.SetActive(false);
                }
            }
        }

        void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
        {

            // dispose of photo object
            photoCaptureObject.Dispose();
            photoCaptureObject = null;

            // reset textures (may be unecessary if preview not enabled, texture won't have changed)
            photoDisplay.texture = DefaultTexture;
            targetTexture = null;

        }

        void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
        {
            if (result.success)
            {
                Debug.Log("Saved Photo to disk at " + filePath);
                photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
            }
            else
            {
                Debug.Log("Failed to save Photo to disk");
            }
        }

        public void OnSliderValueUpdated(SliderEventData eventData)
        {
            hologramOpacity = eventData.NewValue;
        }

        public string GetPhotoFilePath()
        {
            return filePath;
        }

        public void TogglePreview()
        {
            PreviewEnabled = !PreviewEnabled;
        }

        #endregion
    }
}