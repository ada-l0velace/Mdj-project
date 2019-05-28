using UnityEngine;

public class CameraController : MonoBehaviour {
    public float panSpeed = 100000f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;
    public float scrollSpeed = 20f;
    public float minY = 10f;
    public float maxY = 120f;

    public float velocityX = 0, velocityY = 0, velocityZoom = 0;

    public bool disablePanning = false;
    public bool disableSelect = false;
    public bool disableZoom = false;

    public float maximumZoom = 1f;
    public float minimumZoom = 20f;
    public Color selectColor = Color.green;
    public float selectLineWidth = 2f;

    public float lookDamper = 5f;
    public string selectionObjectName = "RTS Selection";

    private readonly string[] INPUT_MOUSE_BUTTONS = { "Mouse Look", "Mouse Select" };
    private GameObject selection;
    private bool[] isDragging = new bool[2];
    private Vector3 selectStartPosition;
    private Texture2D pixel;
    private Vector2 currentRotation = new Vector2(0, 60);
    public float maxYAngle = 90f;

    void Start() {
        setPixel(selectColor);

    }

    private void setPixel(Color color) {
        pixel = new Texture2D(1, 1);
        pixel.SetPixel(0, 0, color);
        pixel.Apply();
    }
    private bool isClicking(int index) {
        return Input.GetMouseButton(index);
    }
    // Update is called once per frame
    void Update() {

        float factor = Mathf.Pow(.99f, 1000*Time.deltaTime);
        Vector3 pos = transform.position;
        if(Input.GetKey("w")) {// || Input.mousePosition.y >= Screen.height -panBorderThickness) {
            velocityY += panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("s")) {// || Input.mousePosition.y <= panBorderThickness) {
            velocityY -= panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("d")) {// || Input.mousePosition.x >= Screen.width -panBorderThickness) {
            velocityX += panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("a")) { // || Input.mousePosition.x <= panBorderThickness) {
            velocityX -= panSpeed * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        velocityZoom += scroll * 100 * Time.deltaTime;
        velocityZoom *= Mathf.Pow(.99f, 1000 * Time.deltaTime);

        pos.x += 10* velocityX * Time.deltaTime;
        pos.z += 10* velocityY * Time.deltaTime;
        pos.y -= 100f * velocityZoom * Time.deltaTime;

        velocityX *= factor;
        velocityY *= factor;
        velocityZoom += scroll;


        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y-20f);
        
        transform.position = pos;
        if (Input.GetMouseButton(2)) {
            currentRotation.x += Input.GetAxis("Mouse X") * 10f;
            currentRotation.y -= Input.GetAxis("Mouse Y") * 10f;
            currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
            currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);
            Camera.main.transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        }
        //updateDragging();
    }
    private void dropSelection(Vector3 screenStart, Vector3 screenEnd) {
        if (!selection) {
            selection = new GameObject(selectionObjectName);
            {
                var collider = selection.AddComponent<BoxCollider>() as BoxCollider;
                collider.isTrigger = true;
                var size = collider.size;
                size.z = 1000000f;  // super friggin tall
                collider.size = size;
            }
            {
                var body = selection.AddComponent<Rigidbody>() as Rigidbody;
                body.useGravity = false;
            }
        }
        {
            var start = Camera.main.ScreenToWorldPoint(screenStart);
            var finish = Camera.main.ScreenToWorldPoint(screenEnd);
            selection.transform.position = new Vector3(
                (start.x + finish.x) / 2,
                (start.y + finish.y) / 2,
                0.5f);
            selection.transform.localScale = new Vector3(
                Mathf.Abs(start.x - finish.x),
                Mathf.Abs(start.y - finish.y),
                1f);
        }
    }

    private void updateDragging() {
        for (int index = 0; index <= 1; index++) {
            if (isClicking(index) && !isDragging[index]) {
                isDragging[index] = true;
                if (index == 0) {
                    selectStartPosition = Input.mousePosition;
                }
            }
            else if (!isClicking(index) && isDragging[index]) {
                isDragging[index] = false;
                if (index == 0) {
                    //dropSelection(selectStartPosition, Input.mousePosition);
                }
            }
        }
    }
    private void updateSelect() {
        if (!isDragging[0] || disableSelect) { return; }
        var x = selectStartPosition.x;
        var y = Screen.height - selectStartPosition.y;
        var width = (Input.mousePosition - selectStartPosition).x;
        var height = (Screen.height - Input.mousePosition.y) - y;
        GUI.DrawTexture(new Rect(x, y, width, selectLineWidth), pixel);
        GUI.DrawTexture(new Rect(x, y, selectLineWidth, height), pixel);
        GUI.DrawTexture(new Rect(x, y + height, width, selectLineWidth), pixel);
        GUI.DrawTexture(new Rect(x + width, y, selectLineWidth, height), pixel);
    }

    /*void OnGUI() {
        updateSelect();
    }*/
    
}
