using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SausageScript : MonoBehaviour
{
    public bool gameOver;
    public bool gameWin;
    public Material sausageFried;
    [Header("Ground Settings")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private float rayLength; //Length to detect ground
    [SerializeField] private LayerMask rayMask; //Mask to detect ground
    [Header("Collider Bones")]
    public GameObject upperBone;
    public GameObject midBone; //Middle sausage collider bone
    public GameObject lowerBone;
    [Header("Cursor Settings")]
    public Camera cameraView;
    [SerializeField] private bool isAiming;
    [SerializeField] private LayerMask rayCursorMask; //Mask to detect sausage from camera
    [SerializeField] private float rayCameraLength; //Length to detect sausage from camera

    [Header("Sounds")]
    public AudioClip audioPull;
    public AudioClip audioJump;
    public AudioClip audioLand;
    public AudioClip audioDie;

    //Singleton instance
    private static SausageScript instance;
    public static SausageScript Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<SausageScript>();
            return instance;
        }
    }

    private bool rayDown; //Ray for ground detection
    private Ray rayCamera; //Ray for camera to detect sausage
    private RaycastHit rayCameraResult; //Needed for raycast but never used
    private Vector3 mouseClickPosition; //Start drag position
    private Vector3 mouseVector; //End drag position
    private GameObject aim; //Arrow mesh
    private double aimAngle; //Angle for arrow rotation based on mouse drag
    private Quaternion aimOrientation; //Save initial rotation of arrow
    private GameObject failText;
    private GameObject winText;
    private GameObject restartButton;
    private bool dieIsPlayed; //To not repeat die sound
    private bool pullIsPlayed; //To not repeat pull sound
    // Start is called before the first frame update
    void Start()
    {
        aim = GameObject.Find("Aim");
        failText = GameObject.Find("TextFail");
        winText = GameObject.Find("TextWin");
        restartButton = GameObject.Find("ButtonRestart");
        aimOrientation = aim.transform.rotation;
        HideFail();
        gameOver = false;

        //Load last active checkpoint (if present)
        GameObject[] objs = GameObject.FindGameObjectsWithTag("checkpoint");
        int currentOrder = 0;
        float checkpointX = upperBone.transform.position.x;
        float checkpointY = upperBone.transform.position.y;
        foreach (GameObject obj in objs)
        {
            if (obj.GetComponent<CheckPointControl>().checkPointActive && obj.GetComponent<CheckPointControl>().checkPointOrder>currentOrder)
            {
                currentOrder = obj.GetComponent<CheckPointControl>().checkPointOrder;
                checkpointX = obj.transform.position.x;
                checkpointY = obj.transform.position.y;
            }
        }
        upperBone.transform.position = new Vector3(checkpointX,checkpointY,upperBone.transform.position.z);
    }

    //Check if grounded and play sound
    private void checkGround()
    {
        bool prevGrounded = isGrounded;
        isGrounded = false;
        rayDown = Physics.Raycast(new Vector3(midBone.transform.position.x, midBone.transform.position.y, midBone.transform.position.z), Vector3.down, rayLength, rayMask);
        if (rayDown) isGrounded = true;
        rayDown = Physics.Raycast(new Vector3(upperBone.transform.position.x, upperBone.transform.position.y, upperBone.transform.position.z), Vector3.down, rayLength/2, rayMask);
        if (rayDown) isGrounded = true;
        rayDown = Physics.Raycast(new Vector3(lowerBone.transform.position.x, lowerBone.transform.position.y, lowerBone.transform.position.z), Vector3.down, rayLength / 2, rayMask);
        if (rayDown) isGrounded = true;
        Debug.DrawRay(new Vector3(midBone.transform.position.x, midBone.transform.position.y, midBone.transform.position.z), Vector3.down * rayLength, Color.red);

        //Play sound on landing
        //Debug.Log("Velocity magnitude: " + upperBone.GetComponent<Rigidbody>().velocity.magnitude);
        if (!prevGrounded && isGrounded && !gameOver && upperBone.GetComponent<Rigidbody>().velocity.magnitude>5)
        {
            upperBone.GetComponent<AudioSource>().clip = audioLand;
            upperBone.GetComponent<AudioSource>().Play();
        }
    }

    //Mouse input control
    private void checkMouse()
    {
        //Check if mouse button is pressed then start dragging
        if (Input.GetMouseButtonDown(0) && isGrounded)
        {
            rayCamera = cameraView.ScreenPointToRay(Input.mousePosition);
            //if (Physics.Raycast(rayCamera, out rayCameraResult, rayCameraLength, rayCursorMask))
            //{
                mouseClickPosition = Input.mousePosition;
                isAiming = true;

                aim.SetActive(true);
            //Debug.Log("Mouse clicked on sausage: "+mouseClickPosition);
            //}
            //upperBone.GetComponent<AudioSource>().clip = audioPull;
            //upperBone.GetComponent<AudioSource>().Play();
        }

        //Check if mouse button is held then calculate magnitude
        if (Input.GetMouseButton(0) && isGrounded && isAiming)
        {
            mouseVector = mouseClickPosition - Input.mousePosition;
            float mX = mouseVector.x / Screen.width;
            float mY = mouseVector.y / Screen.height;
            if (mY < 0) mY = 0;
            mouseVector = new Vector3(mX, mY, 0);
            mouseVector = Vector3.ClampMagnitude(mouseVector, 0.3f);
            //aim.transform.rotation = new Vector3(aim.transform.rotation);
            aim.transform.position = new Vector3(midBone.transform.position.x+0.5f, midBone.transform.position.y + 3f, midBone.transform.position.z - 5f);
            aimAngle = Math.Atan2(mouseVector.x * 0.5f, mouseVector.y) / Math.PI * 180;
            aim.transform.rotation = aimOrientation;
            aim.transform.Rotate(Vector3.up, (float)aimAngle, Space.Self);
            aim.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f + mouseVector.magnitude * 10);

            if (mouseVector.magnitude > 0.1f && !pullIsPlayed)
            {
                upperBone.GetComponent<AudioSource>().clip = audioPull;
                upperBone.GetComponent<AudioSource>().Play();
                pullIsPlayed = true;
            }
            //Debug.Log("Mouse dragged: " + Aim.transform.localScale);
        }
        else
        {
            //Hide arrow
            aim.SetActive(false);
        }

        //Check if mouse button is released then stop dragging and launch
        Rigidbody midBody = midBone.GetComponent<Rigidbody>();
        if (Input.GetMouseButtonUp(0) && isGrounded && isAiming)
        {
            midBody.AddForce(new Vector3(mouseVector.x * 500, mouseVector.y * 1000, 0), ForceMode.Impulse);
            isAiming = false;

            pullIsPlayed = false;
            if (mouseVector.magnitude > 0.1f)
            {
                upperBone.GetComponent<AudioSource>().clip = audioJump;
                upperBone.GetComponent<AudioSource>().Play();
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

        //Controls for debugging
        //TODO: remove it
        Rigidbody midBody = midBone.GetComponent<Rigidbody>();
        float hAxis = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            midBody.AddForce(new Vector3(0, 200, 0), ForceMode.Impulse);
        }
        if (hAxis != 0)
        {
            midBody.AddForce(new Vector3(hAxis*10, 0, 0), ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }


        //Check if grounded
        checkGround();

        //Mouse input
        if (!gameOver)
        {
            if (!gameWin)
            {
                checkMouse();
            }
            else
            {
                ShowWin();
            }
        }
        else 
        {
            //GAME OVER
            GameObject.Find("Torus").GetComponent<SkinnedMeshRenderer>().material = sausageFried;
            //midBody.AddForce(new Vector3(0, 1, 0), ForceMode.Impulse);
            ShowFail();
            if (!dieIsPlayed)
            {
                upperBone.GetComponent<AudioSource>().clip = audioDie;
                upperBone.GetComponent<AudioSource>().Play();
                dieIsPlayed = true;
            }
            //upperBone.GetComponent<CapsuleCollider>().enabled = false;
            //upperBone.GetComponent<Rigidbody>().isKinematic = true;
        }

        //Quit Game on Esc
        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
            //Debug.Log("Quit Game");
        }
    }

    public void HideFail()
    {
        failText.SetActive(false);
        winText.SetActive(false);
        restartButton.SetActive(false);
    }

    public void ShowFail()
    {
        failText.SetActive(true);
        restartButton.SetActive(true);
    }

    public void ShowWin()
    {
        winText.SetActive(true);
        restartButton.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
