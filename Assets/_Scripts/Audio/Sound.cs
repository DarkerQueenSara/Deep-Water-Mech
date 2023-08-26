using UnityEngine;

namespace Audio
{
    /// <summary>
    /// A serializable class that represents a sound in the inspector
    /// </summary>
    [System.Serializable]
    public class Sound
    {
        /// <summary>
        /// The name of the sound
        /// </summary>
        public string name;
        /// <summary>
        /// The audio clip
        /// </summary>
        public AudioClip clip;

        /// <summary>
        /// The volume
        /// </summary>
        public float volume = 0.2f;
        /// <summary>
        /// The pitch
        /// </summary>
        public float pitch = 1.0f;
        /// <summary>
        /// If the playback is supposed to loop or not
        /// </summary>
        public bool loop;

        /// <summary>
        /// The Unity AudioSource (generated via code in the AudioManager)
        /// </summary>
        [HideInInspector]
        public AudioSource source;

        /// <summary>
        /// Sets the source.
        /// </summary>
        /// <param name="audioSource">The audio source.</param>
        public void SetSource(AudioSource audioSource)
        {
            audioSource.clip = clip;

            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.loop = loop;
            this.source = audioSource;
        }

        /// <summary>
        /// Plays the audio clip.
        /// </summary>
        public void Play()
        {
            source.Play();
        }

        /// <summary>
        /// Plays the audio clip after a time interval.
        /// </summary>
        /// <param name="time">The time interval.</param>
        public void PlayScheduled(double time)
        {
            source.PlayScheduled(time);
        }

        /// <summary>
        /// Stops the audio clip.
        /// </summary>
        public void Stop()
        {
            source.Stop();
        }

        /// <summary>
        /// Determines whether this sound clip is playing.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this sound clip is playing; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPlaying()
        {
            return source.isPlaying;
        }
    }
}

