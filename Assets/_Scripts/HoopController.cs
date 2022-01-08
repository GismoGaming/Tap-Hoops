using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gismo.Core
{
    public class HoopController : MonoBehaviour
    {
        [SerializeField] private MultiTag multiTag;
        private Vector2 enterPoint;

        [SerializeField] private float longestScoreDelta;
        private float scoreDeltaStart;

        [SerializeField] private BallMovement playerBall;

        [Header("Hoop Settings")]
        [SerializeField] private Vector2 horizontalBounds;
        [SerializeField] private Vector2 verticalBounds;

        [SerializeField] private float moveSpeed;
        [SerializeField] private LeanTweenType moveTweenType;

        [SerializeField] private float scaleSpeed;

        private Vector3 startScale;

        private bool canScore;

        public int currentHoopCount;
        [SerializeField] private int maxHoopBeforeArrowShut;

        private void Awake()
        {
            startScale = transform.localScale;
            canScore = true;

            Vector2 temp = verticalBounds;
            verticalBounds = new Vector2(0.3f, .7f);
            ChangeHoopPosition();

            verticalBounds = temp;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(multiTag.CompareTag(collision.gameObject.tag))
            {
                scoreDeltaStart = Time.time;
                enterPoint = collision.ClosestPoint(transform.position);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if(multiTag.CompareTag(collision.gameObject.tag))
            {
                if (scoreDeltaStart + longestScoreDelta > Time.time)
                {
                    OnExit(collision.ClosestPoint(transform.position));
                }
            }
        }

        private void OnExit(Vector2 exitPoint)
        {
            if (!canScore)
                return;

            if (exitPoint.y + transform.localScale.y < enterPoint.y)
            {
                GameController.instance.OnScore(ScoreStat.Downwards);
            }
            else if(exitPoint.y > enterPoint.y + transform.localScale.y)
            {
                GameController.instance.OnScore(ScoreStat.Upwards);
            }
            else
            {
                return;
            }

            Audio.AudioManager.Play("Hoop Made");

            ChangeHoopPosition();

            if(currentHoopCount >= maxHoopBeforeArrowShut)
            {
                playerBall.ArrowVis(false);
            }
            else
            {
                currentHoopCount++;
            }
        }

        [ContextMenu("Downwards Dunk")]
        public void TEST_SCORED()
        {
            GameController.instance.OnScore(ScoreStat.Downwards);
            ChangeHoopPosition();
        }

        [ContextMenu("Upwards Dunk")]
        public void TEST_SCOREU()
        {
            GameController.instance.OnScore(ScoreStat.Upwards);
            ChangeHoopPosition();
        }

        private void ChangeHoopPosition()
        {
            Vector3 newPosition = new Vector3(Random.Range(horizontalBounds.x, horizontalBounds.y),Random.Range(verticalBounds.x,verticalBounds.y), transform.position.z);

            Vector3 newScale = startScale;

            playerBall.SwapFaceDirection();

            if(playerBall.CurrentFaceDirection == BallMovement.CurrentDirection.Left)
            {
                newPosition.x = 1f - newPosition.x;
                newScale.x *= -1f;
            }
            canScore = false;
            transform.LeanMove(Tools.ScreenBoundaries.GetPoint(newPosition), moveSpeed).setEase(moveTweenType).setOnComplete(() => canScore = true);
            transform.LeanScale(newScale, scaleSpeed);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(enterPoint, new Vector3(enterPoint.x, enterPoint.y + transform.localScale.y));

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(enterPoint, new Vector3(enterPoint.x, enterPoint.y - transform.localScale.y));
        }
    }
}
