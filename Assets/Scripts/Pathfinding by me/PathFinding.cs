using Algorithms;

public static class PathFinding
{

    private static ReimplementedPathFinder _pathFinder;
    public static ReimplementedPathFinder PathFinder  => _pathFinder;

    public static void Initialize(ReimplementedMap map)
    {

        _pathFinder = new ReimplementedPathFinder(map.CreateByteGrid(), map);
		
		_pathFinder.Formula                 = HeuristicFormula.Manhattan;
		// If false then diagonal movement will be prohibited
        _pathFinder.Diagonals               = false;
		// If true then diagonal movement will have higher cost
        _pathFinder.HeavyDiagonals          = false;
		// Estimate of path length
        _pathFinder.HeuristicEstimate       = 6;
        _pathFinder.PunishChangeDirection   = false;
        _pathFinder.TieBreaker              = false;
        _pathFinder.SearchLimit             = 1000000;
        _pathFinder.DebugProgress           = false;
        _pathFinder.DebugFoundPath          = false;
    }

}
