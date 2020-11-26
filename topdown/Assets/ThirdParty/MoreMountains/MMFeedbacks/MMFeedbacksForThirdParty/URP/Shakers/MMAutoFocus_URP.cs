﻿using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// This class will set the URP depth of field to focus on the set of targets specified in its inspector.
    /// </summary>
    [RequireComponent(typeof(Volume))]
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMAutoFocus_URP")]
    public class MMAutoFocus_URP : MonoBehaviour
    {
        [Header("Bindings")]
        /// the position of the camera
        public Transform CameraTransform;
        /// a list of all possible targets
        public Transform[] FocusTargets;
        [Header("Setup")]
        /// the current target of this auto focus
        public float FocusTargetID;
        [Header("Desired Aperture")]
        /// the aperture to work with
        [Range(0.1f, 20f)]
        public float Aperture = 0.1f;

        protected Volume _volume;
        protected VolumeProfile _profile;
        protected DepthOfField _depthOfField;

        /// <summary>
        /// On Start, stores volume, profile and DoF
        /// </summary>
        void Start()
        {
            _volume = GetComponent<Volume>();
            _profile = _volume.profile;
            _profile.TryGet<DepthOfField>(out _depthOfField);
        }

        /// <summary>
        /// On update we set our focus distance and aperture
        /// </summary>
        void Update()
        {
            float distance = Vector3.Distance(CameraTransform.position, FocusTargets[Mathf.FloorToInt(FocusTargetID)].position);
            _depthOfField.focusDistance.Override(distance);
            _depthOfField.aperture.Override(Aperture);
        }
    }
}
