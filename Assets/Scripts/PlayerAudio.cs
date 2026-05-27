using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource footstepsSource;
    public AudioSource effectsSource;

    [Header("Audio Clips")]
    public AudioClip[] footstepSounds;

    public AudioClip jumpSound;
    public AudioClip flashlightSound;
    public AudioClip interactSound;

    [Header("Footstep Settings")]
    public float stepDelay = 0.35f;

    private float stepTimer;

    void Update()
    {
        HandleFootsteps();
    }

    void HandleFootsteps()
    {
        bool isMoving =
            Keyboard.current.wKey.isPressed ||
            Keyboard.current.aKey.isPressed ||
            Keyboard.current.sKey.isPressed ||
            Keyboard.current.dKey.isPressed;

        if (isMoving)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                PlayFootstep();

                stepTimer = stepDelay;
            }
        }
        else
        {
            stepTimer = 0;
        }
    }

    void PlayFootstep()
    {
        if (footstepSounds.Length == 0) return;

        int randomIndex = Random.Range(0, footstepSounds.Length);

        footstepsSource.pitch = Random.Range(0.95f, 1.05f);

        footstepsSource.PlayOneShot(footstepSounds[randomIndex]);
    }

    // PULO
    public void PlayJump()
    {
        effectsSource.PlayOneShot(jumpSound);
    }

    // LANTERNA
    public void PlayFlashlight()
    {
        effectsSource.PlayOneShot(flashlightSound);
    }

    // INTERAÇÃO
    public void PlayInteract()
    {
        effectsSource.PlayOneShot(interactSound);
    }
}