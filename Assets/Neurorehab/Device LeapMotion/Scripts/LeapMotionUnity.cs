using System;
using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Enums;
using Neurorehab.Scripts.Utilities;
using UnityEngine;

namespace Neurorehab.Device_LeapMotion.Scripts
{
    /// <summary>
    /// Responsible for updating the Unity Leapmotion data according to its <see cref="LeapMotionData"/>.
    /// </summary>
    public class LeapMotionUnity : GenericDeviceUnity
    {
        /// <summary>
        /// All the hand joints initial rotations
        /// </summary>
        private Quaternion[] _initialRotations;

        /// <summary>
        /// Additional rotation to add to the bones
        /// </summary>
        [Tooltip("Additional rotation to add to the bones")]
        public float AdditionalRotation = 0;

        /// <summary>
        /// True if you want to update the object position according to what is being received
        /// </summary>
        [Tooltip("True if you want to update the object position according to what is being received")]
        public bool UpdatePosition = true;

        /// <summary>
        /// True if you want to update the object rotation according to what is being received
        /// </summary>
        [Tooltip("True if you want to update the object rotation according to what is being received")]
        public bool UpdateRotation = true;

        [Header("Hand Skeleton")]
        public GameObject HandGameobject;
        public GameObject Arm;
        public GameObject Hand;
        public GameObject IndexFingerBone1;
        public GameObject IndexFingerBone2;
        public GameObject IndexFingerBone3;
        public GameObject MiddleFingerBone1;
        public GameObject MiddleFingerBone2;
        public GameObject MiddleFingerBone3;
        public GameObject PinkyFingerBone1;
        public GameObject PinkyFingerBone2;
        public GameObject PinkyFingerBone3;
        public GameObject RingFingerBone1;
        public GameObject RingFingerBone2;
        public GameObject RingFingerBone3;
        public GameObject ThumbFingerBone1;
        public GameObject ThumbFingerBone2;
        public GameObject ThumbFingerBone3;

        /// <summary>
        /// Resets the <see cref="HandGameobject"/> initial rotation to (0, 0, 0)
        /// </summary>
        [Tooltip("Resets the HandObject initial rotation to (0, 0, 0)")]
        public bool ResetHandInitialRotation;

        /// <summary>
        /// Initializes all the transforms and <see cref="_initialRotations"/>
        /// </summary>
        private void Awake()
        {
            transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            transform.localScale = Vector3.one / 2;

            var jointCount = Enum.GetNames(typeof(LeapMotionBones)).Length;
            var transforms = new Transform[jointCount];
            _initialRotations = new Quaternion[jointCount];

            transforms[(int) LeapMotionBones.thumb_bone1] =
                ThumbFingerBone1 != null ? ThumbFingerBone1.transform : null;
            transforms[(int) LeapMotionBones.thumb_bone2] =
                ThumbFingerBone2 != null ? ThumbFingerBone2.transform : null;
            transforms[(int) LeapMotionBones.thumb_bone3] =
                ThumbFingerBone3 != null ? ThumbFingerBone3.transform : null;
            transforms[(int) LeapMotionBones.index_bone1] =
                IndexFingerBone1 != null ? IndexFingerBone1.transform : null;
            transforms[(int) LeapMotionBones.index_bone2] =
                IndexFingerBone2 != null ? IndexFingerBone2.transform : null;
            transforms[(int) LeapMotionBones.index_bone3] =
                IndexFingerBone3 != null ? IndexFingerBone3.transform : null;
            transforms[(int) LeapMotionBones.middle_bone1] =
                MiddleFingerBone1 != null ? MiddleFingerBone1.transform : null;
            transforms[(int) LeapMotionBones.middle_bone2] =
                MiddleFingerBone2 != null ? MiddleFingerBone2.transform : null;
            transforms[(int) LeapMotionBones.middle_bone3] =
                MiddleFingerBone3 != null ? MiddleFingerBone3.transform : null;
            transforms[(int) LeapMotionBones.ring_bone1] = 
                RingFingerBone1 != null ? RingFingerBone1.transform : null;
            transforms[(int) LeapMotionBones.ring_bone2] = 
                RingFingerBone2 != null ? RingFingerBone2.transform : null;
            transforms[(int) LeapMotionBones.ring_bone3] = 
                RingFingerBone3 != null ? RingFingerBone3.transform : null;
            transforms[(int) LeapMotionBones.pinky_bone1] =
                PinkyFingerBone1 != null ? PinkyFingerBone1.transform : null;
            transforms[(int) LeapMotionBones.pinky_bone2] =
                PinkyFingerBone2 != null ? PinkyFingerBone2.transform : null;
            transforms[(int) LeapMotionBones.pinky_bone3] =
                PinkyFingerBone3 != null ? PinkyFingerBone3.transform : null;
            transforms[(int) LeapMotionBones.forearm] = Arm != null ? Arm.transform : null;
            transforms[(int) LeapMotionBones.palm] = Hand != null ? Hand.transform : null;
            
            if (HandGameobject.IsNull()) return;

            foreach (LeapMotionBones bone in Enum.GetValues(typeof(LeapMotionBones)))
                if (transforms[(int) bone] != null)
                    _initialRotations[(int) bone] = transforms[(int) bone].rotation;

            if(ResetHandInitialRotation)
                HandGameobject.transform.rotation = Quaternion.Euler(Vector3.zero);

            HandGameobject.transform.rotation = Quaternion.identity;
        }


        // Update is called once per frame
        private void Update()
        {
            if (GenericDeviceData == null) return;

            if (UpdatePosition)
                UpdateHandPosition();
            if (UpdateRotation)
                UpdateHandRotation();
        }

