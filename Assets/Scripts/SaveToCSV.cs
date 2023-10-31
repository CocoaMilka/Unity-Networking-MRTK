using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

namespace Hololens.Inspection.Utilities
{
    public class SaveToCSV : MonoBehaviour
    {
        [SerializeField]
        private int ConditionNumber = 0;
        public TextMeshProUGUI ConditionInputField,
                                LocationInputField,
                                SeverityInputField,
                                CommentInputField;
        public TextMeshPro DistanceDisplayField;
        public GameObject PhotoCaptureTool = null;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SaveLine()
        {
            // create variable to contain path to txt file where distances are stored
            string path = Path.Combine(Application.persistentDataPath, "Test.csv");

            string photoPath = PhotoCaptureTool.GetComponent<PhotoCaptureTool>().GetPhotoFilePath();
    
        // create and write description if file doesn't exist
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("Number, Condition, Location, Severity, Photo Filepath, Length, Comments");
                }
            }

            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine("{0},{1},{2},{3},{4},{5},{6}", ConditionNumber++,
                                                            ConditionInputField.text,
                                                            LocationInputField.text,
                                                            SeverityInputField.text,
                                                            photoPath,
                                                            DistanceDisplayField.text.Replace("\n", ""),
                                                            CommentInputField.text);
            }

            Debug.Log("The filepath generated is: " + path);
        }

    }
}
