using UnityEngine;

public class CameraController : MonoBehaviour {
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;
    public float scrollSpeed = 20f;
    public float minY = 10f;
    public float maxY = 120f;

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
        Vector3 pos = transform.position;
        if(Input.GetKey("w")) {// || Input.mousePosition.y >= Screen.height -panBorderThickness) {
            pos.z += panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("s")) {// || Input.mousePosition.y <= panBorderThickness) {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("d")) {// || Input.mousePosition.x >= Screen.width -panBorderThickness) {
            pos.x += panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("a")) { // || Input.mousePosition.x <= panBorderThickness) {
            pos.x -= panSpeed * Time.deltaTime;
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y-20f);
        
        transform.position = pos;

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
