using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles audio playback for the museum experience.
/// Language selection logic is handled externally before calling methods here.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource narrationSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ambientSource;

    // Removed: Language enum and currentLanguage field

    [Header("Ambient Audio Settings")]
    [SerializeField] private float ambientCrossfadeDuration = 2.0f;
    [SerializeField] private float defaultAmbientVolume = 0.5f; // Kept for ambient fallback

    private Coroutine ambientCrossfadeCoroutine;

    private void Awake()
    {
        // Ensure audio sources exist (same as before)
        if (narrationSource == null)
        {
            narrationSource = gameObject.AddComponent<AudioSource>();
            narrationSource.playOnAwake = false;
            narrationSource.loop = false;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
        }
        if (ambientSource == null)
        {
            ambientSource = gameObject.AddComponent<AudioSource>();
            ambientSource.playOnAwake = false;
            ambientSource.loop = true;
            ambientSource.volume = defaultAmbientVolume; // Initialize volume
        }
    }

    /// <summary>
    /// Plays a narration audio clip. Assumes the correct localized clip is provided.
    /// </summary>
    /// <param name="narrationClip">The specific audio clip to play.</param>
    public void PlayNarration(AudioClip narrationClip) // Changed signature
    {
         if (narrationSource == null) {
             Debug.LogError("Narration AudioSource is not assigned in AudioManager!");
             return;
         }

        // Stop any currently playing narration
        if (narrationSource.isPlaying)
        {
            narrationSource.Stop();
        }

        // Removed internal language selection logic

        // Play the provided clip
        if (narrationClip != null)
        {
            narrationSource.clip = narrationClip;
            narrationSource.Play();
        }
        else
        {
             // Optionally do something if null clip is provided, like ensuring it's stopped.
             // narrationSource.clip = null; // Clear the clip?
             Debug.Log("PlayNarration called with a null clip.");
        }
    }

     /// <summary>
    /// Checks if the narration source is currently playing.
    /// </summary>
    public bool IsNarrationPlaying()
    {
        return narrationSource != null && narrationSource.isPlaying;
    }

    /// <summary>
    /// Stops the currently playing narration.
    /// </summary>
    public void StopNarration()
    {
        if (narrationSource != null && narrationSource.isPlaying)
        {
            narrationSource.Stop();
             narrationSource.clip = null; // Clear clip to prevent replaying on component enable/disable?
        }
    }

    /// <summary>
    /// Plays a sound effect.
    /// </summary>
    /// <param name="clip">The sound effect to play.</param>
    public void PlaySFX(AudioClip clip)
    {
         if (sfxSource == null) {
             Debug.LogError("SFX AudioSource is not assigned in AudioManager!");
             return;
         }
        if (clip != null)
        {
            // Use PlayOneShot for non-looping effects that shouldn't interrupt each other drastically
            sfxSource.PlayOneShot(clip);
        }
    }

     /// <summary>
    /// Checks if the ambient source is playing the specified clip.
    /// </summary>
    public bool IsAmbientClipPlaying(AudioClip clip)
    {
        return ambientSource != null && ambientSource.isPlaying && ambientSource.clip == clip;
    }


    // --- PlayAmbientAudio and other ambient methods remain largely the same ---
    // Make sure they check if ambientSource is null at the start.

    /// <summary>
    /// Plays ambient background audio with optional crossfade.
    /// </summary>
    /// <param name="clip">The ambient audio clip to play.</param>
    /// <param name="volume">Volume level (0.0 to 1.0).</param>
    /// <param name="loop">Whether to loop the ambient audio.</param>
    /// <param name="crossfade">Whether to crossfade from current ambient audio.</param>
    public void PlayAmbientAudio(AudioClip clip, float volume = 0.5f, bool loop = true, bool crossfade = true)
    {
         if (ambientSource == null) {
             Debug.LogError("Ambient AudioSource is not assigned in AudioManager!");
             return;
         }
        if (clip == null) {
            // If null clip provided, treat it as a request to stop ambient sound
             StopAmbientAudio(crossfade);
            return;
        }


        // If the same clip is already playing, just adjust volume if needed
        if (ambientSource.clip == clip && ambientSource.isPlaying)
        {
            if (!Mathf.Approximately(ambientSource.volume, volume))
            {
                 SetAmbientVolume(volume, crossfade); // Use the existing method to handle crossfade
            }
            // Ensure loop status is correct if it wasn't before
            if (ambientSource.loop != loop) {
                 ambientSource.loop = loop;
            }
            return; // Don't restart the same clip
        }

        // If we're switching to a new clip
        if (crossfade && ambientSource.isPlaying && ambientSource.clip != null) // Ensure crossfade only happens if something was playing
        {
            if (ambientCrossfadeCoroutine != null)
                StopCoroutine(ambientCrossfadeCoroutine);

            ambientCrossfadeCoroutine = StartCoroutine(CrossfadeAmbientAudio(clip, volume, loop));
        }
        else
        {
            // No crossfade, just play the new clip
            ambientSource.Stop();
            ambientSource.clip = clip;
            ambientSource.volume = volume;
            ambientSource.loop = loop;
            ambientSource.Play();
        }
    }

    /// <summary>
    /// Adjusts the volume of the ambient audio with an optional crossfade.
    /// </summary>
    /// <param name="volume">Target volume level (0.0 to 1.0).</param>
    /// <param name="crossfade">Whether to crossfade to the new volume.</param>
    public void SetAmbientVolume(float volume, bool crossfade = true)
    {
        if (ambientSource == null) return;

        if (crossfade && ambientSource.isPlaying) // Only crossfade volume if already playing
        {
            StartAmbientCrossfade(ambientSource.volume, volume);
        }
        else
        {
            ambientSource.volume = volume;
        }
    }

    /// <summary>
    /// Starts a volume crossfade for the ambient audio.
    /// </summary>
    private void StartAmbientCrossfade(float startVolume, float targetVolume)
    {
        if (ambientCrossfadeCoroutine != null)
            StopCoroutine(ambientCrossfadeCoroutine);

        ambientCrossfadeCoroutine = StartCoroutine(CrossfadeAmbientVolume(startVolume, targetVolume));
    }


    /// <summary>
    /// Crossfades between the current ambient audio and a new one.
    /// </summary>
    private IEnumerator CrossfadeAmbientAudio(AudioClip newClip, float targetVolume, bool loop)
    {
        // Create temporary audio source for crossfade
        AudioSource tempSource = gameObject.AddComponent<AudioSource>();
        tempSource.clip = newClip;
        tempSource.loop = loop;
        tempSource.volume = 0f;
        tempSource.outputAudioMixerGroup = ambientSource.outputAudioMixerGroup; // Match output
        tempSource.priority = ambientSource.priority; // Match priority
        tempSource.Play();

        float currentVolume = ambientSource.volume;
        float time = 0f;

        while (time < ambientCrossfadeDuration)
        {
            time += Time.deltaTime;
            // Avoid division by zero if duration is instant
            float t = (ambientCrossfadeDuration > 0) ? Mathf.Clamp01(time / ambientCrossfadeDuration) : 1f;

            // Fade out current ambient
            ambientSource.volume = Mathf.Lerp(currentVolume, 0f, t);

            // Fade in new ambient
            tempSource.volume = Mathf.Lerp(0f, targetVolume, t);

            yield return null;
        }

        // Stop the old source completely
        ambientSource.Stop();
        ambientSource.clip = null; // Clear the old clip

        // Swap the sources (transfer state from temp to main)
        ambientSource.clip = tempSource.clip;
        ambientSource.volume = tempSource.volume;
        ambientSource.loop = tempSource.loop;
        ambientSource.time = tempSource.time; // Preserve playback position
        if (!ambientSource.isPlaying) // Ensure it's playing if it finished fading in
             ambientSource.Play();


        // Clean up temporary source
        Destroy(tempSource);

        ambientCrossfadeCoroutine = null;
    }

    /// <summary>
    /// Crossfades the volume of the current ambient audio.
    /// </summary>
    private IEnumerator CrossfadeAmbientVolume(float startVolume, float targetVolume)
    {
         if (ambientSource == null) yield break;

        float time = 0f;

        while (time < ambientCrossfadeDuration)
        {
            time += Time.deltaTime;
            float t = (ambientCrossfadeDuration > 0) ? Mathf.Clamp01(time / ambientCrossfadeDuration) : 1f;


            ambientSource.volume = Mathf.Lerp(startVolume, targetVolume, t);

            yield return null;
        }

        ambientSource.volume = targetVolume; // Ensure target volume is set
        ambientCrossfadeCoroutine = null;
    }

    /// <summary>
    /// Stops the ambient audio with optional fade out.
    /// </summary>
    /// <param name="fadeOut">Whether to fade out or stop immediately.</param>
    public void StopAmbientAudio(bool fadeOut = true)
    {
         if (ambientSource == null) return;

        if (fadeOut && ambientSource.isPlaying && ambientSource.volume > 0.01f) // Only fade if playing and audible
        {
            StartAmbientCrossfade(ambientSource.volume, 0f);
            // Optional: Start a coroutine to actually stop the source after fade if needed,
            // but often just setting volume to 0 is enough if the clip might be restarted later.
            // StartCoroutine(StopAmbientAfterFade());
        }
        else
        {
             // Ensure any active fade is stopped before stopping immediately
             if (ambientCrossfadeCoroutine != null) {
                 StopCoroutine(ambientCrossfadeCoroutine);
                 ambientCrossfadeCoroutine = null;
             }
            ambientSource.Stop();
             ambientSource.clip = null; // Clear clip
             ambientSource.volume = defaultAmbientVolume; // Reset volume for next time? Or keep at 0?
        }
    }

    // Removed: StopAmbientAfterFade (handled by setting volume to 0 in fade)

    /// <summary>
    /// Stops all audio playback immediately.
    /// </summary>
    public void StopAllAudio()
    {
        StopNarration(); // Use the dedicated stop method
        if(sfxSource != null) sfxSource.Stop(); // Stop looping SFX if any were started with Play() instead of PlayOneShot
        StopAmbientAudio(false); // Stop ambient immediately

        // No need to manually stop coroutine here, StopAmbientAudio(false) handles it.
    }

    // Removed: SetLanguage and ToggleLanguage methods
}