using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Main : MonoBehaviour
    {
        public Tile[] Tiles;

        private GameState _gameState = GameState.UpdatePattern;

        private const float ShowPatternTimeout = 0.75f;
        private float _showPatternElapsed;

        private readonly List<int> _tilePattern = new List<int>();
        private int _currentTileIndex;

        private readonly System.Random _rand = new System.Random();

        void Start()
        {
            var index = 0;
            foreach (var tile in Tiles)
            {
                tile.Index = index++;
            }
        }

        void Update()
        {
            switch (_gameState)
            {
                case GameState.UserPlaying:
                    if (_currentTileIndex + 1 > _tilePattern.Count)
                    {
                        _gameState = GameState.UpdatePattern;
                        _currentTileIndex = 0;
                    }
                    else
                    {
                        var selectedTile = Tiles.FirstOrDefault(x => x.IsClicked);

                        if (selectedTile == null)
                            return;

                        if (selectedTile.Index == _tilePattern[_currentTileIndex])
                        {
                            _currentTileIndex++;
                        }
                        else
                        {
                            _gameState = GameState.PlayerLost;
                            Debug.Log("GAME FAILED, score: " + _tilePattern.Count);
                        }
                    }

                    ClearTilesIsClicked();

                    break;
                case GameState.UpdatePattern:
                    AddTileSequence();
                    _gameState = GameState.ShowingPattern;
                    break;
                case GameState.ShowingPattern:
                    _showPatternElapsed += Time.deltaTime;
                    if (_showPatternElapsed >= ShowPatternTimeout)
                    {
                        DumpState("pattern elapsed");
                        _showPatternElapsed = 0f;
                        if (_currentTileIndex + 1 > _tilePattern.Count)
                        {
                            DumpState("going to user playing");
                            _currentTileIndex = 0;
                            _gameState = GameState.UserPlaying;
                            SetAllowTilesToBeClicked(true);
                        }
                        else
                        {
                            DumpState("highlighting tile " + _currentTileIndex);
                            Tiles[_tilePattern[_currentTileIndex]].Highlight();
                            _currentTileIndex++;
                        }
                    }

                    break;
                case GameState.PlayerLost:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddTileSequence()
        {
            var newIndex = _rand.Next(0, Tiles.Length);
            _tilePattern.Add(newIndex);
        }

        private void DumpState(string message)
        {
            Debug.Log(_gameState + ": " + message);
        }

        private void SetAllowTilesToBeClicked(bool allow)
        {
            foreach (var tile in Tiles)
            {
                tile.SetAllowToBeClicked(allow);
            }
        }

        private void ClearTilesIsClicked()
        {
            foreach (var tile in Tiles)
            {
                tile.ClearIsClicked();
            }
        }
    }

    public enum GameState
    {
        UserPlaying,
        UpdatePattern,
        ShowingPattern,
        PlayerLost
    }
}