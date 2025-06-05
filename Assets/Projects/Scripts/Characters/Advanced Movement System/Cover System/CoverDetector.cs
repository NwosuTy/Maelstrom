using UnityEngine;

namespace Creotly_Studios
{
    public class CoverDetector : MonoBehaviour
    {
        private Collider[] wallColliders;
        private CharacterManager characterManager;
        public CreotlyTransforms coverHitPoint { get; private set; }

        //Status
        public bool atLeftEdge { get; private set; }
        public bool atRightEdge { get; private set; }
        public bool isHighCover {  get; private set; }
 

        [Header("Character Parameters")]
        [SerializeField] private float heightOffset;
        [SerializeField] private float rotationSpeed;

        [Header("Cover Parameters")]
        [Range(0, 20f)][SerializeField] private float rayDistance;
        [Range(0, 5.0f)][SerializeField] private float detectRadius;

        [Header("Physics Parameters")]
        [SerializeField] private Transform highRayOrigin;
        [SerializeField] private Transform leftRayOrigin;
        [SerializeField] private Transform rightRayOrigin;
        [SerializeField] private LayerMask coverLayerMask;

        private void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }

        private void Start()
        {
            wallColliders = new Collider[20];
        }

        public void CoverDetector_Update()
        {
            if (characterManager.coverState != CoverState.InCover)
            {
                return;
            }
            CheckIfHitEdge();
            SetCoverType();
        }

        public void DetectCover()
        {
            Transform wallTransform = NearestWallPosition();
            GetHitPoints(wallTransform);
        }

        private void CheckIfHitEdge()
        {
            Vector3 fwd = transform.forward;

            isHighCover = IsHighCover(fwd);
            atLeftEdge = !Physics.Raycast(leftRayOrigin.position, fwd, 3.5f, coverLayerMask);
            atRightEdge = !Physics.Raycast(rightRayOrigin.position, fwd, 3.5f, coverLayerMask);

            if(isHighCover)
            {
                //At The Edge
                if(atLeftEdge || atRightEdge)
                {
                    //Can Aim
                }

                //Cannot Aim
                return;
            }
            //Can Aim
        }

        private bool IsHighCover(Vector3 fwd)
        {
            Vector3 offset = new(0.185f, 0.0f, 0.0f);
            Vector3 left = highRayOrigin.position - offset;
            Vector3 right = highRayOrigin.position + offset;

            bool lHigh = Physics.Raycast(left, fwd, 3.5f, coverLayerMask);
            bool rHigh = Physics.Raycast(right, fwd, 3.5f, coverLayerMask);

            return lHigh && rHigh;
        }

        private void SetCoverType()
        {
            //Crouch if High Cover Not True;
            characterManager.isCrouching = (isHighCover != true);
        }

        private Transform NearestWallPosition()
        {
            Transform nearestWall = null;
            float closestDis = float.MaxValue;
            characterManager.coverState = CoverState.NoCover;

            Vector3 currentPos = transform.position;
            int count = Physics.OverlapSphereNonAlloc(currentPos, detectRadius, wallColliders, coverLayerMask);
            for(int i = 0; i < count; i++)
            {
                if (wallColliders[i] == null)
                {
                    continue;
                }
                
                Vector3 wallPosition = wallColliders[i].transform.position;
                float distance = (wallPosition - currentPos).sqrMagnitude;
                if(distance < closestDis)
                {
                    closestDis = distance;
                    nearestWall = wallColliders[i].transform;
                }
            }
            return nearestWall;
        }

        private void GetHitPoints(Transform wall)
        {
            if(wall == null)
            {
                print(-1);
                return;
            }
            Vector3 origin = transform.position + Vector3.up * heightOffset;
            Vector3 direction = (wall.position - origin).normalized;

            Ray ray = new(origin, direction);
            if(Physics.Raycast(ray, out RaycastHit hit, rayDistance, coverLayerMask))
            {
                characterManager.coverState = CoverState.EnteringCover;
                coverHitPoint = new CreotlyTransforms(hit.point, hit.normal);

                Debug.DrawRay(origin, direction * rayDistance, Color.yellow);
                return;
            }
            characterManager.coverState = CoverState.NoCover;
            Debug.DrawRay(transform.position, direction * rayDistance, Color.red);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectRadius);          
        }
    }
}
