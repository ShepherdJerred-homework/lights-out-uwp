using System;

namespace lights_out_uwp {
    internal class LightsOutGame {
        private bool[,] _grid; // Stores on/off state of cells in g
        private int _numCells = 3; // Number of cells in grid
        private readonly Random _rand; // Used to generate random numbers

        public LightsOutGame() {
            _rand = new Random(); // Initialize random number generator
            _grid = new bool[_numCells, _numCells];
            // Turn entire grid on
            for (var r = 0; r < _numCells; r++) {
                for (var c = 0; c < _numCells; c++) {
                    _grid[r, c] = true;
                }
            }
        }

        // Returns the number of horizontal/vertical cells in the grid
        public int NumCells {
            get => _numCells;
            set {
                _numCells = value;
            }
        }

        // Returns the grid value at the given row and column
        public bool GetGridValue(int row, int col) {
            return _grid[row, col];
        }

        // Randomizes the grid
        public void NewGame() {
            _grid = new bool[_numCells, _numCells];
            // Fill grid with either white or black
            for (var r = 0; r < _numCells; r++) {
                for (var c = 0; c < _numCells; c++) {
                    _grid[r, c] = _rand.Next(2) == 1;
                }
            }
        }

        // Inverts the selected box and all surrounding boxes
        public void MakeMove(int row, int col) {
            for (var i = row - 1; i <= row + 1; i++) {
                for (var j = col - 1; j <= col + 1; j++) {
                    if (i >= 0 && i < _numCells && j >= 0 && j < _numCells) {
                        _grid[i, j] = !_grid[i, j];
                    }
                }
            }
        }

        // Returns true if all cells are off
        public bool PlayerWon() {
            for (var r = 0; r < _numCells; r++) {
                for (var c = 0; c < _numCells; c++) {
                    if (_grid[r, c]) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}