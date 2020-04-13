﻿// Copyright 2017-2019 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.Threading;
using System.Threading.Tasks;
using UnityCommon;
using UnityEngine;

namespace Naninovel.Commands
{
    /// <summary>
    /// Activates/disables camera look mode, when player can offset the main camera with input devices 
    /// (eg, by moving a mouse or using gamepad analog stick).
    /// Check [this video](https://youtu.be/rC6C9mA7Szw) for a quick demonstration of the command.
    /// </summary>
    /// <example>
    /// ; Activate camera look mode with default parameters
    /// @look
    /// 
    /// ; Activate camera look mode with custom parameters
    /// @look zone:6.5,4 speed:3,2.5 gravity:true
    /// 
    /// ; Disable camera look mode and reset camera offset
    /// @look enabled:false
    /// @camera offset:0,0
    /// </example>
    [CommandAlias("look")]
    public class CameraLook : Command
    {
        /// <summary>
        /// Whether to enable or disable the camera look mode. Default: true.
        /// </summary>
        [CommandParameter(optional: true)]
        public bool Enable { get => GetDynamicParameter(true); set => SetDynamicParameter(value); }
        /// <summary>
        /// A bound box with X,Y sizes in units from the initial camera position, 
        /// describing how far the camera can be moved. Default: 5,3.
        /// </summary>
        [CommandParameter("zone", true)]
        public float?[] LookZone { get => GetDynamicParameter<float?[]>(null); set => SetDynamicParameter(value); }
        /// <summary>
        /// Camera movement speed (sensitivity) by X,Y axes. Default: 1.5,1.
        /// </summary>
        [CommandParameter("speed", true)]
        public float?[] LookSpeed { get => GetDynamicParameter<float?[]>(null); set => SetDynamicParameter(value); }
        /// <summary>
        /// Whether to automatically move camera to the initial position when the look input is not active 
        /// (eg, mouse is not moving or analog stick is in default position). Default: false.
        /// </summary>
        [CommandParameter(optional: true)]
        public bool Gravity { get => GetDynamicParameter(false); set => SetDynamicParameter(value); }

        public override Task ExecuteAsync (CancellationToken cancellationToken = default)
        {
            var camera = Engine.GetService<CameraManager>();

            camera.SetLookMode(Enable, ArrayUtils.ToVector2(LookZone, new Vector2(5, 3)), ArrayUtils.ToVector2(LookSpeed, new Vector2(1.5f, 1f)), Gravity);

            return Task.CompletedTask;
        }
    }
}
