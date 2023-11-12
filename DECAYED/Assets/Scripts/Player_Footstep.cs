using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Footstep : MonoBehaviour
{
    public AudioSource AudioSource;

    public AudioClip concrete;
    public AudioClip concrete_land;

    public AudioClip metal;
    public AudioClip metal_land;

    public AudioClip grass;
    public AudioClip grass_land;

    public AudioClip dirt;
    public AudioClip dirt_land;

    public AudioClip wood;
    public AudioClip wood_land;

    public AudioClip water;
    public AudioClip water_land;

    RaycastHit hit;
    public Transform rayStart;
    public float range;
    public LayerMask layerMask;

    public bool isMoving = false;
    public bool isCrouch = false;
    public bool isSprint = false;
    public bool isPause = false;
    public bool isJump = false;
    public bool isLand = false;

    private void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        AudioSource.clip = grass;
    }

    private void Update()
    {
        isMoving = GameObject.Find("Char").GetComponent<Player_Move>().isMoving;
        isCrouch = GameObject.Find("Char").GetComponent<Player_Move>().crouched;
        isSprint = GameObject.Find("Char").GetComponent<Player_Move>().isSprint;
        isPause = GameObject.Find("Char").GetComponent<Player_Move>().isPause;
        isJump = GameObject.Find("Char").GetComponent<Player_Move>().isJump;
        isLand = GameObject.Find("Char").GetComponent<Player_Move>().isLand;

        if (!isPause)
        {
            FootStep();
        }
    }

    public void FootStep()
    {
        if (Physics.Raycast(rayStart.position, rayStart.transform.up * -1, out hit, range, layerMask))
        {
            if (isMoving)
            {
                if (hit.collider.CompareTag("concrete"))
                {
                    if (isSprint)
                    {
                        PlayFootstepSound(concrete, 0.6f);
                    }
                    else if (isCrouch)
                    {
                        PlayFootstepSound(concrete, 0.2f);
                    }
                    else
                    {
                        PlayFootstepSound(concrete, 0.35f);
                    }
                }
                if (hit.collider.CompareTag("Land"))
                {
                    if (isSprint)
                    {
                        PlayFootstepSound(grass, 0.6f);
                    }
                    else if (isCrouch)
                    {
                        PlayFootstepSound(grass, 0.2f);
                    }
                    else
                    {
                        PlayFootstepSound(grass, 0.35f);
                    }
                }
                if (hit.collider.CompareTag("metal"))
                {
                    if (isSprint)
                    {
                        PlayFootstepSound(metal, 0.6f);
                    }
                    else if (isCrouch)
                    {
                        PlayFootstepSound(metal, 0.2f);
                    }
                    else
                    {
                        PlayFootstepSound(metal, 0.35f);
                    }
                }
            }
        }
    }

    public void LandStep()
    {
        if (Physics.Raycast(rayStart.position, rayStart.transform.up * -1, out hit, range, layerMask))
        {
            if (hit.collider.CompareTag("concrete"))
            {
                if (isSprint)
                {
                    PlayLandstepSound(concrete_land, 0.6f);
                }
                else if (isCrouch)
                {
                    PlayLandstepSound(concrete_land, 0.2f);
                }
                else
                {
                    PlayLandstepSound(concrete_land, 0.35f);
                }
            }
            if (hit.collider.CompareTag("Land"))
            {
                if (isSprint)
                {
                    PlayLandstepSound(grass_land, 0.6f);
                }
                else if (isCrouch)
                {
                    PlayLandstepSound(grass_land, 0.2f);
                }
                else
                {
                    PlayLandstepSound(grass_land, 0.35f);
                }
            }
            if (hit.collider.CompareTag("metal"))
            {
                if (isSprint)
                {
                    PlayLandstepSound(metal_land, 0.6f);
                }
                else if (isCrouch)
                {
                    PlayLandstepSound(metal_land, 0.2f);
                }
                else
                {
                    PlayLandstepSound(metal_land, 0.35f);
                }
            }
        }
    }
    void PlayLandstepSound(AudioClip audio, float volume)
    {
        if (isSprint)
        {
            AudioSource.clip = audio;
            AudioSource.volume = volume;
            AudioSource.pitch = 1f;
            AudioSource.PlayOneShot(audio);
        }
        else if (isCrouch)
        {
            AudioSource.clip = audio;
            AudioSource.volume = volume;
            AudioSource.pitch = 1f;
            AudioSource.PlayOneShot(audio);
        }
        else
        {
            AudioSource.clip = audio;
            AudioSource.volume = volume;
            AudioSource.pitch = 1f;
            AudioSource.PlayOneShot(audio);
        }
    }
    void PlayFootstepSound(AudioClip audio, float volume)
    {
        if (!AudioSource.isPlaying || AudioSource.clip != audio)
        {
            AudioSource.Stop(); //이전에 재생 중이던 소리를 중지

            if (isSprint)
            {
                AudioSource.clip = audio;
                AudioSource.volume = volume;
                AudioSource.pitch = Random.Range(1f, 1.2f);
                AudioSource.Play();
            }
            else if (isCrouch)
            {
                AudioSource.clip = audio;
                AudioSource.volume = volume;
                AudioSource.pitch = Random.Range(0.35f, 0.5f);
                AudioSource.Play();
            }
            else
            {
                AudioSource.clip = audio;
                AudioSource.volume = volume;
                AudioSource.pitch = Random.Range(0.45f, 0.7f);
                AudioSource.Play();
            }
        }
    }
}
