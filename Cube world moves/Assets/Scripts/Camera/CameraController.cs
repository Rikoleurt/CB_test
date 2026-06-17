using System;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;


public class CameraController : MonoBehaviour
    {
        [SerializeField] Transform playerTransform;
        [SerializeField] private SplineContainer playerSpline;
        [SerializeField] private Transform pivotTransform;
        [SerializeField] Controller controller;
        [Range(0,1)] [SerializeField] private float sensitivityAngle;
        [Range(0,1)] [SerializeField] private float sensitivityHeight;
        [ShowNonSerializedField] float angle;
        [ShowNonSerializedField] private float height;
        
        void Start()
        {
            controller = Controller.Instance;
        }
        void LateUpdate()
        {
            Vector2 deltaLook = controller.DeltaLook;
            
            angle += sensitivityAngle * 100 * deltaLook.x;
            angle %= 360;
            
            pivotTransform.eulerAngles = new Vector3(0, angle, 0);
            
            height += sensitivityHeight * deltaLook.y;
            height = Mathf.Clamp01(height);
            
            Vector3 splinePos = playerSpline.EvaluatePosition(height);
            transform.position = splinePos;
            
            transform.LookAt(pivotTransform);
            
        }
    }
