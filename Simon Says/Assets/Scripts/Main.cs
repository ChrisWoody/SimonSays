using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Main : MonoBehaviour
    {
        public Tile[] Tiles;
        public Canvas StartMenu;
        public Canvas GameOverMenu;
        public Text ScoreText;
        public Text HighscoreText;

        private GameState _gameState = GameState.ReadyToStartGame;

        private const float ShowPatternTimeout = 0.75f;
        private float _showPatternElapsed;

        private readonly List<int> _tilePattern = new List<int>();
        private int _currentTileIndex;

        private readonly System.Random _rand = new System.Random();

        private int _highscore;

        void Start()
        {
            var index = 0;
            foreach (var tile in Tiles)
            {
                tile.Index = index++;
            }
            GameOverMenu.enabled = false;
        }

        void Update()
        {
            switch (_gameState)
            {
                case GameState.ReadyToStartGame:
                    // do nothing, we are waiting for user to start the game
                    break;
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
                            DumpState("player lost");
                            _gameState = GameState.PlayerLost;
                            SetAllowTilesToBeClicked(false);
                            ShowGameOverMenu();
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
                    // do nothing, we are waiting for user to start the game
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ShowGameOverMenu()
        {
            var score = _tilePattern.Count - 1;
            _highscore = Math.Max(score, _highscore);
            ScoreText.text = "Score: " + score;
            HighscoreText.text = "Highscore: " + _highscore;
            GameOverMenu.enabled = true;
        }

        public void StartGame()
        {
            ResetGame();
            StartMenu.enabled = false;
            GameOverMenu.enabled = false;
            _gameState = GameState.UpdatePattern;
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

        private void ResetGame()
        {
            _showPatternElapsed = 0f;
            _currentTileIndex = 0;
            _tilePattern.Clear();
            SetAllowTilesToBeClicked(false);
            ClearTilesIsClicked();
        }
    }

    public enum GameState
    {
        ReadyToStartGame,
        UserPlaying,
        UpdatePattern,
        ShowingPattern,
        PlayerLost
    }
}