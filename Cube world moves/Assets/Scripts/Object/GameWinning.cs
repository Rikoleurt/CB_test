using UnityEngine;
public class GameWinning : MonoBehaviour
{
    [SerializeField] PlayerPhysics playerPhysics;
    
    void Update()
    {
        if (playerPhysics.isWallDown && playerPhysics.WallDown.collider.CompareTag("End"))
        {
            print("You win fdp");
        }
    }
}