        /// <summary>
        /// Updates the hand joints position values according to its <see cref="GenericDeviceUnity.GenericDeviceData"/> 
        /// </summary>
        private void UpdateHandPosition()
        {
            HandGameobject.transform.localPosition = GenericDeviceData.GetPosition(LeapMotionBones.forearm.ToString());
        }

        /// <summary>
        /// Updates the hand joints rotation values according to its <see cref="GenericDeviceUnity.GenericDeviceData"/> 
        /// </summary>
        public void UpdateHandRotation()
        {
            if (Arm != null)
                Arm.transform.rotation =
                    HandGameobject.transform.rotation * //reference object gameobject (inthis case the hand)
                    GenericDeviceData.GetRotation(LeapMotionBones.forearm.ToString()) * //rotation that comes from leapmotion (world rotation)
                    _initialRotations[(int) LeapMotionBones.forearm]; //initial rotation of the forearm inside unity

            if (Hand != null)
                Hand.transform.rotation =
                    HandGameobject.transform.rotation *
                    GenericDeviceData.GetRotation(LeapMotionBones.palm.ToString()) *
                    _initialRotations[(int) LeapMotionBones.palm];

            if (ThumbFingerBone1 != null)
                ThumbFingerBone1.transform.rotation =
                    HandGameobject.transform.rotation *
                    GenericDeviceData.GetRotation(LeapMotionBones.thumb_bone1.ToString()) *
                    _initialRotations[(int) LeapMotionBones.thumb_bone1];

            if (ThumbFingerBone2 != null)
                ThumbFingerBone2.transform.rotation =
                    HandGameobject.transform.rotation *
                    GenericDeviceData.GetRotation(LeapMotionBones.thumb_bone2.ToString()) *
                    _initialRotations[(int) LeapMotionBones.thumb_bone2];

            if (ThumbFingerBone3 != null)
                ThumbFingerBone3.transform.rotation =
                    HandGameobject.transform.rotation *
                    GenericDeviceData.GetRotation(LeapMotionBones.thumb_bone3.ToString()) *
                    _initialRotations[(int) LeapMotionBones.thumb_bone3];

            if (IndexFingerBone1 != null)
                IndexFingerBone1.transform.rotation =
                    HandGameobject.transform.rotation *
                    GenericDeviceData.GetRotation(LeapMotionBones.index_bone1.ToString()) *
                    _initialRotations[(int) LeapMotionBones.index_bone1];

            if (IndexFingerBone2 != null)
                IndexFingerBone2.transform.rotation =
                    HandGameobject.transform.rotation *
                    GenericDeviceData.GetRotation(LeapMotionBones.index_bone2.ToString()) *
                    _initialRotations[(int) LeapMotionBones.index_bone2];

            if (IndexFingerBone3 != null)
                IndexFingerBone3.transform.rotation =
                    HandGameobject.transform.rotation *
                    GenericDeviceData.GetRotation(LeapMotionBones.index_bone3.ToString()) *
                    _initialRotations[(int) LeapMotionBones.index_bone3];

            if (MiddleFingerBone1 != null)
                MiddleFingerBone1.transform.rotation =
                    HandGameobject.transform.rotation *
                    GenericDeviceData.GetRotation(LeapMotionBones.middle_bone1.ToString()) *
                    _initialRotations[(int) LeapMotionBones.middle_bone1];

            if (MiddleFingerBone2 != null)
                MiddleFingerBone2.transform.rotation =
                    HandGameobject.transform.rotation *
                    GenericDeviceData.GetRotation(LeapMotionBones.middle_bone2.ToString()) *
                    _initialRotations[(int) LeapMotionBones.middle_bone2];

            if (MiddleFingerBone3 != null)
                MiddleFingerBone3.transform.rotation =
                    HandGameobject.transform.rotation *
                    GenericDeviceData.GetRotation(LeapMotionBones.middle_bone3.ToString()) *
                    _initialRotations[(int) LeapMotionBones.middle_bone3];

            if (PinkyFingerBone1 != null)
                PinkyFingerBone1.transform.rotation =
                    HandGameobject.transform.rotation *
                    GenericDeviceData.GetRotation(LeapMotionBones.pinky_bone1.ToString()) *
                    _initialRotations[(int) LeapMotionBones.pinky_bone1];

            if (PinkyFingerBone2 != null)
                PinkyFingerBone2.transform.rotation =
                    HandGameobject.transform.rotation *
                    GenericDeviceData.GetRotation(LeapMotionBones.pinky_bone2.ToString()) *
                    _initialRotations[(int) LeapMotionBones.pinky_bone2];

            if (PinkyFingerBone3 != null)
                PinkyFingerBone3.transform.rotation =
                    HandGameobject.transform.rotation *
                    GenericDeviceData.GetRotation(LeapMotionBones.pinky_bone3.ToString()) *
                    _initialRotations[(int) LeapMotionBones.pinky_bone3];

            if (RingFingerBone1 != null)
                RingFingerBone1.transform.rotation =
                    HandGameobject.transform.rotation *
                    GenericDeviceData.GetRotation(LeapMotionBones.ring_bone1.ToString()) *
                    _initialRotations[(int) LeapMotionBones.ring_bone1];

            if (RingFingerBone2 != null)
                RingFingerBone2.transform.rotation =
                    HandGameobject.transform.rotation *
                    GenericDeviceData.GetRotation(LeapMotionBones.ring_bone2.ToString()) *
                    _initialRotations[(int) LeapMotionBones.ring_bone2];

            if (RingFingerBone3 != null)
                RingFingerBone3.transform.rotation =
                    HandGameobject.transform.rotation *
                    GenericDeviceData.GetRotation(LeapMotionBones.ring_bone3.ToString()) *
                    _initialRotations[(int) LeapMotionBones.ring_bone3];
        }
    }
}