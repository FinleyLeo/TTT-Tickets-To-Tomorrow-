using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    int roomSize;

    [SerializeField] bool trackingPlayer, manualMove;
    [SerializeField] CinemachineCamera cam;
    [SerializeField] Transform player, roomPoint;
    [SerializeField] GameObject camPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        FollowLogic();
    }

    void FollowLogic()
    {
        if (!manualMove)
        {
            if (roomSize > 2)
            {
                trackingPlayer = true;
            }

            else
            {
                trackingPlayer = false;
            }
        }

        if (trackingPlayer)
        {
            cam.Target.TrackingTarget = player;
        }

        else
        {
            cam.Target.TrackingTarget = roomPoint.transform;
        }
    }
}
