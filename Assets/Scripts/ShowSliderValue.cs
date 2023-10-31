//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;

namespace Hololens.Inspection.Utilities
{
    [AddComponentMenu("ARBI/Scripts/ShowSliderValue")]
    public class ShowSliderValue : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro textMesh = null;

        public void OnSliderUpdated(SliderEventData eventData)
        {
            if (textMesh == null)
            {
                textMesh = GetComponent<TextMeshPro>();
            }
            else
            {
                Debug.Log("Recognized that textMesh was not null.");
            }

            if (textMesh != null)
            {
                textMesh.text = $"{eventData.NewValue:F2}";
            }
            else
            {
                Debug.Log("textMesh is still null for some reason...");
            }
        }
    }
}
