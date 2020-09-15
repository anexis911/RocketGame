using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] AudioClip mainEngine = null;
    [SerializeField] AudioClip deathSound = null;
    [SerializeField] AudioClip winSound = null;
    [SerializeField] ParticleSystem enginePArticles = null;
    [SerializeField] ParticleSystem deathParticles = null;
    [SerializeField] ParticleSystem winParticles = null;


    Rigidbody rigidbody;
    AudioSource audioSource;

    bool collisionsAreEnables = true;

    enum State { Alive, Dying, Transending }
    State state = State.Alive;
    int currentScene;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        currentScene = SceneManager.GetActiveScene().buildIndex;
        //rigidbody.mass = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            ProcessInput();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKey();
        }
        

    }

    private void RespondToDebugKey()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsAreEnables = !collisionsAreEnables;
        }
    }

    private void ProcessInput()
    {
        Thrust();
        Rotate();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !collisionsAreEnables) { return; }
        
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                
                break;
            case "Finish":
                state = State.Transending;
                audioSource.Stop();
                winParticles.Play();
                audioSource.PlayOneShot(winSound);
                Invoke("LoadNextScene", 1f);
                break;
            default:
                state = State.Dying;
                audioSource.Stop();
                deathParticles.Play();
                audioSource.PlayOneShot(deathSound);
                Invoke("LoadFirstScene", 1f);
                break;
        }
    }

    private void LoadNextScene()
    {
        int nextSceneIndex = currentScene + 1;
        
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }
    private void LoadFirstScene()
    {
        SceneManager.LoadScene(currentScene);
    }

    private void Rotate()
    {
        rigidbody.freezeRotation = true;
        
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * Time.deltaTime *rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * Time.deltaTime * rotationSpeed);
        }
        rigidbody.freezeRotation = false;
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidbody.AddRelativeForce(Vector3.up  *rcsThrust * Time.deltaTime);
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(mainEngine);
            }
            enginePArticles.Play();

        }
        else
        {
            audioSource.Stop();
            enginePArticles.Stop();
        }
    }
}
