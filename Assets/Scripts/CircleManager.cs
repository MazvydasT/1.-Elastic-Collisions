using UnityEngine;

public class CircleManager : MonoBehaviour
{
    [SerializeField]
    GameObject circlePrefab;

    [SerializeField, Min(0)]
    int count = 10;

    [Space, SerializeField, Min(0)]
    float minRadius = 0.25f;

    [SerializeField, Min(0)]
    float maxRadius = 2.0f;

    [Space, SerializeField, Min(0)]
    float minSpeed = 0.0f;

    [SerializeField, Min(0)]
    float maxSpeed = 10.0f;

    [Space, SerializeField]
    float velocityMultiplier = 2f;

    [SerializeField]
    bool collisionDetection = false;

    struct CircleData
    {
        public Vector3 position;
        public Vector3 velocity;

        public float mass;

        public float radius;

        public Transform transform;
    }

    CircleData[] data;

    void Start()
    {
        (var min, var max) = GetWorldBouds();

        data = new CircleData[count];

        var propertyBlock = new MaterialPropertyBlock();

        for (var i = 0; i < count; ++i)
        {
            var radius = Random.Range(minRadius, maxRadius);

            var x = Random.Range(min.x + radius, max.x - radius);
            var y = Random.Range(min.y + radius, max.y - radius);

            var position = new Vector3(x, y, 0);

            var initialDirection = Random.insideUnitCircle.normalized;
            var initialSpeed = Random.Range(minSpeed, maxSpeed);

            var velocity = initialDirection * initialSpeed;

            var diameter = 2.0f * radius;
            var circleInstance = Instantiate(circlePrefab, position, Quaternion.identity, transform).transform;
            circleInstance.localScale = new Vector3(diameter, diameter, 1);

            // HSV colour graph
            // https://images.app.goo.gl/dqW2rSfFv3r1zzPR9

            var hue = Random.value;
            var circleColour = Color.HSVToRGB(hue, 1.0f, 1.0f);

            // MaterialPropertyBlock allows using per instance colour with GPU Instancing
            propertyBlock.SetColor("_Colour", circleColour);
            circleInstance.gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);

            data[i] = new CircleData
            {
                position = position,
                velocity = velocity,
                transform = circleInstance,
                mass = diameter * diameter,
                radius = radius
            };
        }
    }

    // Update is called once per frame
    void Update()
    {
        (var min, var max) = GetWorldBouds();

        // Move each circle position by velocity over time
        for (int i = 0, c = data.Length; i < c; ++i)
        {
            var circleData = data[i];
            circleData.position = circleData.transform.localPosition + Time.deltaTime * velocityMultiplier * circleData.velocity;

            data[i] = circleData;
        }

        // Run collision detection
        if (collisionDetection)
        {
            for (int firstCircleIndex = 0, c = data.Length; firstCircleIndex < c; ++firstCircleIndex)
            {
                var firstCircleData = data[firstCircleIndex];
                var firstCirclePosition = firstCircleData.position;
                var firstCircleVelocity = firstCircleData.velocity;
                var firstCircleMass = firstCircleData.mass;

                for (int secondCircleIndex = firstCircleIndex + 1; secondCircleIndex < c; ++secondCircleIndex)
                {
                    var secondCircleData = data[secondCircleIndex];
                    var secondCirclePosition = secondCircleData.position;
                    var secondCircleVelocity = secondCircleData.velocity;
                    var secondCircleMass = secondCircleData.mass;


                    var distanceVector = firstCirclePosition - secondCirclePosition;

                    var collisionThreshold = firstCircleData.radius + secondCircleData.radius;


                    if (distanceVector.sqrMagnitude >= collisionThreshold * collisionThreshold) continue;

                    var halfCollisionDistance = (collisionThreshold - distanceVector.magnitude) * .5f;



                    // https://exploratoria.github.io/exhibits/mechanics/elastic-collisions-in-3d/

                    var collisionNormal = distanceVector.normalized;

                    var massSum = firstCircleMass + secondCircleMass;

                    var firstCircleInfluence = firstCircleMass / massSum;
                    var secondCircleInfluence = secondCircleMass / massSum;

                    firstCircleData.position = halfCollisionDistance * secondCircleInfluence * collisionNormal + firstCirclePosition;
                    secondCircleData.position = halfCollisionDistance * firstCircleInfluence * -collisionNormal + secondCirclePosition;



                    var relativeVelocity = firstCircleVelocity - secondCircleVelocity;

                    // Dot product
                    // https://math.stackexchange.com/a/805962

                    var dot = Vector3.Dot(relativeVelocity, collisionNormal);

                    var relativeVelocityAlongCollisionNormal = dot * collisionNormal;

                    firstCircleData.velocity = firstCircleVelocity - 2.0f * secondCircleInfluence * relativeVelocityAlongCollisionNormal;
                    secondCircleData.velocity = secondCircleVelocity + 2.0f * firstCircleInfluence * relativeVelocityAlongCollisionNormal;

                    data[firstCircleIndex] = firstCircleData;
                    data[secondCircleIndex] = secondCircleData;

                    break;
                }
            }
        }

        // Resolve collisions with boundry
        for (int i = 0, c = data.Length; i < c; ++i)
        {
            var circleData = data[i];

            var radius = circleData.radius;

            var position = circleData.position;
            circleData.position = new Vector3
            {
                x = Mathf.Clamp(position.x, min.x + radius, max.x - radius),
                y = Mathf.Clamp(position.y, min.y + radius, max.y - radius),
                z = 0.0f
            };

            circleData.velocity = Vector3.Scale(circleData.velocity, new Vector3
            {
                x = position.x < min.x + radius || position.x > max.x - radius ? -1 : 1,
                y = position.y < min.y + radius || position.y > max.y - radius ? -1 : 1,
                z = position.z < min.z + radius || position.z > max.z - radius ? -1 : 1
            });

            data[i] = circleData;
        }

        // Assign new positions to GameObjects
        for (int i = 0, c = data.Length; i < c; ++i)
        {
            var circleData = data[i];
            circleData.transform.localPosition = circleData.position;
        }
    }

    static (Vector3 min, Vector3 max) GetWorldBouds()
    {
        var currentCamera = Camera.main;
        var cameraZ = currentCamera.transform.position.z;

        var min = currentCamera.ViewportToWorldPoint(new Vector3(0, 0, -cameraZ));
        var max = currentCamera.ViewportToWorldPoint(new Vector3(1, 1, -cameraZ));

        return (min, max);
    }
}
