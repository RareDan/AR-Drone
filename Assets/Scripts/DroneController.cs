using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DroneState
{
    IDLE,
    START_TAKING_OFF,
    TAKING_OFF,
    MOVING_UP,
    FLYING,
    START_LANDING,
    LANDING,
    LANDED,
    WAIT_ENGINE_STOP,
}

public class DroneController : MonoBehaviour
{
    private Animator animatorDrone;
    private Vector3 speed = Vector3.zero;
    public float speedMultiplier = 1f;

    public DroneState droneState;

    // Start is called before the first frame update
    void Start()
    {
        animatorDrone = GetComponent<Animator>();
        //animatorDrone.SetBool("TakeOff", true);

        droneState = DroneState.IDLE;
    }

    void UpdateDrone()
    {
        switch(droneState)
        {
            case DroneState.IDLE:
                break;

            case DroneState.START_TAKING_OFF:
                animatorDrone.SetBool("TakeOff", true);
                droneState = DroneState.TAKING_OFF;
                break;

            case DroneState.TAKING_OFF:
                if (animatorDrone.GetBool("TakeOff") == false)
                {
                    droneState = DroneState.MOVING_UP;
                }
                break;

            case DroneState.MOVING_UP:
                if (animatorDrone.GetBool("MoveUp") == false)
                {
                    droneState = DroneState.FLYING;
                }
                break;

            case DroneState.FLYING:
                float angleX = 30 * speed.z * 60 * Time.deltaTime;
                float angleZ = -30 * speed.x * 60 * Time.deltaTime;

                Vector3 rotation = transform.localRotation.eulerAngles;
                transform.localPosition += speed * speedMultiplier * Time.deltaTime;
                transform.localRotation = Quaternion.Euler(angleX, rotation.y, angleZ);
                break;

            case DroneState.START_LANDING:
                animatorDrone.SetBool("MoveDown", true);
                droneState = DroneState.LANDING;
                break;

            case DroneState.LANDING:
                if (animatorDrone.GetBool("MoveDown") == false)
                {
                    droneState = DroneState.LANDED;
                }
                break;

            case DroneState.LANDED:
                animatorDrone.SetBool("Land", true);
                droneState = DroneState.WAIT_ENGINE_STOP;
                break;

            case DroneState.WAIT_ENGINE_STOP:
                if (animatorDrone.GetBool("Land") == false)
                {
                    droneState = DroneState.IDLE;
                }
                break;
        }
    }

    public void Move(float speedX, float speedZ)
    {
        speed.x = speedX;
        speed.z = speedZ;

        UpdateDrone();
    }

    public bool IsIdle() => droneState == DroneState.IDLE;

    public bool IsFlying() => droneState == DroneState.FLYING;

    public void TakeOff()
    {
        droneState = DroneState.START_TAKING_OFF;
    }
    
    public void Land()
    {
        droneState = DroneState.START_LANDING;
    }
    /*    {
            return droneState == DroneState.IDLE;
            //return droneState == DroneState.IDLE ? true : false;
            *//* 
            if (droneState == DroneState.IDLE) return true;
            else return false;
            *//*
        }*/
}
