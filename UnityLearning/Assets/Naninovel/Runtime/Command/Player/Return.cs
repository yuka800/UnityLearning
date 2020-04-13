﻿// Copyright 2017-2019 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.Threading;
using System.Threading.Tasks;
using UnityCommon;
using UnityEngine;

namespace Naninovel.Commands
{
    /// <summary>
    /// Attempts to jump the naninovel script playback to the command after the last used @gosub.
    /// When the target command is not in the currently played script, will also [reset state](/api/#resetstate) 
    /// before loading the target script, unless [ResetStateOnLoad](https://naninovel.com/guide/configuration.html#state) is disabled in the configuration.
    /// See [`@gosub`](/api/#gosub) command summary for more info and usage examples.
    /// </summary>
    public class Return : Command
    {
        public override async Task ExecuteAsync (CancellationToken cancellationToken = default)
        {
            var player = Engine.GetService<ScriptPlayer>();

            if (player.LastGosubReturnSpots.Count == 0 || string.IsNullOrWhiteSpace(player.LastGosubReturnSpots.Peek().ScriptName))
            {
                Debug.LogWarning("Failed to return to the last gosub: state data is missing or invalid.");
                return;
            }

            var spot = player.LastGosubReturnSpots.Pop();

            if (player.PlayedScript != null && player.PlayedScript.Name.EqualsFastIgnoreCase(spot.ScriptName))
            {
                player.Play(player.PlayedScript, spot.LineIndex);
                return;
            }

            var stateManager = Engine.GetService<StateManager>();
            if (stateManager.ResetStateOnLoad)
            {
                var varsManager = Engine.GetService<CustomVariableManager>();
                var localVars = varsManager.GetAllLocalVariables();
                await stateManager?.ResetStateAsync(
                    () => { // Persist the local vars through the state reset.
                        foreach (var kv in localVars)
                            varsManager.SetVariableValue(kv.Key, kv.Value);
                        return Task.CompletedTask;
                    },
                    () => player.PreloadAndPlayAsync(spot.ScriptName, spot.LineIndex)
                );
            }
            else await player.PreloadAndPlayAsync(spot.ScriptName, spot.LineIndex);
        }
    } 
}
