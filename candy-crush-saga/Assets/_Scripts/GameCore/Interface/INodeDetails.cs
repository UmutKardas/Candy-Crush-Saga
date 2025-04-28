
using GameCore.Scriptables;
using UnityEngine;

namespace Interface
{
    public interface INodeDetails
    {
        Vector3 Position { get; }
        int Row { get; set; }
        int Column { get; set; }
        CandyType CandyType { get; set; }
    }
}