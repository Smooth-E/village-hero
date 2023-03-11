//
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.
//
//  Email:  gustavo_franco@hotmail.com
//
//  Copyright (C) 2006 Franco, Gustavo 
//

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Algorithms
{
    
// #region Enum
// 
//     public enum HeuristicFormula
//     {
//         Manhattan           = 1,
//         MaxDXDY             = 2,
//         DiagonalShortCut    = 3,
//         Euclidean           = 4,
//         EuclideanNoSQR      = 5,
//         Custom1             = 6
//     }
// 
// #endregion
	
    public class ReimplementedPathFinder
    {

    #region Structs

        public struct Location
        {
            public Location(int xy, int z)
            {
                this.xy = xy;
                this.z = z;
            }

            public int xy;
            public int z;
        }

        internal struct PathFinderNodeFast
        {

        #region Variables Declaration

            // f = gone + heuristic
            public int FCost;
            public int GCost;
            public ushort ParentX;
            public ushort ParentY;
            public byte ParentZ;
            public byte Status;
			public short JumpLength;

        #endregion

            public PathFinderNodeFast UpdateStatus(byte newStatus)
            {
                PathFinderNodeFast newNode = this;
                newNode.Status = newStatus;
                return newNode;
            }
        }
		

    #endregion
		
		
    #region Variables Declaration
	
    	private List<PathFinderNodeFast>[] nodes;
        private Stack<int> touchedLocations;

        // Heap variables are initialized to default, but I like to do it anyway

        private byte[,] _grid = null;
        private PriorityQueueB<Location> _openNodes = null;
        private List<Vector2Int> _closedNodes = null;
        private bool _stop = false;
        private bool _stopped = true;
        private HeuristicFormula _formula = HeuristicFormula.Manhattan;
        private bool mDiagonals = true;
        private int mHEstimate = 2;
        private bool _punishChangeDirection = false;
        private bool _tieBreaker = false;
        private bool _heavyDiagonals = false;
        private int _searchLimit = 2000;
        private double _completedTime = 0;
        private bool _debugProgress = false;
        private bool _debugFoundPath = false;
        private byte _openNodeValue = 1;
        private byte _closeNodeValue = 2;
        
        // Promoted local variables to member variables to avoid recreation between calls
        private int _hCost = 0;
        private Location _location;
        private int _newLocation = 0;
        private ushort _locationX = 0;
        private ushort _locationY = 0;
        private ushort _newLocationX = 0;
        private ushort _newLocationY = 0;
        private int _closeNodeCounter = 0;
        private ushort _gridX = 0;
        private ushort _gridY = 0;
        private ushort _gridXMinus1 = 0;
        private ushort _gridXLog2 = 0;
        private bool _found = false;
        private sbyte[,] _directions = new sbyte[8,2] { {0,-1} , {1,0}, {0,1}, {-1,0}, {1,-1}, {1,1}, {-1,1}, {-1,-1} };
        private int _endLocation = 0;
        private int _newGCost = 0;
		
		public Map _map;

    #endregion
		
    #region Constructors

        public ReimplementedPathFinder(byte[,] grid, Map map)
        {
			if (map == null)
				throw new Exception("Map cannot be null");
            if (grid == null)
                throw new Exception("Grid cannot be null");
			
			_map = map;
            _grid           = grid;
            _gridX          = (ushort) (_grid.GetUpperBound(0) + 1);
            _gridY          = (ushort) (_grid.GetUpperBound(1) + 1);
            _gridXMinus1    = (ushort) (_gridX - 1);
            _gridXLog2      = (ushort) Math.Log(_gridX, 2);

            if (Math.Log(_gridX, 2) != (int) Math.Log(_gridX, 2) ||
                Math.Log(_gridY, 2) != (int) Math.Log(_gridY, 2))
                throw new Exception("Invalid Grid, size in X and Y must be power of 2");

            if (nodes == null || nodes.Length != (_gridX * _gridY))
			{
				nodes = new List<PathFinderNodeFast>[_gridX * _gridY];
                touchedLocations = new Stack<int>(_gridX * _gridY);
                _closedNodes = new List<Vector2Int>(_gridX * _gridY);
			}
			
			for (var i = 0; i < nodes.Length; ++i)
				nodes[i] = new List<PathFinderNodeFast>(1);

            _openNodes   = new PriorityQueueB<Location>(new ComparePFNodeMatrix(nodes));
        }

    #endregion

    #region Properties

        public bool Stopped => _stopped;

        public HeuristicFormula Formula
        {
            get => _formula;
            set => _formula = value;
        }

        public bool Diagonals
        {
            get => mDiagonals;
            set 
            { 
                mDiagonals = value; 

                if (mDiagonals)
                    _directions = new sbyte[8,2] { {0,-1} , {1,0}, {0,1}, {-1,0}, {1,-1}, {1,1}, {-1,1}, {-1,-1} };
                else
                    _directions = new sbyte[4,2] { {0,-1} , {1,0}, {0,1}, {-1,0} };
            }
        }

        public bool HeavyDiagonals
        {
            get { return _heavyDiagonals; }
            set { _heavyDiagonals = value; }
        }

        public int HeuristicEstimate
        {
            get { return mHEstimate; }
            set { mHEstimate = value; }
        }

        public bool PunishChangeDirection
        {
            get { return _punishChangeDirection; }
            set { _punishChangeDirection = value; }
        }

        public bool TieBreaker
        {
            get { return _tieBreaker; }
            set { _tieBreaker = value; }
        }

        public int SearchLimit
        {
            get { return _searchLimit; }
            set { _searchLimit = value; }
        }

        public double CompletedTime
        {
            get { return _completedTime; }
            set { _completedTime = value; }
        }

        public bool DebugProgress
        {
            get { return _debugProgress; }
            set { _debugProgress = value; }
        }

        public bool DebugFoundPath
        {
            get { return _debugFoundPath; }
            set { _debugFoundPath = value; }
        }

    #endregion

    #region Methods

        public void FindPathStop()
        {
            _stop = true;
        }

        public List<Vector2Int> FindPath(
            Vector2Int start, 
            Vector2Int end, 
            int characterWidth, 
            int characterHeight, 
            short maxCharacterJumpHeight
        ) {
            lock(this)
            {
                while (touchedLocations.Count > 0)
                    nodes[touchedLocations.Pop()].Clear();

                var inSolidTile = false;

                for (var i = 0; i < 2; ++i)
                {
                    inSolidTile = false;
                    for (var w = 0; w < characterWidth; ++w)
                    {
                        if (_grid[end.x + w, end.y] == 0
                            || _grid[end.x + w, end.y + characterHeight - 1] == 0)
                        {
                            inSolidTile = true;
                            break;
                        }

                    }
                    if (inSolidTile == false)
                    {
                        for (var h = 1; h < characterHeight - 1; ++h)
                        {
                            if (_grid[end.x, end.y + h] == 0
                                || _grid[end.x + characterWidth - 1, end.y + h] == 0)
                            {
                                inSolidTile = true;
                                break;
                            }
                        }
                    }

                    if (inSolidTile)
                        end.x -= characterWidth - 1;
                    else
                        break;
                }

                if (inSolidTile == true)
                    return null;

                _found = false;
                _stop = false;
                _stopped = false;
                _closeNodeCounter = 0;
                _openNodeValue += 2;
                _closeNodeValue += 2;
                _openNodes.Clear();

                _location.xy = (start.y << _gridXLog2) + start.x;
                _location.z = 0;
                _endLocation = (end.y << _gridXLog2) + end.x;

                PathFinderNodeFast firstNode = new PathFinderNodeFast();
                firstNode.GCost = 0;
                firstNode.FCost = mHEstimate;
                firstNode.ParentX = (ushort)start.x;
                firstNode.ParentY = (ushort)start.y;
                firstNode.ParentZ = 0;
                firstNode.Status = _openNodeValue;

                bool startsOnGround = false;

                for (int x = start.x; x < start.x + characterWidth; ++x)
                {
                    if (_map.IsTileBlock(x, start.y - 1))
                    {
                        startsOnGround = true;
                        break;
                    }
                }

                if (startsOnGround)
                    firstNode.JumpLength = 0;
                else
                    firstNode.JumpLength = (short)(maxCharacterJumpHeight * 2);

                nodes[_location.xy].Add(firstNode);
                touchedLocations.Push(_location.xy);

                _openNodes.Push(_location);
				
                while(_openNodes.Count > 0 && !_stop)
                {
                    _location    = _openNodes.Pop();

                    //Is it in closed list? means this node was already processed
                    if (nodes[_location.xy][_location.z].Status == _closeNodeValue)
                        continue;

                    _locationX   = (ushort) (_location.xy & _gridXMinus1);
                    _locationY   = (ushort) (_location.xy >> _gridXLog2);

                    if (_location.xy == _endLocation)
                    {
                        nodes[_location.xy][_location.z] = nodes[_location.xy][_location.z].UpdateStatus(_closeNodeValue);
                        _found = true;
                        break;
                    }

                    if (_closeNodeCounter > _searchLimit)
                    {
                        _stopped = true;
                        return null;
                    }

                    //Lets calculate each successors
                    for (var i=0; i<(mDiagonals ? 8 : 4); i++)
                    {
                        _newLocationX = (ushort) (_locationX + _directions[i,0]);
                        _newLocationY = (ushort) (_locationY + _directions[i,1]);
                        _newLocation  = (_newLocationY << _gridXLog2) + _newLocationX;

                        var onGround = false;
                        var atCeiling = false;

                        for (var w = 0; w < characterWidth; ++w)
                        {
                            if (_grid[_newLocationX + w, _newLocationY] == 0
                                || _grid[_newLocationX + w, _newLocationY + characterHeight - 1] == 0)
                                goto CHILDREN_LOOP_END;

                            if (_map.IsTileBlock(_newLocationX + w, _newLocationY - 1))
                                onGround = true;
                            else if (_grid[_newLocationX + w, _newLocationY + characterHeight] == 0)
                                atCeiling = true;
                        }
                        for (var h = 1; h < characterHeight - 1; ++h)
                        {
                            if (_grid[_newLocationX, _newLocationY + h] == 0
                                || _grid[_newLocationX + characterWidth - 1, _newLocationY + h] == 0)
                                goto CHILDREN_LOOP_END;
                        }
						
						//calculate a proper jumplength value for the successor

                        var jumpLength = nodes[_location.xy][_location.z].JumpLength;
                        short newJumpLength = jumpLength;

                        if (onGround)
							newJumpLength = 0;
						else if (atCeiling)
                        {
                            if (_newLocationX != _locationX)
                                newJumpLength = (short)Mathf.Max(maxCharacterJumpHeight * 2 + 1, jumpLength + 1);
                            else
                                newJumpLength = (short)Mathf.Max(maxCharacterJumpHeight * 2, jumpLength + 2);
                        }
						else if (_newLocationY > _locationY)
						{
                            if (jumpLength < 2 && maxCharacterJumpHeight > 2) //first jump is always two block up instead of one up and optionally one to either right or left
                                newJumpLength = 3;
                            else  if (jumpLength % 2 == 0)
                                newJumpLength = (short)(jumpLength + 2);
                            else
                                newJumpLength = (short)(jumpLength + 1);
						}
						else if (_newLocationY < _locationY)
						{
							if (jumpLength % 2 == 0)
								newJumpLength = (short)Mathf.Max(maxCharacterJumpHeight * 2, jumpLength + 2);
							else
								newJumpLength = (short)Mathf.Max(maxCharacterJumpHeight * 2, jumpLength + 1);
						}
						else if (!onGround && _newLocationX != _locationX)
							newJumpLength = (short)(jumpLength + 1);

                        if (jumpLength >= 0 && jumpLength % 2 != 0 && _locationX != _newLocationX)
                            continue;

                        if ((newJumpLength == 0 && _newLocationX != _locationX && jumpLength + 1 >= maxCharacterJumpHeight * 2 + 6 && (jumpLength + 1 - (maxCharacterJumpHeight * 2 + 6)) % 8 <= 1)
                             || (newJumpLength >= maxCharacterJumpHeight * 2 + 6 && _newLocationX != _locationX && (newJumpLength - (maxCharacterJumpHeight * 2 + 6)) % 8 != 7))
							continue;

                        //if we're falling and succeor's height is bigger than ours, skip that successor
						if (jumpLength >= maxCharacterJumpHeight * 2 && _newLocationY > _locationY)
							continue;

                        _newGCost = nodes[_location.xy][_location.z].GCost + _grid[_newLocationX, _newLocationY] + newJumpLength / 4;

                        if (nodes[_newLocation].Count > 0)
                        {
                            int lowestJump = short.MaxValue;
                            int lowestG = short.MaxValue;
                            bool couldMoveSideways = false;
                            for (int j = 0; j < nodes[_newLocation].Count; ++j)
                            {
                                if (nodes[_newLocation][j].JumpLength < lowestJump)
                                    lowestJump = nodes[_newLocation][j].JumpLength;

                                if (nodes[_newLocation][j].GCost < lowestG)
                                    lowestG = nodes[_newLocation][j].GCost;

                                if (nodes[_newLocation][j].JumpLength % 2 == 0 && nodes[_newLocation][j].JumpLength < maxCharacterJumpHeight * 2 + 6)
                                    couldMoveSideways = true;
                            }

                            // The current node has smaller cost than the previous? then skip this node
                            if (lowestG <= _newGCost && lowestJump <= newJumpLength && (newJumpLength % 2 != 0 || newJumpLength >= maxCharacterJumpHeight * 2 + 6 || couldMoveSideways))
                                continue;
                        }
						
                        switch(_formula)
                        {
                            default:
                            case HeuristicFormula.Manhattan:
                                _hCost = mHEstimate * (Mathf.Abs(_newLocationX - end.x) + Mathf.Abs(_newLocationY - end.y));
                                break;
                            case HeuristicFormula.MaxDXDY:
                                _hCost = mHEstimate * (Math.Max(Math.Abs(_newLocationX - end.x), Math.Abs(_newLocationY - end.y)));
                                break;
                            case HeuristicFormula.DiagonalShortCut:
                                var h_diagonal  = Math.Min(Math.Abs(_newLocationX - end.x), Math.Abs(_newLocationY - end.y));
                                var h_straight  = (Math.Abs(_newLocationX - end.x) + Math.Abs(_newLocationY - end.y));
                                _hCost = (mHEstimate * 2) * h_diagonal + mHEstimate * (h_straight - 2 * h_diagonal);
                                break;
                            case HeuristicFormula.Euclidean:
                                _hCost = (int) (mHEstimate * Math.Sqrt(Math.Pow((_newLocationY - end.x) , 2) + Math.Pow((_newLocationY - end.y), 2)));
                                break;
                            case HeuristicFormula.EuclideanNoSQR:
                                _hCost = (int) (mHEstimate * (Math.Pow((_newLocationX - end.x) , 2) + Math.Pow((_newLocationY - end.y), 2)));
                                break;
                            case HeuristicFormula.Custom1:
                                var dxy       = new Vector2Int(Math.Abs(end.x - _newLocationX), Math.Abs(end.y - _newLocationY));
                                var Orthogonal  = Math.Abs(dxy.x - dxy.y);
                                var Diagonal    = Math.Abs(((dxy.x + dxy.y) - Orthogonal) / 2);
                                _hCost = mHEstimate * (Diagonal + Orthogonal + dxy.x + dxy.y);
                                break;
                        }

                        PathFinderNodeFast newNode = new PathFinderNodeFast();
                        newNode.JumpLength = newJumpLength;
                        newNode.ParentX = _locationX;
                        newNode.ParentY = _locationY;
                        newNode.ParentZ = (byte)_location.z;
                        newNode.GCost = _newGCost;
                        newNode.FCost = _newGCost + _hCost;
                        newNode.Status = _openNodeValue;

                        if (nodes[_newLocation].Count == 0)
                            touchedLocations.Push(_newLocation);

                        nodes[_newLocation].Add(newNode);
                        _openNodes.Push(new Location(_newLocation, nodes[_newLocation].Count - 1));
						
					CHILDREN_LOOP_END:
						continue;
                    }

                    nodes[_location.xy][_location.z] = nodes[_location.xy][_location.z].UpdateStatus(_closeNodeValue);
                    _closeNodeCounter++;
                }

                if (_found)
                {
                    _closedNodes.Clear();
                    var posX = end.x;
                    var posY = end.y;
					
					var fPrevNodeTmp = new PathFinderNodeFast();
                    var fNodeTmp = nodes[_endLocation][0];
					
                    var fNode = end;
                    var fPrevNode = end;

                    var loc = (fNodeTmp.ParentY << _gridXLog2) + fNodeTmp.ParentX;
					
                    while(fNode.x != fNodeTmp.ParentX || fNode.y != fNodeTmp.ParentY)
                    {
                        var fNextNodeTmp = nodes[loc][fNodeTmp.ParentZ];
                        
                        if ((_closedNodes.Count == 0)
                            || (_map.IsTileOneWay(fNode.x, fNode.y - 1))
                            || (_grid[fNode.x, fNode.y - 1] == 0 && _map.IsTileOneWay(fPrevNode.x, fPrevNode.y - 1))
                            || (fNodeTmp.JumpLength == 3)
                            || (fNextNodeTmp.JumpLength != 0 && fNodeTmp.JumpLength == 0)                                                                                                       //mark jumps starts
                            || (fNodeTmp.JumpLength == 0 && fPrevNodeTmp.JumpLength != 0)                                                                                                       //mark landings
                            || (fNode.y > _closedNodes[_closedNodes.Count - 1].y && fNode.y > fNodeTmp.ParentY)
                            || (fNode.y < _closedNodes[_closedNodes.Count - 1].y && fNode.y < fNodeTmp.ParentY)
                            || ((_map.IsTileBlock(fNode.x - 1, fNode.y) || _map.IsTileBlock(fNode.x + 1, fNode.y)) 
                                && fNode.y != _closedNodes[_closedNodes.Count - 1].y && fNode.x != _closedNodes[_closedNodes.Count - 1].x))
                            _closedNodes.Add(fNode);

                        fPrevNode = fNode;
						posX = fNodeTmp.ParentX;
                        posY = fNodeTmp.ParentY;
						fPrevNodeTmp = fNodeTmp;
                        fNodeTmp = fNextNodeTmp;
						loc = (fNodeTmp.ParentY << _gridXLog2) + fNodeTmp.ParentX;
                        fNode = new Vector2Int(posX, posY);
                    } 

                    _closedNodes.Add(fNode);

                    _stopped = true;

                    return _closedNodes;
                }
                _stopped = true;
                return null;
            }
        }

    #endregion

    #region Inner Classes

        internal class ComparePFNodeMatrix : IComparer<Location>
        {

        #region Variables Declaration

            List<PathFinderNodeFast>[] mMatrix;

        #endregion

        #region Constructors

            public ComparePFNodeMatrix(List<PathFinderNodeFast>[] matrix) =>
                mMatrix = matrix;

        #endregion

        #region IComparer Members

            public int Compare(Location a, Location b)
            {
                if (mMatrix[a.xy][a.z].FCost > mMatrix[b.xy][b.z].FCost)
                    return 1;
                else if (mMatrix[a.xy][a.z].FCost < mMatrix[b.xy][b.z].FCost)
                    return -1;
                
                return 0;
            }

        #endregion

        }

    #endregion

    }
}
