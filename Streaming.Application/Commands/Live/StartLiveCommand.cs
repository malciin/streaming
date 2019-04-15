﻿using System;

namespace Streaming.Application.Commands.Live
{
    public class StartLiveCommand : ICommand
    {
        public Guid StreamId { get; set; }
        public string ManifestUrl { get; set; }
    }
}