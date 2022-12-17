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

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    void Start()
    {
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


        //clamps values around the slingshot

        Vector2 maxLeft = new Vector2(pivot.position.x - clampValue, pivot.position.y); // coordinates of max left point
        float angle = Vector3.Angle(maxLeft - pivot.position, new Vector2(worldPosition.x,worldPosition.y) - pivot.position); //angle between horizontal line and clickPoint

        //calculates clmap points on a circle based on an angle between horizontal line and clickPoint
        float ClampX = Mathf.Clamp(worldPosition.x, pivot.position.x - Mathf.Abs(Mathf.Cos(angle * Mathf.PI / 180)) * clampValue, pivot.position.x + Mathf.Abs(Mathf.Cos(angle * Mathf.PI / 180)) * clampValue);
        float ClampY = Mathf.Clamp(worldPosition.y, pivot.position.y - Mathf.Abs(Mathf.Sin(angle * Mathf.PI / 180)) * clampValue, pivot.position.y + Mathf.Abs(Mathf.Sin(angle * Mathf.PI / 180)) * clampValue);

        //float ClampX = Mathf.Clamp(worldPosition.x, pivot.position.x - clampValue, pivot.position.x + clampValue);
        //float ClampY = Mathf.Clamp(worldPosition.y, pivot.position.y - clampValue, pivot.position.y + clampValue);

        currentBallRb.position = new Vector3(ClampX, ClampY,worldPosition.z);
        
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

        Transform[] LeftLinePoints = { ballInstance.transform, leftPivot };
        Transform[] RightLinePoints = { ballInstance.transform, rightPivot };

        if (LeftLinePoints == null || RightLinePoints == null) { return; }
        lrL.SetUpLine(LeftLinePoints);
        lrR.SetUpLine(RightLinePoints);
    }

}
