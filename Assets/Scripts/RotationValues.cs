using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


/// <summary>
/// This file helps rotate the "Bridge Here" object. This object is the parent of the "10th bridge" object
/// The following codes move the calling object's box collider in the desired rotational angle along the y-axis
/// Note that the operations are accurate to 3 decimal points (0.001)
/// </summary>
public class RotationValues : MonoBehaviour
{
    #region Member Properties
    public TextMeshPro rotValues;
    public TextMeshPro systemKeyboardAngle;
    public float originalAngleY;
    public float originalAngleX;
    public float originalAngleZ;
    private bool validAngleInput;
    private bool onlyOneDotForFloatBool = true;
    private int onlyOneDotForFloat = 0;
    private char axis = 'n';

    private float inputAngle;
    private float axisValue;
    Vector3 rotationVector;
    #endregion

    #region Member Methods
    void Start()
    {
        originalAngleX = transform.rotation.eulerAngles.x;
        originalAngleY = transform.rotation.eulerAngles.y;
        originalAngleZ = transform.rotation.eulerAngles.z;
        validAngleInput = true;
    }

    public void ResetToOriginal()
    {
        if (axis == 'x')
        {
	    rotationVector = transform.rotation.eulerAngles;
	    rotationVector.x = originalAngleX;
	    transform.rotation = Quaternion.Euler(rotationVector);

	    axisValue = rotationVector.x;
	    rotValues.text = "Angle " + axis.ToString() + "-axis: " + axisValue.ToString("F3");
        }
        if (axis == 'y')
        {
	    rotationVector = transform.rotation.eulerAngles;
	    rotationVector.y = originalAngleY;
	    transform.rotation = Quaternion.Euler(rotationVector);

	    axisValue = rotationVector.y;
	    rotValues.text = "Angle " + axis.ToString() + "-axis: " + axisValue.ToString("F3");
        }
        if (axis == 'z')
        {
	    rotationVector = transform.rotation.eulerAngles;
	    rotationVector.z = originalAngleZ;
	    transform.rotation = Quaternion.Euler(rotationVector);

	    axisValue = rotationVector.z;
	    rotValues.text = "Angle " + axis.ToString() + "-axis: " + axisValue.ToString("F3");
        }
    }

    public void SetAsOriginal()
    {
        if (axis == 'x')
        {
            originalAngleX = transform.rotation.eulerAngles.x;
        }
        if (axis == 'y')
        {
            originalAngleY = transform.rotation.eulerAngles.y;
        }
        if (axis == 'z')
        {
            originalAngleZ = transform.rotation.eulerAngles.z;
        }
    }

    public void SetToZero()
    {
        if (axis == 'x')
        {
	    rotationVector = transform.rotation.eulerAngles;
	    rotationVector.x = 0;
	    transform.rotation = Quaternion.Euler(rotationVector);

	    axisValue = rotationVector.x;
	    rotValues.text = "Angle " + axis.ToString() + "-axis: " + axisValue.ToString("F3");
        }
        if (axis == 'y')
        {
	    rotationVector = transform.rotation.eulerAngles;
	    rotationVector.y = 0;
	    transform.rotation = Quaternion.Euler(rotationVector);

	    axisValue = rotationVector.y;
	    rotValues.text = "Angle " + axis.ToString() + "-axis: " + axisValue.ToString("F3");
        }
        if (axis == 'z')
        {
	    rotationVector = transform.rotation.eulerAngles;
	    rotationVector.z = 0;
	    transform.rotation = Quaternion.Euler(rotationVector);

	    axisValue = rotationVector.z;
	    rotValues.text = "Angle " + axis.ToString() + "-axis: " + axisValue.ToString("F3");
        }
    }

