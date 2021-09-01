using System;

namespace Misadventure.Entities.CaterpillarStates
{
    public class CaterpillarStateEventArgs : EventArgs
    {
        public Type StateType { get; }
        public SegmentComponent Segment { get; }

        public CaterpillarStateEventArgs(Type stateType, SegmentComponent segment)
        {
            StateType = stateType;
            Segment = segment;
        }
    }
}