using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
//using UnityEngine.UI;
using TMPro;

public class UIInteractions : NetworkBehaviour
{
    [SerializeField] TMP_Dropdown dropdownXYZObj;
    [SerializeField] TMP_Dropdown dropdownPositionRotationObj;
    [SerializeField] TMP_Dropdown dropdownRobotDroneObj;
    private int rotateSpeed;
    private int translateSpeed;
    private int direction; // positive/negative direction
    private int dropdownOption;
    private GameObject ballObject;
    private Vector3 temp_TranslateRotateDirection;
    private Vector3 translateRotateDirection;
    private int positionRotationSelection;
    // Start is called before the first frame update
    void Start()
    {
        direction = 0;
        //dropdownOption = dropdownXYZObj.value;
        translateRotateDirection = new Vector3(0, 0, 0);
        temp_TranslateRotateDirection = new Vector3(0, 0, 0);
        translateRotateDirection = SwitchOptions(dropdownXYZObj.value);
        positionRotationSelection = dropdownPositionRotationObj.value;
        rotateSpeed = 100;
        translateSpeed = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Move the Robot and Drone
    public void OnClickMinusButton()
    {
        direction = -1;
        switch (dropdownRobotDroneObj.value)
        {
            case 0:
                MoveRobotPosRotServerRpc(translateRotateDirection, direction);
                break;
            case 1:
                MoveDronePosRotServerRpc(translateRotateDirection, direction);
                break;
            default:
                break;
        }        
    }

    public void OnClickPlusButton()
    {
        direction = 1;
        switch (dropdownRobotDroneObj.value)
        {
            case 0:
                MoveRobotPosRotServerRpc(translateRotateDirection, direction);
                break;
            case 1:
                MoveDronePosRotServerRpc(translateRotateDirection, direction);
                break;
            default:
                break;
        }
    }

    public void OnDropDownXYZSelectValueChange(TMP_Dropdown xyz)
    {
        translateRotateDirection = SwitchOptions(xyz.value);
    }

    public void OnDropDownPositionRotationValueChange(TMP_Dropdown xyz)
    {
        positionRotationSelection = xyz.value;
    }
    public void OnDropDownRobotDroneValueChange(TMP_Dropdown xyz)
    {
        //translateRotateDirection = SwitchOptions(xyz.value);
        //MoveRobotPosServerRpc(translateDirection, direction);
    }

    private Vector3 SwitchOptions(int options)
    {        
        switch (options)
        {
            case 0:
                temp_TranslateRotateDirection = new Vector3(1f, 0, 0);
                break;
            case 1:
                temp_TranslateRotateDirection = new Vector3(0, 1f, 0);
                break;
            case 2:
                temp_TranslateRotateDirection = new Vector3(0, 0, 1f);
                break;
            default:
                break;
        }
        return temp_TranslateRotateDirection;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void MoveRobotPosRotServerRpc(Vector3 directionVal, int addMinusSelection)
    {        
        //GameObject.FindGameObjectWithTag("Robot").transform.Translate(directionVal * addMinusSelection * Time.deltaTime);
        switch (dropdownPositionRotationObj.value)
        {
            case 0: //Position
                GameObject.FindGameObjectWithTag("Robot").transform.Translate(directionVal * addMinusSelection * translateSpeed* Time.deltaTime);
                break;
            case 1: //Rotation
                GameObject.FindGameObjectWithTag("Robot").transform.Rotate(directionVal * addMinusSelection * rotateSpeed* Time.deltaTime);
                break;
            default:
                break;
        }           
    }


    [ServerRpc(RequireOwnership = false)]
    private void MoveDronePosRotServerRpc(Vector3 directionVal, int addMinusSelection)
    {
        //GameObject.FindGameObjectWithTag("Robot").transform.Translate(directionVal * addMinusSelection * Time.deltaTime);
        switch (dropdownPositionRotationObj.value)
        {
            case 0: //Position
                GameObject.FindGameObjectWithTag("Drone").transform.Translate(directionVal * addMinusSelection *translateSpeed * Time.deltaTime);
                break;
            case 1: //Rotation
                GameObject.FindGameObjectWithTag("Drone").transform.Rotate(directionVal * addMinusSelection *rotateSpeed * Time.deltaTime);
                break;
            default:
                break;
        }
    }

    /*
    [ServerRpc(RequireOwnership = false)]
    private void MoveRobotPosServerRpc(Vector3 directionVal, int addMinusSelection)
    {
        //GameObject.FindGameObjectWithTag("Robot").transform.Translate(directionVal * addMinusSelection * Time.deltaTime);
        GameObject.FindGameObjectWithTag("Robot").transform.Rotate(directionVal * addMinusSelection * rotateSpeed* Time.deltaTime);
        //MoveRobotPosServerRpc(translateDirection, direction);
    }
    //*/




}
