using UnityEngine;

public class MeshModelController : MonoBehaviour
{
    private bool _isRotating = true;
    private Camera camera;

    public bool IsRotating
    {
        get => _isRotating;
        set => _isRotating = value;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(_isRotating) transform.eulerAngles = new Vector3(0,camera.transform.eulerAngles.y,0);
    }
    
}
