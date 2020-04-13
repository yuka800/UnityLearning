﻿// Copyright 2017-2019 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Naninovel.Commands
{
    /// <summary>
    /// Spawns a prefab stored in project resources.
    /// </summary>
    /// <remarks>
    /// If prefab has a <see cref="MonoBehaviour"/> component attached the root object, and the component implements
    /// a <see cref="IParameterized"/> interface, will pass the specified `params` values after the spawn;
    /// if the component implements <see cref="IAwaitable"/> interface, command execution will wait for
    /// the async completion task returned by the implementation.
    /// </remarks>
    /// <example>
    /// ; Given the project contains an `Assets/Resources/Rain.prefab` asset, spawn it
    /// @spawn Rain
    /// </example>
    public class Spawn : Command, Command.IPreloadable
    {
        public interface IParameterized { void SetSpawnParameters (string[] parameters); }
        public interface IAwaitable { Task AwaitSpawnAsync (CancellationToken cancellationToken = default); }

        /// <summary>
        /// Path to the prefab resource to spawn. Path is relative to a `./Resources` folder, eg 
        /// given a `Assets/Resources/FX/Explosion.prefab` asset, use the following path to spawn it: `FX/Explosion`.
        /// </summary>
        [CommandParameter(alias: NamelessParameterAlias)]
        public string Path { get => GetDynamicParameter<string>(null); set => SetDynamicParameter(value); }
        /// <summary>
        /// Parameters to set when spawning the prefab.
        /// Requires the prefab to have a <see cref="IParameterized"/> component attached the root object.
        /// </summary>
        [CommandParameter(optional: true)]
        public string[] Params { get => GetDynamicParameter<string[]>(null); set => SetDynamicParameter(value); }

        protected virtual string FullPath => Path;
        protected virtual SpawnManager SpawnManager => Engine.GetService<SpawnManager>();

        public async Task HoldResourcesAsync ()
        {
            if (string.IsNullOrWhiteSpace(FullPath)) return;
            await SpawnManager.HoldResourcesAsync(this, FullPath);
        }

        public void ReleaseResources ()
        {
            if (string.IsNullOrWhiteSpace(FullPath)) return;
            SpawnManager.ReleaseResources(this, FullPath);
        }

        public override async Task ExecuteAsync (CancellationToken cancellationToken = default)
        {
            await SpawnManager.SpawnAsync(FullPath, cancellationToken, Params);
        }
    }
}
