using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gismo.Tools
{
    public class BoundaryController : MonoBehaviour
    {
        SpriteRenderer render;
        BoxCollider2D boxCollider;
        private void Awake()
        {
            render = GetComponent<SpriteRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();
        }

        public void SetBoundarySize(Vector2 size)
        {
            SetBoundarySize(size, size);
        }

        public void SetBoundarySize(Vector2 rendererSize, Vector2 colliderSize)
        {
            render.size = rendererSize;
            boxCollider.size = colliderSize;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetScale(Vector3 scale)
        {
            transform.localScale = scale;
        }
        public Vector3 GetScale()
        {
            return transform.localScale;
        }

        public void SetCollidability(bool canCollide, bool setVisibilty = true)
        {
            boxCollider.enabled = canCollide;

            if (setVisibilty)
                render.enabled = canCollide;
        }
    }
}
