using UnityEngine;
using UnityEngine.UI;

public class BallSwipe : MonoBehaviour
{
    [SerializeField]float throwForceX = 30f;
    [SerializeField]float throwForceY = 40f;
    [SerializeField]float throwForceZ = 20f;
    [SerializeField]Rigidbody ball;
    [SerializeField]private Text scoreF;
    [SerializeField]private Text timeF;
    private Vector3 initPosition, startingPos, endPos;
    private Vector2 direction, touchPos, touchPosition;
    private float startingTime, endTime, timeInterval, time = 1f;
    private TouchControlInput touchControls;
    private Camera cam;
    private int score = 0;
    private Ray rayP;
    private bool ballReleased = false, fingerOut = false, touchStart = false, hitScore = false;

    public bool NetCollide
    {
        set {hitScore = value;}
        get {return hitScore;}
    }

    public int Score
    {
        get {return score;}
        set {score += value;}
    }

    public int ResetScore
    {
        set {score = value;}
    }

    public float TimeCheck
    {
        get {return time;}
        set {time = value;}
    }


    void Awake()
    {
        if (!PlayerPrefs.HasKey("highScore"))
        {
            PlayerPrefs.SetInt("highScore", 0);
        }
        //instantiate your input system
        touchControls = new TouchControlInput();
        cam = Camera.main;
    }

    void OnEnable()

    {
        touchControls.Enable();
    }

    void OnDisable()
    {
        touchControls.Disable();
    }


    void FixedUpdate()
    {
        //countdown for the game
        time -= 0.01f * Time.deltaTime;
        timeF.text = time.ToString("0.##");
        scoreF.text = score.ToString();
        
        //read the position of your touch
        touchPos = touchControls.Touch.TouchPosition.ReadValue<Vector2>();
        rayP = cam.ScreenPointToRay(touchPos);

        if (Physics.Raycast(rayP, out RaycastHit raycastHit, float.MaxValue, LayerMask.GetMask("XRef")))
        {
            if (touchControls.Touch.TouchPhase.phase.ToString().Equals("Started") && !touchStart)
            {
                //initialize starting position with the current touch position
                startingPos = touchPos;
                startingPos.z = cam.transform.position.z;
                startingPos = cam.ScreenToWorldPoint(startingPos);
                //initialize starting time with the current time
                startingTime = Time.time;
                touchStart = true;

                initPosition.x = raycastHit.point.x;
                initPosition.y = 1.8f;
                initPosition.z = -8.85f;
                ball.velocity = Vector3.zero;
                ball.isKinematic = true;
                transform.position = initPosition;
                ballReleased = false;
                fingerOut = false;
                hitScore = false;
            }
        }
        else
        {
            fingerOut = true;
        }

 
        if (touchControls.Touch.TouchPhase.ReadValue<UnityEngine.InputSystem.TouchPhase>().ToString().Equals("Ended"))
        {
            touchStart = false;
            if (!ballReleased && fingerOut)
            {
                //assign end position with the value of the touch position before lifting the finger
                endPos = touchPos;
                endPos.z = cam.transform.position.z;
                endPos = cam.ScreenToWorldPoint(endPos);
                //assign end time with the value of the time recorded before lifting the finger
                endTime = Time.time;
                //time interval will be the difference of the end time and starting time
                timeInterval = endTime - startingTime;
                //the difference of the end position and the starting position is the direction of the ball
                direction = endPos - startingPos;
                
                ball.isKinematic = false;
                ball.AddForce(-direction.x * throwForceX, -direction.y * throwForceY, -throwForceZ / timeInterval);
                ballReleased = true;
            }
        }
    }
}
