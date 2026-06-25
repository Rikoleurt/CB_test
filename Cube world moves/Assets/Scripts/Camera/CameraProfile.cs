using UnityEngine;

[CreateAssetMenu]  
public class CameraProfile : ScriptableObject
{

    public bool _canPlayerControlCamera = true;
    public float FOV;
    public float _sensitivityAngle = 1f;
    public float _sensitivityHeight = 1f;
    public float _moveSmoothSpeed = 12f;
    public float _rotationSmoothSpeed = 15f;
    public float _collisionRadius = 0.25f;

    [Header("OFFSETS")]

    [Space(15)]
    public Vector3 CameraPosOffets = default;
    public Vector3 CameraLookOffets = default;

}

