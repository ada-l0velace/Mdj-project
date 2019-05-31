using UnityEngine;

public class CameraController : MonoBehaviour {
    public float panSpeed = 1f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;
    public float scrollSpeed = 20f;
    public float minY = 10f;
    public float maxY = 120f;
    public Transform player;

    public float velocityX = 0, velocityY = 0, velocityZoom = 0;

    public bool disablePanning = false;
    public bool disableSelect = false;
    public bool disableZoom = false;

    public float maximumZoom = 1f;
    public float minimumZoom = 20f;
    public float selectLineWidth = 2f;

    public float lookDamper = 5f;
    
    private Vector2 currentRotation = new Vector2(0, 60);
    public float maxYAngle = 90f;

    void Start() {

    }

    float CeilOrFloor(float cord) {
        if (cord > 0)
            return 1;
        else if (cord < 0)
            return -1;
        else
            return 0;
    }
    // Update is called once per frame
    void Update() {

        float factor = Mathf.Pow(.99f, 1000*Time.deltaTime);
        Vector3 pos = transform.position;

        Vector3 forwardMovement = player.transform.forward * Input.GetAxis("Vertical") * panSpeed * Time.deltaTime;
        Vector3 rightMovement = player.transform.right * Input.GetAxis("Horizontal") * panSpeed * Time.deltaTime;
        Vector3 finalMovement = forwardMovement + rightMovement;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        velocityZoom += scroll * 100 * Time.deltaTime;
        velocityZoom *= Mathf.Pow(.99f, 1000 * Time.deltaTime);

        
        pos.x += finalMovement.x;
        pos.z += finalMovement.z;
        pos.y -= 100f * velocityZoom * Time.deltaTime;

        velocityX *= factor;
        velocityY *= factor;
        velocityZoom += scroll;


        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y-20f);
        
        transform.position = pos;
        player.transform.position = pos;
        if (Input.GetMouseButton(2)) {
            currentRotation.x += Input.GetAxis("Mouse X") * 10f;
            currentRotation.y -= Input.GetAxis("Mouse Y") * 10f;
            currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
            currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);
            //Camera.main.transform.Rotate(Vector3.left * currentRotation.y);
            //player.transform.rotation = transform.rotation;
            Camera.main.transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
            player.transform.rotation = Quaternion.Euler(0, currentRotation.x, 0);
            //player.transform.rotation = transform.rotation;
        }
    }
    
}
