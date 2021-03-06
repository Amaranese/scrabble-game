using Scrabble.Core.Words;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scrabble.Core.Tile
{
    public class RackManager
    {
        public ScrabbleForm ScrabbleForm { get; set; }

        /// <summary>
        /// Initialize the rack.
        /// </summary>
        public List<RackTile> SetupRack()
        {
            var tiles = new List<RackTile>();

            for (int x = 1; x <= ScrabbleForm.RACK_TILES; x++)
            {
                var tile = new RackTile();
                tile.BackColor = Color.Goldenrod;
                tile.Location = new Point(175 + (x * (ScrabbleForm.TILE_SIZE + 2)), 825);
                tile.Size = new Size(ScrabbleForm.TILE_SIZE, ScrabbleForm.TILE_SIZE);
                tile.UseVisualStyleBackColor = false;
                tile.ForeColor = Color.Black;
                tile.Font = new Font("Verdana", 15.75F, FontStyle.Regular);
                tile.TextAlign = ContentAlignment.MiddleCenter;
                tile.Click += RackTile_Click;
                tile.Visible = false;
                ScrabbleForm.Controls.Add(tile);

                tiles.Add(tile);
            }

            FillRack(tiles);

            return tiles;
        }

        /// <summary>
        /// Fills the player's rack with tiles. Will attempt to fill the rack completely,
        /// or just take as many as it can if there's not enough tiles left to completely re-fill 
        /// the rack.
        /// </summary>
        public void FillRack(List<RackTile> tiles)
        {
            // How many letters in the rack are missing?
            int missingLetters = tiles.Where(r => string.IsNullOrEmpty(r.Text)).Count();

            // Take random letters from the tile back, and fill up the rack again.
            var letters = ScrabbleForm.TileManager.TileBag.TakeLetters(missingLetters);
            for (int x = 0; x < letters.Length; x++)
            {
                var tile = tiles.FirstOrDefault(r => string.IsNullOrEmpty(r.Text));

                tile.Letter = letters[x];
                tile.LetterValue = WordScorer.LetterValue(letters[x]);

                tile.Text = letters[x].ToString();
            }
        }

        /// <summary>
        /// Event handler for when a tile in the player's rack is clicked.
        /// Highlights the tile so you can clearly see it's been selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RackTile_Click(object sender, EventArgs e)
        {
            if (!ScrabbleForm.GamePlaying) return;

            var tile = (RackTile) sender;
            var alreadySelected = tile.LetterSelected;

            // Reset the selected display of all tiles.
            foreach (var t in ScrabbleForm.PlayerManager.CurrentPlayer.Tiles)
            {
                tile.OnLetterDeselected();

                //if (t.LetterSelected)
                //{
                //    // Another tile was selected so swap them around, allows reordering of the rack.
                //    // Todo: this has broken the ability to swap letters
                //    var tmpLetter = tile.Letter;
                //    var tmpText = tile.Text;

                //    tile.Letter = t.Letter;
                //    tile.Text = t.Text;

                //    t.Letter = tmpLetter;
                //    t.Text = tmpText;

                //    //ScrabbleForm.PlayerManager.CurrentPlayer.Tiles.ForEach(rt => rt.OnLetterDeselected());
                //    //return;
                //}
            }

            // If it wasn't already selected (e.g you're now de-selecting it)
            // then apply the "selected" styling to the button.
            if (!alreadySelected)
                tile.OnLetterSelected();
        }
    }
}
