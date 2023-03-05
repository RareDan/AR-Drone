using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GameManager : MonoBehaviour
{
    public DroneController DroneController;
    public Button FlyButton;
    public Button LandButton;
    public GameObject ControlsPanel;

    public ARRaycastManager RaycastManager;
    public ARPlaneManager PlaneManager;
    private List<ARRaycastHit> HitResult = new();
    public GameObject Drone;

    struct DroneAnimationControls
    {
        public bool moving;
        public bool interpolatingAsc;
        public bool interpolatingDesc;
        public float axis;
        public float direction;
    }

    DroneAnimationControls movingX;
    DroneAnimationControls movingY;



    void Start()
    {
        Application.targetFrameRate = 60;
        FlyButton.onClick.AddListener(EventOnClickFlyButton);
        LandButton.onClick.AddListener(EventOnClickLandButton);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*        float speedX = Input.GetAxis("Horizontal");
                float speedY = Input.GetAxis("Vertical");*/

       UpdateControls(ref movingX);
       UpdateControls(ref movingY);

       DroneController.Move(
           movingX.axis * movingX.direction,
           movingY.axis * movingY.direction
       );

        if(DroneController.IsIdle())
        {
            UpdateAR();
        }
    }

    void UpdateAR()
    {
        Vector2 positionScreenSpace = Camera.current.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        RaycastManager.Raycast(positionScreenSpace, HitResult, TrackableType.PlaneWithinBounds);

        if (HitResult.Count > 0)
        {
            TrackableId trackableId = HitResult[0].trackableId;
            ARPlane plane = PlaneManager.GetPlane(trackableId);

            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                Pose pose = HitResult[0].pose;
                Drone.transform.position = pose.position;
                Drone.SetActive(true);
            }
        }
    }

    void UpdateControls(ref DroneAnimationControls controls)
    {
        if (controls.moving 
            || controls.interpolatingAsc 
            || controls.interpolatingDesc
            )
        {
            if (controls.interpolatingAsc)
            {
                controls.axis += 0.05f;

                if (controls.axis >= 1)
                {
                    controls.axis = 1f;
                    controls.interpolatingAsc = false;
                    controls.interpolatingDesc = true;
                }
            }
            else if (!controls.moving)
            {
                controls.axis -= 0.05f;

                if (controls.axis <= 0)
                {
                    controls.axis -= 0;
                    controls.interpolatingDesc = false;
                }
            }
        }
    }

    void EventOnClickFlyButton()
    {
        if (DroneController.IsIdle())
        {
            DroneController.TakeOff();
            FlyButton.gameObject.SetActive(false);
            ControlsPanel.SetActive(true);
        }
    }

    void EventOnClickLandButton()
    {
        if (DroneController.IsFlying())
        {
            DroneController.Land();
            FlyButton.gameObject.SetActive(true);
            ControlsPanel.SetActive(false);
        }
    }

    public void EvenentOnLeftButtonPressed()
    {
        movingX.moving = true;
        movingX.interpolatingAsc = true;
        movingX.direction = -1;
    }

    public void EvenentOnLeftButtonReleased()
    {
        movingX.moving = false;
    }

    public void EvenentOnRightButtonPressed()
    {
        movingX.moving = true;
        movingX.interpolatingAsc = true;
        movingX.direction = 1;
    }

    public void EvenentOnRightButtonReleased()
    {
        movingX.moving = false;
    }

    public void EvenentOnForwardButtonPressed()
    {
        movingY.moving = true;
        movingY.interpolatingAsc = true;
        movingY.direction = 1;
    }

    public void EvenentOnForwardButtonReleased()
    {
        movingY.moving = false;
    }

    public void EvenentOnBackwardButtonPressed()
    {
        movingY.moving = true;
        movingY.interpolatingAsc = true;
        movingY.direction = -1;
    }

    public void EvenentOnBackwardButtonReleased()
    {
        movingY.moving = false;
    }
}
