using UnityEngine;
public class PIDVector3
{
    private PID pidX, pidY, pidZ;

    public PIDVector3(float pFactor, float iFactor, float dFactor, bool angular=false)
    {
        this.pidX = new PID(pFactor, iFactor, dFactor, angular);
        this.pidY = new PID(pFactor, iFactor, dFactor, angular);
        this.pidZ = new PID(pFactor, iFactor, dFactor, angular);
    }

    public Vector3 Update(Vector3 setpoint, Vector3 actual, float timeFrame)
    {
        float x = pidX.Update(setpoint.x, actual.x, timeFrame);
        float y = pidY.Update(setpoint.y, actual.y, timeFrame);
        float z = pidZ.Update(setpoint.z, actual.z, timeFrame);
        return new Vector3(x, y, z);
    }
}
