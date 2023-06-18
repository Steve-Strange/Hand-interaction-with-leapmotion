using System.Collections.Generic;
using System.Linq;
using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Devices.Data;
using Neurorehab.Scripts.Enums;
using Neurorehab.Scripts.Udp;
using UnityEngine;

namespace Neurorehab.Device_LeapMotion.Scripts
{   
    /// <summary>
    /// The controller of all the <see cref="LeapMotionData"/>. Responsible for creating, deleting and updating all the <see cref="LeapMotionData"/> according to what is receiving by UDP.
    /// </summary>
    public class LeapMotion : GenericDeviceController
    {
        [Header("Avatar Info")]
        public List<Material> HandsMaterials;

        protected override void Awake()
        {
            base.Awake();
            DeviceName = Devices.leapmotion.ToString();
        }

        /// <summary>
        /// First it creates a new <see cref="LeapMotionData"/> according to the <see cref="Neurorehab.Scripts.CpDebugger.Udp.GenericDevice"/> received as a parameter. Then, it instantiates a Unity object according to the hand side for each new detection for devices of this type.
        /// </summary>
        /// <param name="genericDevice">The device being checked.</param>
        protected override void CreateNewUnityObject(GenericDevice genericDevice)
        {
            foreach (var values in genericDevice.GetNewDetections(DevicesData.Keys.ToList()))
            {
                var genericDeviceData = (LeapMotionData) CreateGenericDeviceData(genericDevice.DeviceName, values);
                InstantiateUnityObject(genericDeviceData, genericDeviceData.LeftHanded ? 0 : 1);

                AddDeviceDataToList(values.Id, genericDeviceData);
            }
        }
    }
}