using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Penguin : MonoBehaviour
{
    private const float RANGE               = 0.5f;
    private const float RANGE_SEPARATATION  = 0.3f;
    private const float WEIGHT_SEPARATATION = 0.03f;
    private const float WEIGHT_ALIGNMENT    = 0.01f;
    private const float WEIGHT_CONHENSION   = 0.05f;
    private const float MAX_VELOCITY        = 0.01f;

    private Vector3 myVelocity;
    private Vector3 myPosition;

    // Start is called before the first frame update
    void Start()
    {
        // sphereCol = GetComponent<SphereCollider>();
        // colliderFlag = false;

        myVelocity = new Vector3(Random.Range(-1 * MAX_VELOCITY, MAX_VELOCITY), 0.0f, Random.Range(-1 * MAX_VELOCITY, MAX_VELOCITY));
    }

    // Update is called once per frame
    void Update()
    {

        myPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        Collider[] colliders = Physics.OverlapSphere(myPosition, RANGE);
        List<Vector3> objectsPosition = new List<Vector3>();
        List<Vector3> penguinsPosition = new List<Vector3>();
        List<Vector3> penguinsVelocity = new List<Vector3>();

        foreach(var collider in colliders) {
            string name = collider.name;
            if (string.Compare(name, "Floor") == 0 || string.Compare(name, this.gameObject.name) == 0) continue;

            GameObject otherGameObject = GameObject.Find(name);

            if (string.Compare(name, "Cube (1)") == 0 || string.Compare(name, "Cube (2)") == 0 || string.Compare(name, "Cube") == 0 || string.Compare(name, "Cube (3)") == 0) {
                Vector3 wallPosition = otherGameObject.transform.position;
                objectsPosition.Add(wallPosition);
            } else {
                Penguin penguin = otherGameObject.GetComponent<Penguin>();
                Vector3 penguinPosition = new Vector3(penguin.getX(), penguin.getY(), penguin.getZ());
                Vector3 penguinVelocity = new Vector3(penguin.getVX(), penguin.getVY(), penguin.getVZ());
                objectsPosition.Add(penguinPosition);
                penguinsPosition.Add(penguinPosition);
                penguinsVelocity.Add(penguinVelocity);
            }
        }

        Vector3 sumForce = Vector3.zero;
        if (penguinsPosition.Count != 0) {
            sumForce = WEIGHT_SEPARATATION * separatation(objectsPosition) + WEIGHT_ALIGNMENT * allignment(penguinsVelocity) + WEIGHT_CONHENSION * conhension(penguinsPosition);
        }

        Debug.Log("separatation:" + separatation(penguinsPosition).ToString("F4"));
        Debug.Log("allignment  :" + allignment(penguinsVelocity).ToString("F4"));
        Debug.Log("conhension  :" + conhension(penguinsPosition).ToString("F4"));
        
        updateVelocity(sumForce);
        Debug.Log(sumForce);
        updatePosition(myVelocity);

        // move();
    }

    private void updateVelocity(Vector3 force) {
        myVelocity += force;
        if (myVelocity.x > MAX_VELOCITY) myVelocity.x = MAX_VELOCITY;
        if (myVelocity.y > MAX_VELOCITY) myVelocity.y = MAX_VELOCITY;
        if (myVelocity.z > MAX_VELOCITY) myVelocity.z = MAX_VELOCITY;
        if (myVelocity.x < -1 * MAX_VELOCITY) myVelocity.x = -1 * MAX_VELOCITY;
        if (myVelocity.y < -1 * MAX_VELOCITY) myVelocity.y = -1 * MAX_VELOCITY;
        if (myVelocity.z < -1 * MAX_VELOCITY) myVelocity.z = -1 * MAX_VELOCITY;
    }

    private void updatePosition(Vector3 velocity) {
        // Debug.Log(v);
        this.transform.Translate(velocity.x, 0.0f, velocity.z);
    }

    private void OnTriggerEnter(Collider other) {


    }

    private Vector3 devide(Vector3 a, Vector3 b) {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    private Vector3 devide(Vector3 a, float s) {
        return new Vector3(a.x / s, a.y / s, a.z / s);
    }

    private Vector3 separatation(List<Vector3> penguinsPosition) {
        Vector3 forseSeparatation = new Vector3(0.0f, 0.0f, 0.0f);

        // foreach(Vector3 penguinPosition in penguinsPosition) {
        //     Debug.Log("my→" + myPosition.ToString("F4") + ", pe→" + penguinPosition.ToString("F4"));
        // }

        foreach(Vector3 penguinPosition in penguinsPosition) {
            float distance = Vector3.Distance(this.transform.position, penguinPosition);
            // Debug.Log("pos = " + penguinPosition);
            // Debug.Log("dis = " + distance);
            if (distance < RANGE_SEPARATATION) {
                Vector3 diff = myPosition - penguinPosition;
                Debug.Log("diff = " + diff.ToString("F4"));
                if (!(diff == Vector3.zero)) {
                    // Debug.Log("diff→" + diff);
                    forseSeparatation += devide(diff, (Vector3.Scale(diff, diff)));
                    // Debug.Log("myPosition:" + myPosition);
                    // Debug.Log("pePosition:" + penguinPosition);
                }
                
            }
        }
        return forseSeparatation;
    }

    private Vector3 allignment(List<Vector3> penguinsVelocity) {
        Vector3 swarmVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        if (penguinsVelocity.Count == 0) return swarmVelocity;

        foreach(Vector3 penguinVelocity in penguinsVelocity) {
            // Debug.Log("pen→" + penguinVelocity);
            swarmVelocity += penguinVelocity;
        }

        swarmVelocity = devide(swarmVelocity, (float)penguinsVelocity.Count);

        // Debug.Log(swarmVelocity);
        return (myVelocity - swarmVelocity);
    }

    private Vector3 conhension(List<Vector3> penguinsPosition) {
        Vector3 swarmPosition = new Vector3(0.0f, 0.0f, 0.0f);
        foreach(Vector3 penguinPosition in penguinsPosition) {
            swarmPosition += penguinPosition;
        }
        devide(swarmPosition, (float)penguinsPosition.Count);
        // swarmPosition /= (float)penguinsPosition.Count;
        return (myPosition - swarmPosition);
    }

    public float getX() {
        return this.transform.position.x;
    }
    public float getY() {
        return this.transform.position.x;
    }
    public float getZ() {
        return this.transform.position.z;
    }
    public float getVX() {
        return myVelocity.x;
    }
    public float getVY() {
        return myVelocity.y;
    }
    public float getVZ() {
        return myVelocity.z;
    }

    private void move() {
        if (Input.GetKey(KeyCode.LeftArrow)) {
            this.transform.Translate(-0.01f,0.0f,0.0f);
        }
        if (Input.GetKey (KeyCode.RightArrow)) {
            this.transform.Translate(0.01f,0.0f,0.0f);
        }
        if (Input.GetKey (KeyCode.UpArrow)) {
            this.transform.Translate(0.0f,0.0f,0.01f);
        }
        if (Input.GetKey (KeyCode.DownArrow)) {
            this.transform.Translate(0.0f,0.0f,-0.01f);
        }
    }
}
