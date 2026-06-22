using UnityEngine;
using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{
    [SerializeField] SplineContainer _splineToFollow;

    public Vector3 UpdatePos(float percent)
    {
        transform.position = _splineToFollow.EvaluatePosition(percent);
        return transform.position;
    }
}
