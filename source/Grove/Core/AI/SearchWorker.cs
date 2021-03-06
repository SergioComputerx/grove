﻿namespace Grove.AI
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Grove.Infrastructure;

  public class SearchWorker
  {
    private readonly Game _game;
    private readonly Trackable<int> _moveIndex = new Trackable<int>();
    private readonly Trackable<InnerResult> _parentResult;
    private readonly Search _search;
    private readonly SearchResults _searchResults;

    public SearchWorker(Search search, InnerResult rootResult, Game game, SearchResults searchResults)
    {
      _game = game;

      _search = search;
      _searchResults = searchResults;
      _parentResult = new Trackable<InnerResult>(rootResult).Initialize(game.ChangeTracker);
      _moveIndex.Initialize(game.ChangeTracker);
    }

    public object Id { get { return _game; } }

    private InnerResult ParentResult { get { return _parentResult.Value; } set { _parentResult.Value = value; } }
    private int ResultIndex { get { return _moveIndex.Value; } set { _moveIndex.Value = value; } }
    public int SubTreesPrunned { get; private set; }

    private int SearchDepth { get { return _search.GetCurrentDepthInSteps(_game.Turn.StepCount); } }

    public override string ToString()
    {
      return GetHashCode().ToString();
    }

    public void StartSearch(ISearchNode root)
    {
      LogFile.Debug("Search started, evaluating node: {0}", root);
      EvaluateBranches(root, ParentResult);
    }

    public void Evaluate(ISearchNode node)
    {
      LogFile.Debug("Evaluating node: {0}", node);

      InnerResult rootResult;

      var statehash = _game.CalculateHash();

      var isNew = _searchResults.CreateOrGetExistingResult(
        statehash,
        node.Controller.IsMax,
        SearchDepth,
        out rootResult);


      if (isNew || !ParentResult.HasChildrenWithIndex(ResultIndex))
      {
        ParentResult.AddChild(ResultIndex, rootResult);
      }

      if (isNew)
      {
        EvaluateBranches(node, rootResult);
      }
      else
      {
        SubTreesPrunned++;
        LogFile.Debug("state {0}, prunning node {1}", statehash, node);
      }


      // By evaluating each branch we have already traversed the
      // entire subtree, so we stop the game and yield control to
      // upper level.
      _game.Stop();
    }

    public void EvaluateBranch(int index, ISearchNode searchNode, InnerResult parentResult)
    {
      LogFile.Debug("{0} start eval move {1}", searchNode, index);

      // Create a snapshot, of the game before traversing 
      // a branch.
      var snaphost = _game.CreateSnapshot();

      searchNode.SetResult(index);
      ParentResult = parentResult;
      ResultIndex = index;

      // Traverse this branch, and build result subtree.
      _game.Simulate(_search.SearchUntilDepth);

      if (parentResult.HasChildrenWithIndex(index) == false)
      {
        // No children were added on this branch and the game has
        // finished or we have reached our intended depth.
        // Add a leaf node with game score.
        var leafResult = new LeafResult(_game.Score, SearchDepth);
        parentResult.AddChild(index, leafResult);
      }

      // Restore the game from the snapshot.
      _game.RollbackToSnapshot(snaphost);

      LogFile.Debug("{0} stop eval move {1}", searchNode, index);
    }

    private void EvaluateBranches(ISearchNode searchNode, InnerResult rootResult)
    {
      LogFile.Debug("Evaluating moves of node {0}", searchNode);

      var tasks = new List<Task>();

      for (var i = 0; i < searchNode.ResultCount; i++)
      {
        var task = _search.EvaluateBranch(
          worker: this,
          rootNode: searchNode,
          rootResult: rootResult,
          moveIndex: i);

        if (task != null)
          tasks.Add(task);
      }

      Task.WaitAll(tasks.ToArray());
      return;
    }
  }
}