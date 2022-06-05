using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraArmApproach : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxInteractDistance = 100f;
    [SerializeField] private int layerNum1 = 8;
    [SerializeField] private int layerNum2 = 9;
    
    [HideInInspector] public bool beReady = true;
    private bool action = false;
    private bool moveTowards = false;
    private Vector3 posForCam;
    private Vector3 defaultPosForCam;
    private GameObject moveTo;
    private PlayerInput playerInput;
    private InputAction approachAction;
    private InputAction defCameraInputAction;
    private Camera cam;
    private DeviceInfo roboarm;
    private InteractableBase interact;


    private void Awake()
    {
        cam = gameObject.GetComponent<Camera>();
        roboarm = gameObject.GetComponentInParent<DeviceInfo>();
        playerInput = gameObject.GetComponentInParent<PlayerInput>();

        defCameraInputAction = playerInput.currentActionMap.FindAction("DefCamera");
        approachAction = playerInput.currentActionMap.FindAction("Approach");

        defCameraInputAction.performed += OnDefCamera;
        approachAction.performed += OnApproach;

        defaultPosForCam = gameObject.transform.localPosition;
        defCameraInputAction.Disable();
    }

    void Update()
    {
        if (roboarm.isActive) MovingToObject();
        OnDefCamera();
    }

    private void OnApproach(InputAction.CallbackContext context)
    {
        if (roboarm.isActive)
        {
            action = true;
            CheckHit(layerNum1, layerNum2);
        }
    }

    private void CheckHit(int layerNum1, int layerNum2)
    {
        if (action)
        {
            int layerMask1 = 1 << layerNum1;
            int layerMask2 = 1 << layerNum2;
            layerMask1 = layerMask1 | layerMask2;
            layerMask1 = ~layerMask1;
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
            if (Physics.Raycast(ray, out hit, maxInteractDistance, layerMask1))
            {
                interact = hit.transform.GetComponent<InteractableBase>();
                if (interact == null) return;
                if (interact.CameraApproach == true)
                {
                    EventAggregator.endMoving.Publish(gameObject);
                    defCameraInputAction.Enable();
                    moveTo = hit.collider.gameObject;
                    Transform[] transformForCam = hit.transform.GetComponentsInChildren<Transform>();
                    if (transformForCam[1] != null)
                    {
                        posForCam = transformForCam[1].position;
                        moveTowards = true;
                        moveTo.GetComponent<Collider>().enabled = false;
                    }
                }
            }
        }
    }

    private void MovingToObject()
    {
        if (moveTowards)
        {
            if (posForCam != defaultPosForCam)
            {
                beReady = false;
                float step = movementSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, posForCam, step);

                if (Vector3.Distance(transform.position, posForCam) < 0.001f)
                {
                    action = false;
                    moveTowards = false;
                }
            }
        }
    }

    private void OnDefCamera(InputAction.CallbackContext context)
    {
        if (moveTowards == false && transform.localPosition != defaultPosForCam)
        {
            transform.localPosition = defaultPosForCam;
            moveTo.GetComponent<Collider>().enabled = true;
            beReady = true;
            defCameraInputAction.Disable();
        }
    }

    private void OnDefCamera()
    {
        if (!beReady && !roboarm.isActive)
        {
            transform.localPosition = defaultPosForCam;
            moveTo.GetComponent<Collider>().enabled = true;
            beReady = true;
            defCameraInputAction.Disable();
        }
    }
}
