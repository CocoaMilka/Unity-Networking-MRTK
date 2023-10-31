using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.IO;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.UI.HandCoach;

namespace Hololens.Inspection.Utilities
{
    [AddComponentMenu("Scripts/HololensInspection/Utilities/MeasureMode")]
    public class MeasureMode : MonoBehaviour//, IMixedRealityFocusHandler
    {

        #region Member Properties

        private List<GameObject> points = new List<GameObject>();
        private bool IsActive, InFocus;  // indicates measure mode or not
        private List<float> distances = new List<float>();

        public int[] measureType; //Stores the type of measurement (1=standard using two points, 2=area of circle, 3=area of rectangle)

        private const string distancesFileName = "SavedDistances.txt";
        private string default_dist_text = "Distances:\n";  // default text displayed to DistanceDisplay

        // public GameObject InspectionArea;
        public int maxPoints = 14; // max number of points to measure with (points / 2 measurements)
        private int prevMaxPoints = -1; // temp var to restore maxPoints after singe measure
        public TextMeshPro DistanceDisplay = null; // TextMeshPro to display distance to
        public GameObject pointPreFab = null;  // prefab that will indicate points 1 and 2

        [SerializeField]
        [Tooltip("List of colors that will be used for the measurement function. Ensure there are at least maxPoints/2 colors available.")]
        private Color[] Color_List = new[] { new Color(0, 0, 1, 1), new Color(0, 1, 0, 1), new Color(1, 0, 0, 1), new Color(1, 1, 0, 1), new Color(1, 0, 1, 1), new Color(0, 1, 1, 1), new Color(0, 0, 0, 1) };
        [Tooltip("Array of elements containing point to point icons.")]
        public Image[] Regular_Color_Images;
        [Tooltip("Array of elements containing circular icons.")]
        public Image[] Circular_Color_Images;
        [Tooltip("Array of elements containing rectangular icons.")]
        public Image[] Rectangular_Color_Images;

        [Tooltip("Green circle identifier that shows up when the regular point to point mode is selected.")]
        public GameObject Regular_Measurement_Type_Identifier;
        [Tooltip("Green circle identifier that shows up when the circular mode is selected.")]
        public GameObject Circular_Measurement_Type_Identifier;
        [Tooltip("Green circle identifier that shows up when the rectangular mode is selected.")]
        public GameObject Rectangular_Measurement_Type_Identifier;

        [Tooltip("Image components for drawing circles")]
        public Image[] Circle_Gameobjects;
        [Tooltip("Image components for drawing rectangles")]
        public Image[] Rectangle_Gameobjects;


        #endregion


        #region Member Methods

        //File note: The measurement system works in pairs of two points. There are always two points required for each type of measurement



        // Start runs when once upon initialization of the instance (when activated)
        void Start()
        {
            measureType = new int[maxPoints / 2];
            for(int i = 0; i < (maxPoints / 2); i++)
            {
                measureType[i] = 1;
            }
            
            int j=0;
            foreach(Image Circles in Circle_Gameobjects)
            {
                Circles.rectTransform.sizeDelta = new Vector2(0f, 0f);
                Circles.color = Color_List[j];
                j++;
            }
            j=0;
            foreach(Image Rectangles in Rectangle_Gameobjects)
            {
                Rectangles.rectTransform.sizeDelta = new Vector2(0f, 0f);
                Rectangles.color = Color_List[j];
                j++;
            }
        }

    void Update()
    {
        if(IsActive)
        {
            UpdateVisuals();
        }
    }

        void OnEnable()
        {
            if (!IsActive)
            {
                Debug.Log("MeasureMode.OnEnable() was called.");

                // initialize properties
                DistanceDisplay.text = default_dist_text;
                points.Clear();
                distances.Clear();

                StartMeasure();
            }
        }

        void OnDisable()
        {
            if (IsActive)
            {
                Debug.Log("OnDisable() was called. Number points to destroy: " + points.Count.ToString());
                // remove all measure points from the scene
                foreach (GameObject point in points)
                {
                    destroyPoint(point);
                }

                // clear the points and distances lists
                points.Clear();
                distances.Clear();

                foreach (Image image in Regular_Color_Images)
                {
                    image.color = new Color(0, 0, 0, 0);
                }
                foreach (Image image in Circular_Color_Images)
                {
                    image.color = new Color(0, 0, 0, 0);
                }
                foreach (Image image in Rectangular_Color_Images)
                {
                    image.color = new Color(0, 0, 0, 0);
                }

                // reset the distance display text
                DistanceDisplay.text = default_dist_text;

                // set the mesaure mode to inactive
                IsActive = false;
            }
        }



