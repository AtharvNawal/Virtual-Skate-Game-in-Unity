using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Skateboard : MonoBehaviour
{
    public TrickTextController trickText;
    public ScoreTextController scoreText;
    public TotalScore totalScoreText;
    public CountRotations rotationCounter;
    public AudioManager audioManager;

    public SkateBoardHitTrigger topHitTrigger;
    public SkateBoardHitTrigger bottomHitTrigger;

    public TrailRenderer _trailA;
    public TrailRenderer _trailB;

    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Rigidbody _rigid;
    private bool _jumping;
    private bool _landed;
    private Vector3 _jumpForce = new Vector3(0, 4.5f, 0);
    private int potentialScore;

    private bool locked = true;

    public float moveSpeed = 10f;  // Speed of movement
    public float turnSpeed = 100f;  // Speed of turning

    // Use this for initialization
    private void Start()
    {
        _rigid = GetComponent<Rigidbody>();
        _startPosition = _rigid.position;
        _startRotation = _rigid.rotation;
    }

    public void activateMe()
    {
        locked = false;
    }

private void FixedUpdate()
{
    if (!locked)
    {
        // Handle movement and rotation while not jumping
        if (!_jumping)
        {
            // Get input for forward/backward movement
            float move = Input.GetAxis("Vertical");  // W/S or Up/Down arrows
            Vector3 forwardMovement = transform.forward * move * moveSpeed * Time.fixedDeltaTime;
            _rigid.MovePosition(_rigid.position + forwardMovement); // Move skateboard

            // Get input for left/right turning
            float turn = Input.GetAxis("Horizontal");  // A/D or Left/Right arrows
            Quaternion turnRotation = Quaternion.Euler(0f, turn * turnSpeed * Time.fixedDeltaTime, 0f);  // Rotation
            _rigid.MoveRotation(_rigid.rotation * turnRotation); // Apply rotation
        }

        // Handle tricks during jump
        if (Input.GetKeyDown(KeyCode.Space) && _jumping)
        {
            // Perform trick (example flip with parameters)
            doTrick(new Vector3(0, 0, 0), transform.position, 1, 180f);
        }

        // Allow jumping with 'Q' key
        if (Input.GetKeyDown(KeyCode.Q) && !_jumping)
        {
            jump(4.5f); // Call jump function, adjust force as needed
        }
    }

    // Reset game when "R" is pressed (only if grounded)
    if (Input.GetKeyDown(KeyCode.R) && !_jumping)
    {
        ResetToStartPosition();
    }
}

    private void OnTriggerEnter(Collider other)
    {
        if (!_jumping) return;

        _jumping = false;
        Landed();
         // Check if the skateboard leaves the grey surface area
        if (other.gameObject.tag == "GreySurface")
        {
            EndGame();  // Call function to end the game
        }
    }
    void EndGame()
    {
        Debug.Log("Game Over: Skateboard went out of bounds!");
        // Add your game over logic here
        // For example, reload the scene or show a game over UI
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the current scene
    }

    private void Landed()
    {
        bool landedTop = topHitTrigger.active;
        bool landedBottom = bottomHitTrigger.active;

        if (landedBottom)
        {
            audioManager.playLanded();
            totalScoreText.setScoreText(potentialScore);
        }
        else
        {
            audioManager.playFail();
            trickText.setScoreFail();
            scoreText.setScoreFail();
        }
        locked = false;
        potentialScore = 0;
    }

    public void doTrick(Vector3 force, Vector3 forcePos, float verticalForce, float trickAngle)
    {
        if (locked) return;

        locked = true;
        addForceAtPosition(force, forcePos);
        jump(verticalForce);
        startTrackingTrick(trickAngle);
        audioManager.playJump();
    }

    public void ResetToStartPosition()
    {
        resetTrickTracking();
        _jumping = false;
        _rigid.isKinematic = true;
        _rigid.position = _startPosition;
        _rigid.rotation = _startRotation;
        _rigid.isKinematic = false;
    }

    List<TrickData> trickList = new List<TrickData>() {
        new TrickData("heelflip", 90f, TrickData.TrickAxis.x, 100),
        new TrickData("impossible", 0f, TrickData.TrickAxis.z, 200),
        new TrickData("hardflip", 180f, TrickData.TrickAxis.z, 200),
        new TrickData("kickflip", -100f, TrickData.TrickAxis.x, 200),
        new TrickData("rest", -999f, TrickData.TrickAxis.z, 0)
    };

    private float _trickAngleTreshold = 30f;

    private void resetTrickTracking()
    {
        locked = false;
        rotationCounter.Reset();
    }

    public void startTrackingTrick(float angle)
    {
        var points = findTrick(angle).points;
        var name = findTrick(angle).name;
        trickText.AddTrick(name);
        scoreText.AddScore(points);
        potentialScore += points;
    }

    private void trackTrick()
    {
        if (Mathf.Abs(rotationCounter.completedXRotations) == 1 || Mathf.Abs(rotationCounter.completedZRotations) == 1)
        {
            trickText.fireCompleted();
            resetTrickTracking();
        }
    }

    private TrickData findTrick(float angle)
    {
        TrickData res = null;
        trickList.ForEach((TrickData t) =>
        {
            if (inRange(t.detectionAngle, angle))
            {
                res = t;
            }
        });

        if (res == null)
        {
            res = trickList[trickList.Count - 1];
        }

        return res;
    }

    private bool inRange(float range, float angle)
    {
        return modToRotation(angle) >= range - _trickAngleTreshold && modToRotation(angle) <= range + _trickAngleTreshold;
    }

    private float modToRotation(float rot)
    {
        return rot % 360f;
    }

    public void jump(float _force)
    {
        if (_jumping) return;

        _jumping = true;
        trickText.ClearText();
        scoreText.ClearText();

        float cappedJumpForce = Mathf.Min(_force, 30f);
        _rigid.AddForceAtPosition(new Vector3(_jumpForce.x, _jumpForce.y * cappedJumpForce, _jumpForce.z), transform.position);
    }

    public void addForceAtPosition(Vector3 _force, Vector3 _position)
    {
        _rigid.AddForceAtPosition(_force, _position);
    }
}