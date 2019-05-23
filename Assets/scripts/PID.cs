[System.Serializable]
public class PID {
	public float pFactor, iFactor, dFactor;
		
	float integral;
	float lastError;

    // handle modulus 360
    bool angular;
	
	
	public PID(float pFactor, float iFactor, float dFactor, bool angular = false) {
		this.pFactor = pFactor;
		this.iFactor = iFactor;
		this.dFactor = dFactor;
        this.angular = angular;
	}
	
	
	public float Update(float setpoint, float actual, float timeFrame) {
		float present = setpoint - actual;
        if (angular)
        {
            while (present >= 180) present -= 360;
            while (present < -180) present += 360;
        }

        integral += present * timeFrame;
        float deriv = (present - lastError);
        deriv /= timeFrame;

		lastError = present;
		return present * pFactor + integral * iFactor + deriv * dFactor;
	}
}