        //Enables the measurement functionality
        public void StartMeasure()
        {
            if (!IsActive)
            {
                Remove_Shapes();
                IsActive = true;  // mesaure mode IsActive
                NewPoint(); // add new point for placement
                
                //Remove each of the images from the measurement window
                foreach (Image image in Regular_Color_Images)
                {
                    image.color = new Color(0, 0, 0, 0);
                }
                foreach (Image image in Circular_Color_Images)
                {
                    image.color = new Color(0, 0, 0, 0);
                }
                foreach (Image image in Rectangular_Color_Images)
                {
                    image.color = new Color(0, 0, 0, 0);
                }
                SetRegularMeasurement();

                Debug.Log("Number of points after NewPoint() returns: " + points.Count);
            }
        }

        // Ends the measure sequence. Destroys the points, sets distance display
        // to the default text, and sets IsActive to false.
        public void StopMeasure()
        {
            Remove_Shapes();

            // remove all measure points from the scene
            foreach (GameObject point in points)
            {
                destroyPoint(point);
            }

            // clear the points and distances lists
            points.Clear();
            distances.Clear();
            
            //Remove each of the images from the measurement window
            foreach (Image image in Regular_Color_Images)
            {
                image.color = new Color(0, 0, 0, 0);
            }
            foreach (Image image in Circular_Color_Images)
            {
                image.color = new Color(0, 0, 0, 0);
            }
            foreach (Image image in Rectangular_Color_Images)
            {
                image.color = new Color(0, 0, 0, 0);
            }

            // reset the distance display text
            DistanceDisplay.text = default_dist_text;

            // set the mesaure mode to inactive
            IsActive = false;
        }
    
        //Removes the shapes from view by making them infinitely small
        public void Remove_Shapes()
        {
            foreach(Image Circles in Circle_Gameobjects)
            {
                Circles.rectTransform.sizeDelta = new Vector2(0f, 0f);
            }
            foreach(Image Rectangles in Rectangle_Gameobjects)
            {
                Rectangles.rectTransform.sizeDelta = new Vector2(0f, 0f);
            }
        }


        // Adds new point at measuring pointer's location; this relies on
        // measuringPointer and pointPreFab being assigned. A new currentPoint
        // is instantiated.
        private void NewPoint()
        {
            if (IsActive)
            {
                Debug.Log("newPoint() called at " + System.DateTime.Now.ToString("h:mm:ss tt")); // log call
                if ((pointPreFab != null) && (pointPreFab.GetComponent<SurfaceMagnetism>() != null))
                {
                    // if points exist, spawn at most recent one. otherwise, spawn at some other location
                    if (points.Count > 0)
                    {
                        // spawn prefab at pointer last point location, assign gameObject's parent as parent
                        // TODO: change parent to public var of inspection area's main transform
                        points.Add(Instantiate(pointPreFab,
                                               points[points.Count - 1].transform.position,
                                               points[points.Count - 1].transform.rotation/*,
                                               transform.parent*/));
                        Debug.Log("New point added to the collection at last point position.");
                    }
                    else
                    {
                        // spawn prefab at gameObject location, root becomes parent
                        points.Add(Instantiate(pointPreFab));
                        Debug.Log("New point added to the collection at origin.");
                    }
                    points[points.Count - 1].transform.GetChild(0).gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color_List[(points.Count - 1) / 2]);

                    Debug.Log("Number of points now: " + points.Count);

                }
                else
                    Debug.Log("pointPreFab is either null or missing the SurfaceMagnetism component.");
            }
        }


        //Places a point at the desired location if there are less than maxPoints currently placed
        public void PlacePoint()
        {
            // log this call for debuggin purposes
            Debug.Log("PlacePoint() called");
            
            if (IsActive && (points.Count > 0))
            {
                // disable SurfaceMagnetism so point will stay put
                var SurfaceMagnetismComponent = points[points.Count - 1].GetComponent<SurfaceMagnetism>();

                if (SurfaceMagnetismComponent != null)
                {
                    SurfaceMagnetismComponent.enabled = false;
                }
                else
                {
                    Debug.Log("Point Prefab missing SurfaceMagnetism solver.");
                }

                // add new point if max points value not reached yet
                if (points.Count < maxPoints)
                {
                    NewPoint();
                }
                else
                {
                    Debug.Log($"Max point(s) of {points.Count} reached.");
                }

                if((points.Count - 1) % 2 == 0 || points.Count == maxPoints)
                {
                    // recalculate the measured distances and update the distance
                    // display accordingly
                    updateDistances();

                    if (distances.Count == 0)
                    {
                        DistanceDisplay.text = default_dist_text; // set to default
                    }
                    else
                    {
                        DistanceDisplay.text = "";
                        int index_val = 0;
                        foreach (float distance in distances)
                        {
                            if(measureType[index_val] != 1)
                            {
                                DistanceDisplay.text += distance.ToString("F3") + " m^2\n";
                            }
                            else
                            {
                                DistanceDisplay.text += distance.ToString("F3") + " m\n";
                            }
                            index_val++;
                        }
                    }
                }
            }
        }


