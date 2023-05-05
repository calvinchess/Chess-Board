using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess_Game_Library
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public int boardSquareSize = 100;
        public int startingX = 50;
        public int startingY = 50;
        public bool whitePOV = true;
        public ChessBoard board;

        private void Form1_Load(object sender, EventArgs e)
        {
            board = new ChessBoard();

            LoadChessBoardPictures(board);
        }

        public void LoadChessBoardPictures(ChessBoard board)
        {
            for (int i = 0; i < 8; i++)
            {
                for(int x = 0; x < 8;x++)
                {
                    if (whitePOV) board.squares[i][x].UpdatePicture(startingY + boardSquareSize * x, (startingX + boardSquareSize * 7) - (startingX + boardSquareSize * i) + startingX, boardSquareSize);
                    else board.squares[i][x].UpdatePicture((startingY + boardSquareSize * 7) - (startingY + boardSquareSize * x) + startingY, startingX + boardSquareSize * i, boardSquareSize);
                    this.Controls.Add(board.squares[i][x].background);
                    if (board.squares[i][x].piece != null)
                    {
                        this.Controls.Add(board.squares[i][x].piece.picture);
                        board.squares[i][x].piece.picture.BringToFront();
                        board.squares[i][x].piece.picture.Refresh();
                    }
                }
            }
        }

        public void UpdateBoard(ChessBoard board)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (whitePOV) board.squares[i][x].UpdatePicture(startingY + boardSquareSize * x, (startingX + boardSquareSize * 7) - (startingX + boardSquareSize * i) + startingX, boardSquareSize);
                    else board.squares[i][x].UpdatePicture((startingY + boardSquareSize * 7) - (startingY + boardSquareSize * x) + startingY, startingX + boardSquareSize * i, boardSquareSize);
                    if (board.squares[i][x].piece != null)
                    {
                        board.squares[i][x].piece.picture.BringToFront();
                        board.squares[i][x].piece.picture.Refresh();
                    }
                }
            }
        }

        public void UnloadChessBoardPictures(ChessBoard board)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int x = 0; x < 8; x++)
                {
                    this.Controls.Remove(board.squares[i][x].background);
                    if (board.squares[i][x].piece != null)
                    {
                        this.Controls.Remove(board.squares[i][x].piece.picture);
                    }
                }
            }
        }

        public void GenerateBoard()
        {
            PictureBox pictureBox = new PictureBox();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            whitePOV = !whitePOV;

            UpdateBoard(board);
        }
    }
}
