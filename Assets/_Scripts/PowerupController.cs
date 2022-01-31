using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gismo.Core.Shop
{
    public class PowerupController : MonoBehaviour
    {
        [SerializeField] private Tools.BoundaryController boundLeft;
        [SerializeField] private Tools.BoundaryController boundRight;

        private Vector2 gravityNormal;
        [SerializeField] private Vector2 gravityLow;

        private void Start()
        {
            gravityNormal = Physics2D.gravity;
        }

        public void BrickBarriers(bool status)
        {
            boundLeft.SetCollidability(status, status);
            boundRight.SetCollidability(status, status);
        }

        public void GravityChange(bool status)
        {
            Physics2D.gravity = status ? gravityLow : gravityNormal;
        }
    }
}
