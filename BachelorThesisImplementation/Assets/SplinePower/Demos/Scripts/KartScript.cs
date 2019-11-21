using UnityEngine;
using System.Collections;
using System.Linq;

namespace SplinePower.Demos.Scripts
{
    public class KartScript : MonoBehaviour
    {
        public enum ControlType
        {
            Player,
            Bot
        };

        public ControlType controlType;

        private float _maxSpeed = 8;
        private SplineFormer _splineFormer;
        private Waypoint _closestNode;
        private float _accelerateAxis;
        private float _turnAxis;

        private Waypoint _nextNode;
        private Waypoint _currentNode;
        private float _random;

        // Use this for initialization
        void Start()
        {
            _splineFormer = GameObject.Find("Road").GetComponent<SplineFormer>();
            _random = UnityEngine.Random.value;
        }

        // Update is called once per frame
        void Update()
        {
        }

        void FixedUpdate()
        {
            _closestNode = GetClosest();

            if (controlType == KartScript.ControlType.Player)
            {
                _accelerateAxis = Input.GetAxis("Vertical");
                _turnAxis = Input.GetAxis("Horizontal");
            }
            else if (controlType == ControlType.Bot)
            {
                if (_currentNode == null)
                {
                    _currentNode = _closestNode;
                }
                if (_nextNode == null)
                {
                    _nextNode = _currentNode.Next;
                    if (_nextNode == null) _nextNode = _splineFormer.WaypointsModule.Waypoints.First();
                }

                if (Vector3.Distance(transform.position, _nextNode.Position) < 2)
                {
                    _currentNode = _nextNode;
                    _nextNode = _currentNode.Next;
                    if (_nextNode == null) _nextNode = _splineFormer.WaypointsModule.Waypoints.First();
                }

                var direction = _nextNode.Position - (_currentNode.Position + transform.position) * 0.5f;
                var angleBetween = Quaternion.FromToRotation(transform.forward, direction).eulerAngles.y;
                float turnLerp = 1 * Time.deltaTime;
                float accelerateLerp = 3f * Time.deltaTime;

                if (angleBetween > 180) angleBetween -= 360;

                if (angleBetween > 15)
                {
                    _turnAxis = Mathf.Lerp(_turnAxis, 1, turnLerp);
                }
                else if (angleBetween < -15)
                {
                    _turnAxis = Mathf.Lerp(_turnAxis, -1, turnLerp);
                }

                if (Mathf.Abs(angleBetween) < 60)
                {
                    _accelerateAxis = Mathf.Lerp(_accelerateAxis, 1, accelerateLerp);
                }
                else
                {
                    _accelerateAxis = Mathf.Lerp(_accelerateAxis, 0, accelerateLerp);
                }

                _maxSpeed = 12 - _random; //dirty cheat

                if (Mathf.Abs(angleBetween) < 30 && Mathf.Abs(_accelerateAxis) > 0.95) //a little bit of drunk driving
                {
                    _turnAxis = Mathf.Lerp(_turnAxis,
                        Mathf.Cos(Time.timeSinceLevelLoad * 0.001f + _random * 2 * Mathf.PI),
                        Mathf.Abs(_accelerateAxis) - (GetComponent<Rigidbody>().velocity.magnitude * 2));
                }
            }

            if (GetComponent<Rigidbody>().velocity.magnitude < _maxSpeed)
            {
                float force = Time.deltaTime * (500 - 100 * _random);
                GetComponent<Rigidbody>()
                    .AddRelativeForce(Vector3.forward * _accelerateAxis * force, ForceMode.Acceleration);
            }


            float angle = 180 * Time.deltaTime;
            transform.Rotate(Vector3.up, _turnAxis * angle);

            if (transform.position.y < -15)
            {
                transform.position = _closestNode.Position + Vector3.up * 5;
            }

            GetComponent<AudioSource>().pitch = GetComponent<Rigidbody>().velocity.magnitude / _maxSpeed * 8;
        }

        Waypoint GetClosest()
        {
            Waypoint resultNode = null;
            float resultDistance = float.MaxValue;
            foreach (var node in _splineFormer.WaypointsModule.Waypoints)
            {
                float currentDistance = Vector3.Distance(node.Position, transform.position);
                if (currentDistance < resultDistance)
                {
                    resultDistance = currentDistance;
                    resultNode = node;
                }
            }

            return resultNode;
        }
    }
}