        //Update the text and images displayed on the measurement window
        //Updates both the measurement type identifier and the numerical measurements
        public void updateDistances()
        {
            if (IsActive)
            {
                distances.Clear();  // clear the distance list, to be recalculated
                var num_pnts = points.Count; // poll the current num points

                //This section enables specific measurement type identifiers to show up on the measurement window
                //These include the two circles, single circle, and square images
                int index_value = 1;
                foreach (Image image in Regular_Color_Images)
                {
                    if (num_pnts > index_value && measureType[index_value / 2] == 1)
                    {
                        image.color = Color_List[index_value / 2];
                    }
                    else
                    {
                        image.color = new Color(0, 0, 0, 0);
                    }
                    index_value+=2;
                }
                index_value = 1;
                foreach (Image image in Circular_Color_Images)
                {
                    if (num_pnts > index_value && measureType[index_value / 2] == 2)
                    {
                        image.color = Color_List[index_value / 2];
                    }
                    else
                    {
                        image.color = new Color(0, 0, 0, 0);
                    }
                    index_value+=2;
                }
                index_value = 1;
                foreach (Image image in Rectangular_Color_Images)
                {
                    if (num_pnts > index_value && measureType[index_value / 2] == 3)
                    {
                        image.color = Color_List[index_value / 2];
                    }
                    else
                    {
                        image.color = new Color(0, 0, 0, 0);
                    }
                    index_value+=2;
                }

                //This section calculates the distance to be displayed onto the measurement window
                float distance = 0;
                if(num_pnts>=2)
                {
                    int j = 0;
                    for (int i = 1; i < num_pnts; i+=2)
                    {
                        if(measureType[j] == 1) //Regular
                        {
                            distance = Vector3.Distance(points[i].transform.position, points[i - 1].transform.position);
                        }
                        else if(measureType[j] == 2) //Circular
                        {
                            //Read the x size of the visual circle and calculate the area
                            float radius = Circle_Gameobjects[i/2].rectTransform.sizeDelta.x/2.0f;
                            distance = 3.14159265f * Mathf.Pow(radius, 2.0f); //Area of circle
                        }
                        else if(measureType[j] == 3) //Rectangular
                        {
                            //Read the x and y size of the visual rectangle and calculate the area
                            distance = Rectangle_Gameobjects[i/2].rectTransform.sizeDelta.x*Rectangle_Gameobjects[i/2].rectTransform.sizeDelta.y;
                        }
                        else
                        {
                            distance = 0;
                        }
                        distances.Add(distance);
                        j++;
                    }
                }
            }
        }

