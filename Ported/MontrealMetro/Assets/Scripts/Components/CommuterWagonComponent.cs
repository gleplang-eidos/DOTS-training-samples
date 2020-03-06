﻿using System;
using Unity.Entities;
using Unity.Mathematics; 

public struct CommuterWagonComponent: IComponentData
{
    public enum EBoardingState
    {
        Boarding,
        Unboarding
    }

    public Entity Wagon;
    public float3 EntryPosition;
    public float3 SeatPosition;
    public EBoardingState BoardingState;
}
