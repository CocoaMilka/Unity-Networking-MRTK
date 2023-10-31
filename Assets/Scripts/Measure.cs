using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;
using UnityEngine.Events;


namespace Hololens.Inspection.Utilities
{
    /// <summary>
    /// This custom solver implements a visual measurement system of
    /// the object it is attached to.
    /// To Do:
    ///     Determine what goes into this solver from "findDistance.cs"
    ///     Determine use of this solver (on prefab, on inspection area)
    ///         goes on prefab
    ///         prefab offset target object when in focus
    ///         will turn off solver when target object loses focus
    ///         target object will be set from MeasureMode Handler
    ///     Copy over members
    ///     Implement abstract method SolverUpdate()
    ///     
    /// </summary>
    [AddComponentMenu("Scripts/HololensInspection/Utilities/Measure")]
    public class Measure : Microsoft.MixedReality.Toolkit.Utilities.Solvers.Solver
    {
        [SerializeField]
        [Tooltip("Prefab to spawn indicating measurement points.")]
        private GameObject measurePointPrefab = null;

        /// <summary>
        /// Prefab to spawn indicating measurement points.
        /// </summary>
        public GameObject MeasurePointPrefab
        {
            get => measurePointPrefab;
            set => measurePointPrefab = value;
        }

        [SerializeField]
        [Tooltip("Tooltip here")]
        private int maxPoints;

        /// <summary>
        /// Max number of points that can be mesaured in a session, greater than 0. 
        /// This limit may need to be experimented with to find a proper value.
        /// </summary>
        public int MaxPoints
        {
            get => maxPoints;
            set => maxPoints = value;
            /*
            {
                if (Debug.Assert((value > 0), $"Max Points value of {value} isn't greater than 0."))
                {
                    maxPoints = value;
                }
            }
            */
        }


        [SerializeField]
        [Tooltip("Tooltip here")]
        private float defaultOffsetDistance = 0.1f;

        /// <summary>
        /// This value indicates how far the measure points are offset the object this
        /// solver is attached to, normal to the surface.
        /// </summary>
        public float DefaultOffsetDistance
        {
            get => defaultOffsetDistance;
            set => defaultOffsetDistance = value;
        }


        public override void SolverUpdate()
        {
            if (SolverHandler != null && SolverHandler.TransformTarget != null)
            {
                Debug.Log("Solverupdate() called.");
            }
        }
    }
}