        //This function updates the measurement visual elements that are overlayed onto the bridge
        //These include the circle images and rectangular images used to visualize the measured area
        void UpdateVisuals()
        {
            if(points.Count%2==1)
            {
                return;
            }

            var num_pnts = points.Count; // poll the current num points
            int elementNumber = (num_pnts-2)/2;

            if(measureType[elementNumber] == 2) //Circular
            {
                //Calculate equation of plane and project the second point onto a plane generated by the first
                Vector3 pointPosition = points[num_pnts-2].transform.position;
                Quaternion pointRotation = points[num_pnts-2].transform.rotation;
                Vector3 point2Position = points[num_pnts-1].transform.position;
                Vector3 rotationVector = points[num_pnts-2].transform.rotation * Vector3.forward;
                float dotProduct = (pointPosition.x*rotationVector.x)+(pointPosition.y*rotationVector.y)+(pointPosition.z*rotationVector.z);
                float kValue = (dotProduct-(rotationVector.x*point2Position.x)-(rotationVector.y*point2Position.y)-(rotationVector.z*point2Position.z))/(Mathf.Pow(rotationVector.x, 2.0f)+Mathf.Pow(rotationVector.y, 2.0f)+Mathf.Pow(rotationVector.z, 2.0f));
                float xNewPos = point2Position.x+(kValue*rotationVector.x);
                float yNewPos = point2Position.y+(kValue*rotationVector.y);
                float zNewPos = point2Position.z+(kValue*rotationVector.z);
                Vector3 newPoint = new Vector3(xNewPos, yNewPos, zNewPos);

                //Move the position of the second point onto the plane generated by the first
                points[num_pnts-1].transform.position = newPoint;

                //Calculate the radius using the first point and adjusted second
                float radius = Vector3.Distance(pointPosition, newPoint);

                //Reset the second point to the original position on the plane so it makes sense to the user
                points[num_pnts-1].transform.position = point2Position;
                //Rotate the center point to be at the correct rotation again
                points[num_pnts-2].transform.rotation = pointRotation;

                //Draw the visual circle in the world space
                Circle_Gameobjects[elementNumber].rectTransform.sizeDelta = new Vector2(radius*2, radius*2);
                Circle_Gameobjects[elementNumber].rectTransform.position = points[num_pnts-2].transform.position;
                Circle_Gameobjects[elementNumber].rectTransform.rotation = points[num_pnts-2].transform.rotation;
            }
            else if(measureType[elementNumber] == 3)
            {
                //Calculate equation of plane and project the second point onto a plane generated by the first
                Vector3 pointPosition = points[num_pnts-2].transform.position;
                Quaternion pointRotation = points[num_pnts-2].transform.rotation;
                Vector3 point2Position = points[num_pnts-1].transform.position;
                Vector3 rotationVector = points[num_pnts-2].transform.rotation * Vector3.forward;
                float dotProduct = (pointPosition.x*rotationVector.x)+(pointPosition.y*rotationVector.y)+(pointPosition.z*rotationVector.z);
                float kValue = (dotProduct-(rotationVector.x*point2Position.x)-(rotationVector.y*point2Position.y)-(rotationVector.z*point2Position.z))/(Mathf.Pow(rotationVector.x, 2.0f)+Mathf.Pow(rotationVector.y, 2.0f)+Mathf.Pow(rotationVector.z, 2.0f));
                float xNewPos = point2Position.x+(kValue*rotationVector.x);
                float yNewPos = point2Position.y+(kValue*rotationVector.y);
                float zNewPos = point2Position.z+(kValue*rotationVector.z);
                Vector3 newPoint = new Vector3(xNewPos, yNewPos, zNewPos);

                //Move the position of the second point onto the plane generated by the first
                points[num_pnts-1].transform.position = newPoint;

                //Set the second point as a parent of the first so that we can rotate the second point about the first
                //Rotate the plane onto a flat axis making it easier to calculate the x and y directions
                points[num_pnts-1].transform.localScale = new Vector3(3,3,3);
                points[num_pnts-1].transform.SetParent(points[num_pnts-2].transform);
                //points[num_pnts-2].transform.rotation = Quaternion.identity;
                points[num_pnts - 2].transform.rotation = Quaternion.Euler(0, 0, pointRotation.eulerAngles.z);
                points[num_pnts-1].transform.SetParent(null);

                //Calculate the size of the square
                float XSize = Mathf.Abs(points[num_pnts-2].transform.position.x-points[num_pnts-1].transform.position.x)*2;
                float YSize = Mathf.Abs(points[num_pnts-2].transform.position.y-points[num_pnts-1].transform.position.y)*2;

                //Move the rectangular indicator to the correct location and size
                Rectangle_Gameobjects[elementNumber].rectTransform.sizeDelta = new Vector2(XSize, YSize);
                Rectangle_Gameobjects[elementNumber].rectTransform.position = points[num_pnts-2].transform.position;

                //Reset the second point to the original position on the plane so it makes sense to the user
                points[num_pnts-1].transform.position = newPoint;
                //Rotate the center point to be at the correct rotation again
                points[num_pnts-2].transform.rotation = pointRotation;

                //Rotate the visual rectangle to be facing the correct orientation
                Vector3 rotationValue = points[num_pnts-2].transform.eulerAngles;
                Rectangle_Gameobjects[elementNumber].rectTransform.rotation = Quaternion.Euler(rotationValue.x, rotationValue.y, 0f);
            }
        }


