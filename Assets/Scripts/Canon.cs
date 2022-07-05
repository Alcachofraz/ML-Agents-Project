using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    public AudioSource audioSource;
    bool spawnable = true;
    float cooldown = 0;
    public float rotationSpeed = 4.0f;
    List<GameObject> balls;
    public bool useMicrophone;
    public int rateInSeconds = 6;


    // Start is called before the first frame update
    void Start()
    {
        balls = new List<GameObject>();
        if (useMicrophone)
        {
            audioSource = GetComponent<AudioSource>();
            string deviceName;
            if (Microphone.devices.Length > 0)
            {
                deviceName = Microphone.devices[0];
                audioSource.clip = Microphone.Start(deviceName, true, 10, 1000);
                while (Microphone.GetPosition(deviceName) < 1 * 30) ;
                audioSource.loop = true;
                audioSource.Play();
            }
            else
            {
                Debug.Log("Please, connect your microphone");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate cannon:
        transform.Rotate(new Vector3(rotationSpeed * Time.deltaTime, 0f, 0f));

        // Check balls lifetime:
        DestroyBalls();

        if (useMicrophone) Mic();
        else Machine();
    }

    private void Machine()
    {
        // Update ball spawning cooldown:
        cooldown += Time.deltaTime;

        if (cooldown > rateInSeconds)
        {
            // Spawn ball:
            GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Rigidbody rb = ball.AddComponent<Rigidbody>();
            ball.AddComponent<Ball>();
            Vector3 direction = -transform.up;
            ball.transform.position = new Vector3(direction.x * transform.localScale.x, transform.position.y, direction.z * transform.localScale.x);
            ball.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            ball.name = "Ball";
            ball.tag = "Ball";

            // Use energy to determine velocity of the ball:
            float force = Random.Range(1.0f, 10.0f);
            rb.velocity = new Vector3(direction.x * force, 0.0f, direction.z * force);

            // Use energy to determine the color of the ball:
            float rgbValue = Map(0.0f, 255.0f, 1.0f, 20.0f, force);
            ball.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(rgbValue / 255.0f, rgbValue / 255.0f, 120.0f));

            balls.Add(ball);
            cooldown = 0;
        }
    }

    private void Mic()
    {
        // Update ball spawning cooldown:
        cooldown += Time.deltaTime;

        // Get microphone energy:
        float energy = AudioAnalysis.MeanEnergy(audioSource);
        float energyDB = AudioAnalysis.ConvertToDB(energy);
        if (energyDB > 30)
        {
            if (spawnable)
            {
                // Spawn ball:
                GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Rigidbody rb = ball.AddComponent<Rigidbody>();
                ball.AddComponent<Ball>();
                Vector3 direction = -transform.up;
                ball.transform.position = new Vector3(direction.x * transform.localScale.x, transform.position.y, direction.z * transform.localScale.x);
                ball.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                ball.name = "Ball";
                ball.tag = "Ball";

                // Use energy to determine velocity of the ball:
                float force = Map(0.0f, 10.0f, 30.0f, 50.0f, energyDB);
                rb.velocity = new Vector3(direction.x * force, 0.0f, direction.z * force);

                // Use energy to determine the color of the ball:
                float rgbValue = Map(0.0f, 255.0f, 30.0f, 50.0f, energyDB);
                ball.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(rgbValue / 255.0f, rgbValue / 255.0f, rgbValue / 255.0f));

                balls.Add(ball);
                spawnable = false;
                cooldown = 0;
            }
        }
        else if (cooldown > 1)
        {
            spawnable = true;
        }
    }

    void DestroyBalls()
    {
        List<GameObject> toRemove = new List<GameObject>();
        foreach (GameObject ball in balls)
        {
            if (ball.transform.position.y < -20.0f)
            {
                toRemove.Add(ball);
            }
        }
        foreach (GameObject ball in toRemove)
        {
            balls.Remove(ball);
            Destroy(ball);
        }
    }

    public void DestroyAllBalls()
    {
        foreach (GameObject ball in balls)
        {
            Destroy(ball);
        }
        balls.Clear();
    }

    float Map(float newMin, float newMax, float originalMin, float originalMax, float value)
    {
        return Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(originalMin, originalMax, value));
    }
}
