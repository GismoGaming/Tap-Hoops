using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gismo.Core
{
    public class BallMovement : MonoBehaviour
    {
        public enum CurrentDirection { Left,Right}

        [SerializeField] private CurrentDirection startingDirection;

        public CurrentDirection CurrentFaceDirection { get; private set; }

        [SerializeField] private Vector2 leftForceDirection;
        [SerializeField] private Vector2 rightForceDirection;

        private float tapStartTime;

        [SerializeField] private float tapForceCurrent;

        public Vector2 tapForceTimeBounds;

        [SerializeField] private Transform tapForceCircle;
        private Vector3 tapForceCircleStartSize;

        private Rigidbody2D rb;

        private bool canTap;

        [SerializeField] private float groundCheckDistance;
        [SerializeField] private LayerMask groundCheckLayerMask;
        [SerializeField] private Transform directionalArrow;

        [SerializeField] private Transform trailFXRoot;
        [SerializeField] private Transform nonRotatingItems;

        [SerializeField] private float maxSpeed;

        private bool isGrounded;

        [HideInInspector]
        public bool powerShotMode;

        void Awake()
        {
            CurrentFaceDirection = startingDirection;
            rb = GetComponent<Rigidbody2D>();
            tapForceCircleStartSize = tapForceCircle.localScale;

            leftForceDirection.Normalize();
            rightForceDirection.Normalize();

            ResetBall();
        }

        public void ResetBall()
        {
            tapForceCircle.localScale = Vector3.zero;

            tapStartTime = Time.time;
            canTap = false;

            StartCoroutine(QuickWaitBeforeInputStart());
        }

        IEnumerator QuickWaitBeforeInputStart()
        {
            yield return new WaitForSeconds(.1f);
            canTap = true;
        }

        private void FixedUpdate()
        {
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }

        void Update()
        {
            if (!Tools.ScreenBoundaries.ObjectInBounds(transform))
            {
                Vector3 pos = transform.position;

                if (transform.position.x < 0f)
                {
                    pos.x = Tools.ScreenBoundaries.GetPoint(Tools.PresetPoints.BottomRight).x;
                }
                else if (transform.position.x > 1f)
                {
                    pos.x = Tools.ScreenBoundaries.GetPoint(Tools.PresetPoints.BottomLeft).x;
                }

                transform.position = pos;
                nonRotatingItems.position = pos;

                foreach (TrailRenderer render in trailFXRoot.GetComponentsInChildren<TrailRenderer>())
                {
                    render.Clear();
                }
            }

            if (canTap && Statics.gameRunning)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    tapStartTime = Time.time;
                    tapForceCircle.localScale = tapForceTimeBounds.x * tapForceCircleStartSize;
                }

                if (Input.GetMouseButton(0))
                {
                    tapForceCircle.localScale = Mathf.Clamp(Time.time - tapStartTime + 1f, tapForceTimeBounds.x, tapForceTimeBounds.y) * tapForceCircleStartSize;
                }

                if(Input.GetMouseButtonUp(0))
                {
                    tapForceCircle.localScale = Vector3.zero;

                    tapForceCurrent = Mathf.Clamp(Time.time - tapStartTime + 1f, tapForceTimeBounds.x, tapForceTimeBounds.y);
                    Audio.AudioManager.Play("Ball Tap");
                    switch (CurrentFaceDirection)
                    {
                        case CurrentDirection.Left:
                            rb.AddForce(tapForceCurrent * Statics.currentForce * leftForceDirection);
                            break;
                        case CurrentDirection.Right:
                            rb.AddForce(tapForceCurrent * Statics.currentForce * rightForceDirection);
                            break;
                    }

                    if (powerShotMode)
                        Effects.CameraShake.instance.DoShake(0.15f, 0.4f);

                    if (!Statics.playerAddedForce)
                        Statics.playerAddedForce = true;

                    StartCoroutine(TapCooldown());
                }
            }

            if (Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundCheckLayerMask))
            {
                if(!isGrounded)
                { 
                    if (Statics.ballHasTouchedGround == false)
                        Statics.ballHasTouchedGround = true;
                    isGrounded = true;
                    Audio.AudioManager.Play("Ball Grounded");
                }
            }
            else
            {
                isGrounded = false;
            }
        }

        public void ArrowVis(bool isActive)
        {
            directionalArrow.gameObject.SetActive(isActive);
        }

        public void SwapFaceDirection()
        {
            CurrentFaceDirection = CurrentFaceDirection switch
            {
                CurrentDirection.Left => CurrentDirection.Right,
                _ => CurrentDirection.Left,
            };

            switch (CurrentFaceDirection)
            {
                case CurrentDirection.Right:
                    directionalArrow.LeanRotateZ(45f, 2f).setEaseOutBounce();
                    break;
                case CurrentDirection.Left:
                default:
                    directionalArrow.LeanRotateZ(-45f, 2f).setEaseOutBounce();
                    break;
            }
        }

        IEnumerator TapCooldown()
        {
            canTap = false;
            yield return new WaitForSeconds(Statics.currentTapCooldown);
            canTap = true;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, leftForceDirection * 2f);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, rightForceDirection * 2f);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector3.down * groundCheckDistance);
        }
    }
}
