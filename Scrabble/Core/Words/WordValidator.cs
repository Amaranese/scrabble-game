using Scrabble.Core.Movement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scrabble.Core.Words
{
    public class WordValidator
    {
        public List<string> ValidWords { get; set; }
        public ScrabbleForm ScrabbleForm { get; set; }

        public WordValidator()
        {
            this.LoadWords();
        }

        /// <summary>
        /// Loads the list of valid words from the input file.
        /// These words are from the Collin's dictionary of valid scrabble words.
        /// </summary>
        private void LoadWords()
        {
            ValidWords = new List<string>();

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\valid_words.txt");
            foreach (var w in File.ReadAllLines(path))
            {
                ValidWords.Add(w);
            }
        }

        /// <summary>
        /// Check if a provided word is present in the list of known valid words.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool CheckWord(Word word)
        {
            if (ValidWords == null || !ValidWords.Any())
                LoadWords();

            // Todo: maybe not all 1 length words should be valid???
            if (word.Text.Length == 1)
                return true;

            return ValidWords.FirstOrDefault(w => w == word.Text) != null;
        }

        /// <summary>
        /// Validate all the words on the board.
        /// </summary>
        /// <returns></returns>
        public MoveResult ValidateAllWordsInPlay()
        {
            var words = new List<Word>();

            for (int x = 0; x < ScrabbleForm.BOARD_WIDTH; x++)
            {
                for (int y = 0; y < ScrabbleForm.BOARD_HEIGHT; y++)
                {
                    if (!string.IsNullOrEmpty(ScrabbleForm.TileManager.Tiles[x, y].Text) && ScrabbleForm.TileManager.Tiles[x, y].TileInPlay)
                    {
                        foreach (var w in GetSurroundingWords(x, y))
                        {
                            // Todo: need to allow duplicated words if the word actually has been played twice
                            // Think this is sorted, just need to test it.
                            if (!words.Contains(w))
                                words.Add(w);
                        }
                    }
                }
            }

            foreach (var w in words)
            {
                w.Tiles = GetWordTiles(w);
                w.Score = WordScorer.ScoreWord(w);
                w.Valid = CheckWord(w);
                w.SetValidHighlight();
                //MessageBox.Show($"{w} valid: {w.Valid}");
            }

            return new MoveResult {
                TotalScore = words.Sum(w => w.Score),
                Words = words,
                Valid = words.All(w => w.Valid)
            };
        }

        /// <summary>
        /// Get the tiles from the game board that a word has been played on.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public List<ScrabbleTile> GetWordTiles(Word word)
        {
            var tiles = new List<ScrabbleTile>();

            if (word.StartX != word.EndX)
            {
                // Word is played horizontally
                for (var x = word.StartX; x <= word.EndX; x++)
                    tiles.Add(ScrabbleForm.TileManager.Tiles[x, word.StartY]);
            }
            else
            {
                // Word is played vertically
                for (var y = word.StartY; y <= word.EndY; y++)
                    tiles.Add(ScrabbleForm.TileManager.Tiles[word.StartX, y]);
            }

            return tiles;
        }


        /// <summary>
        /// Traverse the board horizontally and vertically from a given point (x, y)
        /// to find the full word in play in both the horizontal and vertical direction.
        /// These words are then validated to ensure that the move is valid.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public List<Word> GetSurroundingWords(int x, int y)
        {
            var words = new List<Word>();

            string horizontal = string.Empty;
            string vertical = string.Empty;

            // Start/End location for the horizonal word
            var tx = x;
            while (tx >= 0 && !string.IsNullOrEmpty(ScrabbleForm.TileManager.Tiles[tx, y].Text))
                tx -= 1;
            tx += 1;

            var tx2 = x;
            while (tx2 < ScrabbleForm.BOARD_WIDTH && !string.IsNullOrEmpty(ScrabbleForm.TileManager.Tiles[tx2, y].Text))
                tx2 += 1;
            tx2 -= 1;

            for (var i = Math.Max(tx, 0); i <= Math.Min(tx2, ScrabbleForm.BOARD_WIDTH - 1); i++)
                horizontal += ScrabbleForm.TileManager.Tiles[i, y].Text;

            // Start/End location for the vertical word
            var ty = y;
            while (ty >= 0 && !string.IsNullOrEmpty(ScrabbleForm.TileManager.Tiles[x, ty].Text))
                ty -= 1;
            ty += 1;

            var ty2 = y;
            while (ty2 < ScrabbleForm.BOARD_WIDTH && !string.IsNullOrEmpty(ScrabbleForm.TileManager.Tiles[x, ty2].Text))
                ty2 += 1;
            ty2 -= 1;

            for (var i = Math.Max(ty, 0); i <= Math.Min(ty2, ScrabbleForm.BOARD_HEIGHT - 1); i++)
                vertical += ScrabbleForm.TileManager.Tiles[x, i].Text;

            if (!string.IsNullOrEmpty(horizontal) && horizontal.Length > 1)
                words.Add(new Word
                {
                    StartX = tx,
                    EndX = tx2,
                    StartY = y,
                    EndY = y,
                    Text = horizontal
                });

            if (!string.IsNullOrEmpty(vertical) && vertical.Length > 1)
                words.Add(new Word
                {
                    StartX = x,
                    EndX = x,
                    StartY = ty,
                    EndY = ty2,
                    Text = vertical
                });

            return words;
        }
    }
}
