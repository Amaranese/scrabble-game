using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scrabble.Core
{
    public class RackTile : Button
    {
        public char Letter { get; set; }
        public int LetterValue { get; set; }
        public bool LetterSelected { get; set; }

        public void ClearDisplay()
        {
            this.LetterSelected = false;
            this.FlatStyle = FlatStyle.Standard;
            this.Text = string.Empty;
        }

        public void OnLetterSelected()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderColor = Color.LimeGreen;
            this.FlatAppearance.BorderSize = 5;
            this.LetterSelected = true;
        }

        public void OnLetterDeselected()
        {
            this.FlatStyle = FlatStyle.Standard;
            this.LetterSelected = false;
        }
    }
}
