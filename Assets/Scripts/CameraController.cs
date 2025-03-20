using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    [SerializeField] bool trackingPlayer, manualMove;
    
    [SerializeField] Transform player, roomPoint;
    [SerializeField] GameObject camPos;

    List<GameObject> camPoint = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