    public void SetToAngle()
    {
	String StringAngle = "";
        foreach (char c in systemKeyboardAngle.text)
        {
	    //Based on ASCII table
	    if (c >= 46 && c <= 57)
	    {
		StringAngle += c;
	    }
            if (c == '.')
            {
                onlyOneDotForFloat += 1;
            }

            if (onlyOneDotForFloat > 1)
            {
                onlyOneDotForFloatBool = false;
            }

            if ((!(char.IsDigit(c))) && (c != '.') && (!onlyOneDotForFloatBool) && (c != '-'))
            {
                validAngleInput = false;
            }
        }
        onlyOneDotForFloat = 0;

        if (validAngleInput)
        {
            if (axis == 'x')
            {
                inputAngle = string.IsNullOrEmpty(systemKeyboardAngle.text) ? 0 : float.Parse(StringAngle);

		rotationVector = transform.rotation.eulerAngles;
		rotationVector.x = inputAngle;
		transform.rotation = Quaternion.Euler(rotationVector);

		if(rotationVector.x >= 0)
		{
		    axisValue = rotationVector.x;
		}
		else
		{
		    axisValue = 180 + (180 - (-1 * rotationVector.x));
		}
		rotValues.text = "Angle " + axis.ToString() + "-axis: " + axisValue.ToString("F3");
            }
            if (axis == 'y')
            {
                inputAngle = string.IsNullOrEmpty(systemKeyboardAngle.text) ? 0 : float.Parse(StringAngle);

		rotationVector = transform.rotation.eulerAngles;
		rotationVector.y = inputAngle;
		transform.rotation = Quaternion.Euler(rotationVector);

		if(rotationVector.y >= 0)
		{
		    axisValue = rotationVector.y;
		}
		else
		{
		    axisValue = 180 + (180 - (-1 * rotationVector.y));
		}
		rotValues.text = "Angle " + axis.ToString() + "-axis: " + axisValue.ToString("F3");
            }
            if (axis == 'z')
            {
                inputAngle = string.IsNullOrEmpty(systemKeyboardAngle.text) ? 0 : float.Parse(StringAngle);

		rotationVector = transform.rotation.eulerAngles;
		rotationVector.z = inputAngle;
		transform.rotation = Quaternion.Euler(rotationVector);

		if(rotationVector.z >= 0)
		{
		    axisValue = rotationVector.z;
		}
		else
		{
		    axisValue = 180 + (180 - (-1 * rotationVector.z));
		}
		rotValues.text = "Angle " + axis.ToString() + "-axis: " + axisValue.ToString("F3");
            }

        }
        validAngleInput = true;
    }

    public void SetAxisX()
    {
        axis = 'x';
	rotationVector = transform.rotation.eulerAngles;
	axisValue = rotationVector.x;
	rotValues.text = "Angle " + axis.ToString() + "-axis: " + axisValue.ToString("F3");
    }

    public void SetAxisY()
    {
        axis = 'y';
	rotationVector = transform.rotation.eulerAngles;
	axisValue = rotationVector.y;
	rotValues.text = "Angle " + axis.ToString() + "-axis: " + axisValue.ToString("F3");
    }

    public void SetAxisZ()
    {
        axis = 'z';
	rotationVector = transform.rotation.eulerAngles;
	axisValue = rotationVector.z;
	rotValues.text = "Angle " + axis.ToString() + "-axis: " + axisValue.ToString("F3");
    }

    public void Minus5()
    {
	RotateSetAmount(-5);
    }

    public void Minus1()
    {
	RotateSetAmount(-1);
    }

    public void Plus1()
    {
	RotateSetAmount(1);
    }

    public void Plus5()
    {
	RotateSetAmount(5);
    }

    private void RotateSetAmount(int RotateAmount)
    {
	if (axis == 'x')
        {
	    rotationVector = transform.rotation.eulerAngles;
	    rotationVector.x = rotationVector.x + RotateAmount;
	    transform.rotation = Quaternion.Euler(rotationVector);

	    axisValue = rotationVector.x;
	    rotValues.text = "Angle " + axis.ToString() + "-axis: " + axisValue.ToString("F3");
        }
        if (axis == 'y')
        {
	    rotationVector = transform.rotation.eulerAngles;
	    rotationVector.y = rotationVector.y + RotateAmount;
	    transform.rotation = Quaternion.Euler(rotationVector);

	    axisValue = rotationVector.y;
	    rotValues.text = "Angle " + axis.ToString() + "-axis: " + axisValue.ToString("F3");
        }
        if (axis == 'z')
        {
	    rotationVector = transform.rotation.eulerAngles;
	    rotationVector.z = rotationVector.z + RotateAmount;
	    transform.rotation = Quaternion.Euler(rotationVector);

	    axisValue = rotationVector.z;
	    rotValues.text = "Angle " + axis.ToString() + "-axis: " + axisValue.ToString("F3");
        }
    }

    #endregion
}
