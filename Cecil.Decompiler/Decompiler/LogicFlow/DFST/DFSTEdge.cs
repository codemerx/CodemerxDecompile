using System;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DFST
{
    internal class DFSTEdge : IEquatable<DFSTEdge>
    {
        /// <summary>
        /// Gets the start of the edge.
        /// </summary>
        public DFSTNode Start { get; private set; }

        /// <summary>
        /// Gets the end of the edge.
        /// </summary>
        public DFSTNode End { get; private set; }

        public DFSTEdge(DFSTNode start, DFSTNode end)
        {
            Start = start;
            End = end;
        }

        public override int GetHashCode()
        {
            //Every hash function must have bitwise operations!! (total overkill in this case)
            uint startHash = (uint)Start.GetHashCode();
            uint endHash = (uint)End.GetHashCode();
            int uintBitSize = sizeof(uint) << 3;
            int count = (int)(startHash | endHash) & (uintBitSize - 1);
            
            return (int)(((startHash << count) | (startHash >> (uintBitSize - count))) ^ endHash);
        }

        public override bool Equals(object obj)
        {
            DFSTEdge other = obj as DFSTEdge;
            return this.Equals(other);
        }

        public bool Equals(DFSTEdge other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Start == other.Start && this.End == other.End;
        }
    }
}
