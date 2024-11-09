// <copyright file="SPOILER_SpeakerToy.cs" company="Klybok Team">
// Copyright (c) Klybok Team. All rights reserved.
// </copyright>

#pragma warning disable

using Mirror;  
using UnityEngine;
using VoiceChat.Playbacks;

namespace AdminToys
{
    /// <summary>
    /// Spawns a speaker. (NOT WORKING !!!!)
    /// </summary>
    public class SpeakerToy : AdminToyBase
	{
		/// <summary>
		/// Attached speaker playback used for playing audio.
		/// </summary>
		public SpeakerToyPlaybackBase Playback;

		/// <summary>
		/// Id of controller set inside of Playback.
		/// </summary>
		public byte ControllerId;

		/// <summary>
		/// Indicates whether audio should be played globally or in 3d space around the speaker toy.
		/// </summary>
		public bool IsSpatial = true;

		/// <summary>
		/// The volume of the audio source. This value is clamped between 0 and 1.
		/// </summary>
		public float Volume = 1f;

		/// <summary>
		/// The minimum distance at which the maximum volume level is audible.
		/// </summary>
		public float MinDistance = 1f;

		/// <summary>
		/// The maximum distance at which audio source is audible.
		/// </summary>
		public float MaxDistance = 15f;

		/// <inheritdoc />
		public override string CommandName => "Speaker";

		/// <inheritdoc />
		public override void OnSpawned(ReferenceHub admin, ArraySegment<string> arguments)
		{
		}

		private void OnControllerIdChanged(byte _, byte id)
		{
		}

		private void OnIsSpatialChanged(bool _, bool isSpatial)
		{
		}

		private void OnVolumeChanged(float _, float volume)
		{
		}

		private void OnMinDistanceChanged(float _, float distance)
		{
		}

		private void OnMaxDistanceChanged(float _, float distance)
		{
		}
	}
}

#pragma warning restore