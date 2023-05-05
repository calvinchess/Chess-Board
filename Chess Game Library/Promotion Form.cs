using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Chess_Game_Library
{
    public partial class Promotion_Form : Form
    {
        public Promotion_Form()
        {
            InitializeComponent();
        }

        public ChessPiece oldPiece;
        public ChessBoard board;

        public void LoadPromotion(ChessBoard board, ChessPiece piece)
        {
            oldPiece = piece;
            this.board = board;
            if(!piece.isWhite)
            {
                pictureBox1.ImageLocation = @"C:\Personal Projects\Chess Game Library\Chess Game Library\Images\black queen.png";
                pictureBox2.ImageLocation = @"C:\Personal Projects\Chess Game Library\Chess Game Library\Images\black rook.png";
                pictureBox3.ImageLocation = @"C:\Personal Projects\Chess Game Library\Chess Game Library\Images\black bishop.png";
                pictureBox4.ImageLocation = @"C:\Personal Projects\Chess Game Library\Chess Game Library\Images\black knight.png";
            }
        }

        private void Promotion_Form_Load(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            board.PromoteTo(oldPiece, new Queen());
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            board.PromoteTo(oldPiece, new Rook());
            this.Close();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            board.PromoteTo(oldPiece, new Bishop());
            this.Close();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            board.PromoteTo(oldPiece, new Knight());
            this.Close();
        }
    }
}
