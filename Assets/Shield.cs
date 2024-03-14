using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OldSystems
{
    public class Shield : MonoBehaviour
    {
        public Transform shieldParent;
        public BoxCollider2D boxCollider { get; set; }
        public SpriteRenderer shieldRenderer { get; set; }
        public float hitForceMultiplier;
        public float hitForce;
        public Color highForceColor;
        public Color defaultColor;
        public LayerMask layerMask;
        public Vector3 hitPoint;
        public Vector3 colliderPoint;

        float angle;
        float distance;
        RaycastHit2D[] hitResults;

        Vector3 newScale;
        float returnToNormalLerpValue;
        float returnToNormalLerpSpeed;
        void Start()
        {
            boxCollider = GetComponent<BoxCollider2D>();
            shieldRenderer = GetComponentInChildren<SpriteRenderer>();
            newScale = Vector3.one;
            returnToNormalLerpValue = 1f;
        }
        void Update()
        {
            float lerpValue = Mathf.InverseLerp(0f, hitForceMultiplier, hitForce);
            shieldRenderer.color = Color.Lerp(defaultColor, highForceColor, lerpValue);

            transform.localScale = Vector3.Lerp(newScale, Vector3.one, returnToNormalLerpValue);
            if (returnToNormalLerpValue < 1)
            {
                returnToNormalLerpValue += Time.deltaTime * returnToNormalLerpSpeed;
            }
        }
        public void ScaleLerp(Vector2 newScale, float returnToNormalLerpSpeed)
        {
            this.newScale = newScale;
            this.returnToNormalLerpSpeed = returnToNormalLerpSpeed;
            returnToNormalLerpValue = 0;
        }
    }
}