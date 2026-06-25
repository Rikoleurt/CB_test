using UnityEngine;

namespace Object
{
    public class LevelBehaviour : MonoBehaviour
    {
        [SerializeField] PlayerPhysics playerPhysics;
        void Update()
        {
            if (playerPhysics.transform.position.y < -50)
            {
                playerPhysics.transform.position = new Vector3(1, 2, 14);
                print("t'es nul fdp");
            }
        }
    }
}