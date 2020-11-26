﻿using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// Add this to an audio distortion low pass to shake its values remapped along a curve 
    /// </summary>
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/Audio/MMAudioFilterLowPassShaker")]
    [RequireComponent(typeof(AudioLowPassFilter))]
    public class MMAudioFilterLowPassShaker : MMShaker
    {
        [Header("Low Pass")]
        /// whether or not to add to the initial value
        public bool RelativeLowPass = false;
        /// the curve used to animate the intensity value on
        public AnimationCurve ShakeLowPass = new AnimationCurve(new Keyframe(0, 1f), new Keyframe(0.5f, 0f), new Keyframe(1, 1f));
        /// the value to remap the curve's 0 to
        [Range(10f, 22000f)]
        public float RemapLowPassZero = 0f;
        /// the value to remap the curve's 1 to
        [Range(10f, 22000f)]
        public float RemapLowPassOne = 10000f;

        /// the audio source to pilot
        protected AudioLowPassFilter _targetAudioLowPassFilter;
        protected float _initialLowPass;
        protected float _originalShakeDuration;
        protected bool _originalRelativeLowPass;
        protected AnimationCurve _originalShakeLowPass;
        protected float _originalRemapLowPassZero;
        protected float _originalRemapLowPassOne;

        /// <summary>
        /// On init we initialize our values
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _targetAudioLowPassFilter = this.gameObject.GetComponent<AudioLowPassFilter>();
        }

        /// <summary>
        /// When that shaker gets added, we initialize its shake duration
        /// </summary>
        protected virtual void Reset()
        {
            ShakeDuration = 2f;
        }

        /// <summary>
        /// Shakes values over time
        /// </summary>
        protected override void Shake()
        {
            float newLowPassLevel = ShakeFloat(ShakeLowPass, RemapLowPassZero, RemapLowPassOne, RelativeLowPass, _initialLowPass);
            _targetAudioLowPassFilter.cutoffFrequency = newLowPassLevel;
        }

        /// <summary>
        /// Collects initial values on the target
        /// </summary>
        protected override void GrabInitialValues()
        {
            _initialLowPass = _targetAudioLowPassFilter.cutoffFrequency;
        }

        /// <summary>
        /// When we get the appropriate event, we trigger a shake
        /// </summary>
        /// <param name="lowPassCurve"></param>
        /// <param name="duration"></param>
        /// <param name="amplitude"></param>
        /// <param name="relativeLowPass"></param>
        /// <param name="attenuation"></param>
        /// <param name="channel"></param>
        public virtual void OnMMAudioFilterLowPassShakeEvent(AnimationCurve lowPassCurve, float duration, float remapMin, float remapMax, bool relativeLowPass = false,
            float attenuation = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true)
        {
            if (!CheckEventAllowed(channel) || Shaking)
            {
                return;
            }

            _resetShakerValuesAfterShake = resetShakerValuesAfterShake;
            _resetTargetValuesAfterShake = resetTargetValuesAfterShake;

            if (resetShakerValuesAfterShake)
            {
                _originalShakeDuration = ShakeDuration;
                _originalShakeLowPass = ShakeLowPass;
                _originalRemapLowPassZero = RemapLowPassZero;
                _originalRemapLowPassOne = RemapLowPassOne;
                _originalRelativeLowPass = RelativeLowPass;
            }

            ShakeDuration = duration;
            ShakeLowPass = lowPassCurve;
            RemapLowPassZero = remapMin * attenuation;
            RemapLowPassOne = remapMax * attenuation;
            RelativeLowPass = relativeLowPass;

            Play();
        }

        /// <summary>
        /// Resets the target's values
        /// </summary>
        protected override void ResetTargetValues()
        {
            base.ResetTargetValues();
            _targetAudioLowPassFilter.cutoffFrequency = _initialLowPass;
        }

        /// <summary>
        /// Resets the shaker's values
        /// </summary>
        protected override void ResetShakerValues()
        {
            base.ResetShakerValues();
            ShakeDuration = _originalShakeDuration;
            ShakeLowPass = _originalShakeLowPass;
            RemapLowPassZero = _originalRemapLowPassZero;
            RemapLowPassOne = _originalRemapLowPassOne;
            RelativeLowPass = _originalRelativeLowPass;
        }

        /// <summary>
        /// Starts listening for events
        /// </summary>
        public override void StartListening()
        {
            base.StartListening();
            MMAudioFilterLowPassShakeEvent.Register(OnMMAudioFilterLowPassShakeEvent);
        }

        /// <summary>
        /// Stops listening for events
        /// </summary>
        public override void StopListening()
        {
            base.StopListening();
            MMAudioFilterLowPassShakeEvent.Unregister(OnMMAudioFilterLowPassShakeEvent);
        }
    }

    /// <summary>
    /// An event used to trigger vignette shakes
    /// </summary>
    public struct MMAudioFilterLowPassShakeEvent
    {
        public delegate void Delegate(AnimationCurve lowPassCurve, float duration, float remapMin, float remapMax, bool relativeLowPass = false,
            float attenuation = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(AnimationCurve lowPassCurve, float duration, float remapMin, float remapMax, bool relativeLowPass = false,
            float attenuation = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true)
        {
            OnEvent?.Invoke(lowPassCurve, duration, remapMin, remapMax, relativeLowPass,
                attenuation, channel, resetShakerValuesAfterShake, resetTargetValuesAfterShake);
        }
    }
}
