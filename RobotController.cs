using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    // naming constraints do not change
    [SerializeField] private WheelCollider WCFrontLeft;
    [SerializeField] private WheelCollider WCFrontRight;
    [SerializeField] private WheelCollider WCBackLeft;
    [SerializeField] private WheelCollider WCBackRight;

    [SerializeField] private Transform WTFrontLeft;
    [SerializeField] private Transform WTFrontRight;
    [SerializeField] private Transform WTBackLeft;
    [SerializeField] private Transform WTBackRight;

    [SerializeField] private Transform RCFR;
    [SerializeField] private Transform RCL1;
    [SerializeField] private Transform RCL2;
    [SerializeField] private Transform RCL3;
    [SerializeField] private Transform RCR1;
    [SerializeField] private Transform RCR2;
    [SerializeField] private Transform RCR3;
    [SerializeField] private Transform AGOR;

    public float CarSpeed=20f;
    public float TurnSpeed=30f;
    public float minDistanceOfObstacle =10f;
    public float minAngle= 60f;

    private void FixedUpdate()
    {
        Drive();
    }

    private void Drive()
    {   
        Accelerate();
        TurnAngles();
        DodgeObstacles();
        
    }

    private void Accelerate()
    {
        WCFrontLeft.motorTorque = CarSpeed;
        WCFrontRight.motorTorque = CarSpeed;
        WCBackLeft.motorTorque = CarSpeed;
        WCBackRight.motorTorque = CarSpeed;

        WheelSpiner(WCFrontLeft, WTFrontLeft);
        WheelSpiner(WCFrontRight, WTFrontRight);
        WheelSpiner(WCBackLeft, WTBackLeft);
        WheelSpiner(WCBackRight, WTBackRight);
    }

    private void WheelSpiner(WheelCollider WC, Transform WT)
    {
        Quaternion rotation;
        Vector3 position;
        WC.GetWorldPose(out position, out rotation);
        WT.rotation = rotation;
    }

    private void TurnAngles()
    {
        Vector3 orientationEulerAngles = AGOR.eulerAngles;
        if (Mathf.Abs(orientationEulerAngles.y) > minAngle)
        {
            SpinCar(-Mathf.Sign(orientationEulerAngles.y)*TurnSpeed*Time.deltaTime);
        }
    }

    private void SpinCar(float direction)
    {
        transform.Rotate(Vector3.up, direction * TurnSpeed * Time.deltaTime);
    }

    private int SpinDirn= 0; //1->right, -1->left
    
    private void DodgeObstacles()
    {
        RaycastHit hit;
        if (Physics.Raycast(RCFR.position, RCFR.forward, out hit, minDistanceOfObstacle) || Physics.Raycast(RCL1.position, RCL1.forward, out hit, minDistanceOfObstacle) || Physics.Raycast(RCL2.position, RCL2.forward, out hit, minDistanceOfObstacle) || Physics.Raycast(RCL3.position, RCL3.forward, out hit, minDistanceOfObstacle) || Physics.Raycast(RCR1.position, RCR1.forward, out hit, minDistanceOfObstacle) || Physics.Raycast(RCR2.position, RCR2.forward, out hit, minDistanceOfObstacle) || Physics.Raycast(RCR3.position, RCR3.forward, out hit, minDistanceOfObstacle))
        { 
            if (SpinDirn == 0)
            {
                if (!Physics.Raycast(RCR1.position, RCR1.forward, out hit, minDistanceOfObstacle)
                    && !Physics.Raycast(RCR2.position, RCR2.forward, out hit, minDistanceOfObstacle)
                    && !Physics.Raycast(RCR3.position, RCR3.forward, out hit, minDistanceOfObstacle))
                {
                    SpinDirn = -1; //spin left
                }
                else if (!Physics.Raycast(RCL1.position, RCL1.forward, out hit, minDistanceOfObstacle)
                    && !Physics.Raycast(RCL2.position, RCL2.forward, out hit, minDistanceOfObstacle)
                    && !Physics.Raycast(RCL3.position, RCL3.forward, out hit, minDistanceOfObstacle))
                {
                    SpinDirn = 1; //spin right
                }
            }

            SpinCar(SpinDirn);
        }
        else
        {
            SpinDirn= 0; 
            Accelerate();
        }

        bool IsOnTrack = Physics.Raycast(transform.position, Vector3.down, out hit, 1f, LayerMask.GetMask("Road"));
        if (!IsOnTrack)
        {
            SteerTowardsTrack();
        }
    }

    private void SteerTowardsTrack()
    {
        RaycastHit hit;
        bool leftSideHit = Physics.Raycast(RCL1.position, -transform.up, out hit, 1f, LayerMask.GetMask("Road"));
        bool rightSideHit = Physics.Raycast(RCR1.position, -transform.up, out hit, 1f, LayerMask.GetMask("Road"));

        if(leftSideHit && rightSideHit) Accelerate();
  
        else if(leftSideHit) SpinCar(-1f);
        
        else if(rightSideHit) SpinCar(1f);
        
        else StopCar();
    }

    private void StopCar()
    {
        WCFrontLeft.motorTorque= 0f;
        WCFrontRight.motorTorque= 0f;
        WCBackLeft.motorTorque= 0f;
        WCBackRight.motorTorque= 0f;
    }


    
}