        // Uses UnityEngine.Object.Destroy() function to remove point from the game,
        // sets point to null.
        void destroyPoint(GameObject point)
        {
            Debug.Log("destroyPoint() called"); // log call
                                                // destroy point if it exists
            if (point != null)
            {
                Destroy(point); // built-in Unity function Destroy() destantiates the
                                // GameObject from the scene
                point = null; // point now should point to null? just making sure
            }

            if ((points.Count - 1) % 2 == 0 || points.Count == maxPoints)
            {
                // recalculate the measured distances and update the distance
                // display accordingly
                updateDistances();

                if (distances.Count == 0)
                {
                    DistanceDisplay.text = default_dist_text; // set to default
                }
                else
                {
                    DistanceDisplay.text = "";
                    foreach (float distance in distances)
                    {
                        DistanceDisplay.text += distance.ToString("F3") + " m\n";
                    }
                }
            }
        }


        public void saveDistances() //NOT WORKING PROPERLY ANYMORE
        {
            if (IsActive)
            {
                Debug.Log("saveDistances() called");

                // create variable to contain path to txt file where distances are stored
                string path = Path.Combine(Application.persistentDataPath, distancesFileName);

                // create and write description if file doesn't exist
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine("Created " + System.DateTime.Now.ToString("MM/dd/yyyy h:mm:ss"));
                        sw.WriteLine("This file contains distances measured using HoloLens.");
                    }
                }

                // append current distance
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Distances saved at " + System.DateTime.Now.ToString("MM/dd/yyyy h:mm:ss tt") + "\n");
                    foreach (float distance in distances)
                        sw.WriteLine("   " + distance + " m");
                    sw.WriteLine("\n"); // add two extra lines for padding
                }

                Debug.Log("Distance saved to " + path);
            }
            else
            {
                Debug.Log("Couldn't save measurement: start a measure session first.");
            }
        }

        //Modifies the measure type to point to point measurement mode
        //This is used to determine which visual element should be displayed and how the distance/area should be calculated
        public void SetRegularMeasurement()
        {
            for(int i = ((points.Count - 1) / 2); i < (maxPoints / 2); i++)
            {
                measureType[i] = 1;
            }

            Regular_Measurement_Type_Identifier.SetActive(true);
            Circular_Measurement_Type_Identifier.SetActive(false);
            Rectangular_Measurement_Type_Identifier.SetActive(false);
        }

        //Modifies the measure type to circular measurement mode
        //This is used to determine which visual element should be displayed and how the distance/area should be calculated
        public void SetCircleMeasurement()
        {
            for(int i = ((points.Count - 1) / 2); i < (maxPoints / 2); i++)
            {
                measureType[i] = 2;
            }

            Regular_Measurement_Type_Identifier.SetActive(false);
            Circular_Measurement_Type_Identifier.SetActive(true);
            Rectangular_Measurement_Type_Identifier.SetActive(false);
        }

        //Modifies the measure type to rectangular measurement mode
        //This is used to determine which visual element should be displayed and how the distance/area should be calculated
        public void SetRectangleMeasurement()
        {
            for(int i = ((points.Count - 1) / 2); i < (maxPoints / 2); i++)
            {
                measureType[i] = 3;
            }

            Regular_Measurement_Type_Identifier.SetActive(false);
            Circular_Measurement_Type_Identifier.SetActive(false);
            Rectangular_Measurement_Type_Identifier.SetActive(true);
        }

        // to be called after a single measure is confirmed - NOT WORKING
        public void RestoreToPrevMeasure()
        {
            if (prevMaxPoints != -1)
            {
                maxPoints = prevMaxPoints;
                prevMaxPoints = -1;
            }
            else
            {
                Debug.Log("A single measure is already taking place or wasn't closed right.");
            }
        }
        #endregion
       

        #region IMixedRealityFocusHandler Impelementation

        // having focus issues, temporarily disabling the actual logic. Left the focus
        // log indicators for testing still.
        /*
        /// <inheritdoc />
        public void OnFocusEnter(FocusEventData eventData)
        {
            Debug.Log("Focus gained!");
            InFocus = true;

            // pause point from updating
            if (points.Count > 0)
            {
                var SurfaceMagnetismComponent = points[points.Count - 1].GetComponent<SurfaceMagnetism>();
                if (SurfaceMagnetismComponent != null)
                {
                    SurfaceMagnetismComponent.enabled = false;
                }
            }
        }

        
        /// <inheritdoc />
        public void OnFocusExit(FocusEventData eventData)
        {
            Debug.Log("Focus lost.");
            InFocus = false;

            // resume point updating
            if (points.Count > 0)
            {
                var SurfaceMagnetismComponent = points[points.Count - 1].GetComponent<SurfaceMagnetism>();
                if (SurfaceMagnetismComponent != null)
                {
                    SurfaceMagnetismComponent.enabled = true;
                }
            }
        }
        */

        #endregion
    }
}

