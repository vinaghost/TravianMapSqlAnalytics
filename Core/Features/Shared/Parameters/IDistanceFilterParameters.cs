﻿namespace Core.Features.Shared.Parameters
{
    public interface IDistanceFilterParameters
    {
        public int X { get; }
        public int Y { get; }

        public int MinDistance { get; }
        public int MaxDistance { get; }
    }
}