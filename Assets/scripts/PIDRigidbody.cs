using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIDRigidbody
{
    private PIDVector3 pidVelocity;
    private PIDVector3 pidRotation;
    private Vector3 accelLimit;
    private Vector3 torqueLimit;

    public PIDRigidbody(Vector3 velocityParams, Vector3 rotationParams, Vector3 accelLimit, Vector3 torqueLimit)
    {
        this.pidVelocity = new PIDVector3(velocityParams.x, velocityParams.y, velocityParams.z);
        this.pidRotation = new PIDVector3(velocityParams.x, velocityParams.y, velocityParams.z, true);
        this.accelLimit = accelLimit;
        this.torqueLimit = torqueLimit;
    }

    public void Update(Rigidbody rb, Vector3 desiredVelocity, Vector3 desiredRotation, float timeFrame)
    {
        Vector3 accel = this.pidVelocity.Update(desiredVelocity, rb.velocity, timeFrame);
        Vector3 torque = this.pidRotation.Update(desiredRotation, rb.transform.eulerAngles, timeFrame);

        accel = Vector3.Min(accel, this.accelLimit);
        accel = Vector3.Max(accel, -this.accelLimit);

        torque = Vector3.Min(torque, this.torqueLimit);
        torque = Vector3.Max(torque, -this.torqueLimit);

        /*
        Debug.Log("Act:   " + rb.velocity);
        Debug.Log("Des:   " + desiredVelocity);
        Debug.Log("Accel: " + accel);
        */

        rb.AddForce(accel*timeFrame, ForceMode.Acceleration);
    }
}
