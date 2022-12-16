using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float delayDuration = 0.15f;
    [SerializeField] private float respawnDelay = 4f;
    [SerializeField] private float clampValue = 3f;
    [SerializeField] private LineRendererScript lrL;
    [SerializeField] private LineRendererScript lrR;
    [SerializeField] private Transform leftPivot;
    [SerializeField] private Transform rightPivot;

    private Rigidbody2D currentBallRb;
    private SpringJoint2D currentBallSpringJoint;
    private Camera mainCamera;
    private bool isDragging = false;

    void Start()
    {
        mainCamera = Camera.main;
        SpawnNewBall();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (currentBallRb == null) { return; }

        if(!Touchscreen.current.primaryTouch.press.isPressed) 
        {
            currentBallRb.isKinematic = false;
            if (isDragging)
            {
                LaunchBall();
            }

            isDragging = false;
            return; 
        }

        isDragging = true;
        currentBallRb.isKinematic = true;

        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);

        currentBallRb.position = worldPosition;
        

        Debug.Log("Touch position: " + touchPosition);
        Debug.Log("World position: " + worldPosition);
    }    

    private void LaunchBall()
    {
        currentBallRb.isKinematic = false;
        currentBallRb = null;

        Invoke(nameof(DetachBall), delayDuration);
    }

    private void DetachBall()
    {
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);
        lrL.DestroyLine();
        lrR.DestroyLine();
    }

    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRb = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();
        currentBallSpringJoint.connectedBody = pivot;

        Transform[] LeftLinePoints = { currentBallRb.transform, leftPivot };
        Transform[] RightLinePoints = { currentBallRb.transform, rightPivot };

        lrL.SetUpLine(LeftLinePoints);
        lrR.SetUpLine(RightLinePoints);
    }

}
