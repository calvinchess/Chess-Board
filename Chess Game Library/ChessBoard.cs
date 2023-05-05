using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace Chess_Game_Library
{
    public class ChessBoard
    {
        public Position[][] squares;
        public bool whiteTurn = true;
        public bool promoting = false;

        public ChessBoard()
        {
            squares = new Position[8][];
            for(int i = 0;i < 8;i ++)
            {
                squares[i] = new Position[8];
            }

            for(int i = 0;i < 8;i++)
            {
                for(int x = 0; x < 8;x++)
                {
                    squares[i][x] = new Position();
                }
            }

            squares[0][0].piece = new Rook();
            squares[0][1].piece = new Knight();
            squares[0][2].piece = new Bishop();
            squares[0][3].piece = new Queen();
            squares[0][4].piece = new King();
            squares[0][5].piece = new Bishop();
            squares[0][6].piece = new Knight();
            squares[0][7].piece = new Rook();

            squares[7][0].piece = new Rook();
            squares[7][1].piece = new Knight();
            squares[7][2].piece = new Bishop();
            squares[7][3].piece = new Queen();
            squares[7][4].piece = new King();
            squares[7][5].piece = new Bishop();
            squares[7][6].piece = new Knight();
            squares[7][7].piece = new Rook();

            for (int i = 0; i < 8; i++)
            {
                squares[1][i].piece = new Pawn();
                squares[1][i].piece.isWhite = true;

                squares[6][i].piece = new Pawn();
                squares[6][i].piece.isWhite = false;

                squares[0][i].piece.isWhite = true;
                squares[7][i].piece.isWhite = false;

                squares[0][i].piece.SetPicture();
                squares[1][i].piece.SetPicture();
                squares[6][i].piece.SetPicture();
                squares[7][i].piece.SetPicture();
            }

            bool rowStartsBlack = true;
            for (int i = 0; i < 8; i++)
            {
                bool isBlack = rowStartsBlack;
                for (int x = 0; x < 8; x++)
                {
                    squares[i][x].SetSquareColor(isBlack);
                    isBlack = !isBlack;

                    squares[i][x].y = i;
                    squares[i][x].x = x;

                    squares[i][x].board = this;
                    if (squares[i][x].piece != null)
                    {
                        squares[i][x].piece.board = this;
                        squares[i][x].piece.position = squares[i][x];
                    }
                }
                rowStartsBlack = !rowStartsBlack;
            }
        }

        public void ResetBackgroundColors()
        {
            bool rowStartsBlack = true;
            for (int i = 0; i < 8; i++)
            {
                bool isBlack = rowStartsBlack;
                for (int x = 0; x < 8; x++)
                {
                    squares[i][x].SetSquareColor(isBlack);
                    isBlack = !isBlack;
                }
                rowStartsBlack = !rowStartsBlack;
            }
        }

        public void PromoteTo(ChessPiece oldPiece, ChessPiece newPiece)
        {
            this.promoting = false;

            oldPiece.position.piece = newPiece;

            newPiece.position = oldPiece.position;
            newPiece.board = this;
            newPiece.isWhite = oldPiece.isWhite;
            newPiece.picture = oldPiece.picture;

            newPiece.picture.Click -= oldPiece.onclick;
            newPiece.picture.Click += newPiece.onclick;

            if (newPiece is Rook rook) rook.hasMoved = true;

            newPiece.SetPicture();

            newPiece.position.UpdatePicture(newPiece.position.background.Location.X, newPiece.position.background.Location.Y, newPiece.position.background.Size.Width);

            newPiece.picture.BringToFront();
            newPiece.picture.Refresh();

            MessageBox.Show((squares[7][7].piece != null && squares[7][7].piece is Queen) + "");
        }

        public ChessBoard CreateCopy()
        {
            ChessBoard copy = new ChessBoard();
            for(int i = 0;i < 8;i++)
            {
                for(int x = 0;x < 8;x++)
                {
                    if (squares[i][x].piece != null)
                    {
                        if (squares[i][x].piece is Rook) copy.squares[i][x].piece = new Rook();
                        if (squares[i][x].piece is King) copy.squares[i][x].piece = new King();
                        if (squares[i][x].piece is Queen) copy.squares[i][x].piece = new Queen();
                        if (squares[i][x].piece is Knight) copy.squares[i][x].piece = new Knight();
                        if (squares[i][x].piece is Pawn) copy.squares[i][x].piece = new Pawn();
                        if (squares[i][x].piece is Bishop) copy.squares[i][x].piece = new Bishop();

                        copy.squares[i][x].piece.isWhite = squares[i][x].piece.isWhite;
                        copy.squares[i][x].piece.board = copy;
                        copy.squares[i][x].piece.position = copy.squares[i][x];
                    }
                    else
                    {
                        copy.squares[i][x].piece = null;
                    }
                }
            }

            copy.whiteTurn = whiteTurn;

            return copy;
        }

        public void MovePiece(Position oldPosition, Position newPosition, bool doPromote = true)
        {
            if (oldPosition.piece == null) return;
            if (whiteTurn != oldPosition.piece.isWhite) return;
            if (promoting) return;

            whiteTurn = !whiteTurn;
            
            if(newPosition != null && newPosition.piece != null && newPosition.piece.picture != null)
            {
                newPosition.piece.picture.Visible = false;
            }

            if (newPosition.piece == null && oldPosition.piece is Pawn && Math.Abs(oldPosition.x - newPosition.x) > 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        if (squares[i][x].piece != null && squares[i][x].piece is Pawn enPessantPawn && enPessantPawn.enpessantable)
                        {
                            squares[i][x].piece.picture.Visible = false;
                            squares[i][x].piece = null;
                            squares[i][x].UpdatePicture(squares[i][x].background.Location.X, squares[i][x].background.Location.Y, squares[i][x].background.Size.Width);
                        }
                    }
                }
            }

            for (int i = 0;i < 8;i++)
            {
                for(int x = 0; x < 8;x++)
                {
                    if (squares[i][x].piece != null && squares[i][x].piece is Pawn randomPawn)
                    {
                        randomPawn.enpessantable = false;
                    }
                }
            }

            newPosition.piece = oldPosition.piece;
            newPosition.piece.position = newPosition;

            if(newPosition.piece is King && newPosition.x - oldPosition.x == 2)
            {
                squares[newPosition.y][5].piece = squares[newPosition.y][7].piece;
                squares[newPosition.y][7].piece = null;
                squares[newPosition.y][5].UpdatePicture(squares[newPosition.y][5].background.Location.X, squares[newPosition.y][5].background.Location.Y, squares[newPosition.y][5].background.Size.Width);
                squares[newPosition.y][5].piece.position = squares[newPosition.y][5];
            }

            if (newPosition.piece is King && newPosition.x - oldPosition.x == -2)
            {
                squares[newPosition.y][3].piece = squares[newPosition.y][0].piece;
                squares[newPosition.y][0].piece = null;
                squares[newPosition.y][3].UpdatePicture(squares[newPosition.y][3].background.Location.X, squares[newPosition.y][3].background.Location.Y, squares[newPosition.y][3].background.Size.Width);
                squares[newPosition.y][3].piece.position = squares[newPosition.y][3];
            }

            if (newPosition.piece is Pawn && ((newPosition.y == 7 && newPosition.piece.isWhite) || (newPosition.y == 0 && !newPosition.piece.isWhite)) && doPromote)
            {
                promoting = true;
                Promotion_Form newForm = new Promotion_Form();

                newForm.LoadPromotion(this, newPosition.piece);

                newForm.Show();
            }

            oldPosition.piece = null;

            newPosition.UpdatePicture(newPosition.background.Location.X, newPosition.background.Location.Y, newPosition.background.Size.Width);
            oldPosition.UpdatePicture(oldPosition.background.Location.X, oldPosition.background.Location.Y, oldPosition.background.Size.Width);

            if (newPosition.piece is Rook rook) rook.hasMoved = true;
            if (newPosition.piece is King king) king.hasMoved = true;
            if (newPosition.piece is Pawn pawn && Math.Abs(newPosition.y - oldPosition.y) == 2)
            {
                pawn.enpessantable = true;
            }
        }

        public List<Position> GetLegalMovesOfSide(bool isWhiteSide)
        {
            List<Position> allLegalMoves = new List<Position>();

            for(int i = 0; i < 8; i++)
            {
                for(int x = 0; x < 8;x++)
                {
                    if (squares[i][x].piece != null && squares[i][x].piece.isWhite == isWhiteSide)
                    {
                        allLegalMoves.AddRange(squares[i][x].piece.GetLegalMoves(false));
                    }
                }
            }

            return allLegalMoves;
        }

        public bool IsInCheck(bool isWhiteSide)
        {
            Position kingPosition = null;

            for(int i = 0; i < 8;i++)
            {
                for(int x = 0; x < 8; x++)
                {
                    if (squares[i][x].piece != null && squares[i][x].piece is King && squares[i][x].piece.isWhite == isWhiteSide)
                    {
                        kingPosition = squares[i][x];
                    }
                }
            }

            return GetLegalMovesOfSide(!isWhiteSide).Contains(kingPosition);
        }
    }

    public class Position
    {
        public ChessBoard board;
        public PictureBox background;
        public ChessPiece piece;
        public int y;
        public int x;

        public void UpdatePicture(int x, int y, int size) 
        {
            if (background == null)
            {
                background = new PictureBox();
                background.Click += onclick;
            }

            background.Location = new Point(x, y);
            background.Size = new Size(size, size);
            background.BorderStyle = BorderStyle.FixedSingle;

            if(piece != null && piece.picture != null)
            {
                piece.picture.Location = new Point(x + 1, y + 1);
                piece.picture.Size = new Size(size - 2, size - 2);
                piece.picture.SizeMode = PictureBoxSizeMode.StretchImage;

                piece.picture.BackColor = background.BackColor;

                piece.picture.BringToFront();
                piece.picture.Refresh();
            }
        }

        public void SetSquareColor(bool isDark)
        {
            if (background == null)
            {
                background = new PictureBox();
                background.Click += onclick;
            }

            if (isDark) background.BackColor = Color.DarkOliveGreen;
            else background.BackColor = Color.White;

            if (piece != null)
            {
                piece.picture.BackColor = background.BackColor;
                piece.picture.Refresh();
            }

            background.Refresh();
        }

        public void onclick(object sender, EventArgs e)
        {
            if(background.BackColor == Color.LightGreen)
            {
                ChessPiece highlightedPiece = null;
                for (int i = 0; i < 8; i++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        if (board.squares[i][x].piece != null && board.squares[i][x].piece.picture.BackColor == Color.Yellow)
                        {
                            highlightedPiece = board.squares[i][x].piece;
                            break;
                        }
                    }
                }

                board.MovePiece(highlightedPiece.position, this);
            }

            this.board.ResetBackgroundColors();
        }
    }

    public class ChessPiece
    {
        public bool isWhite;
        public PictureBox picture;
        public ChessBoard board;
        public Position position;
        
        public virtual void SetPicture() 
        {
            if (picture == null)
            {
                picture = new PictureBox();
                picture.Click += onclick;
            }
        }

        public void onclick(object sender, EventArgs e)
        {
            if (!(this.picture.BackColor == Color.LightGreen || this.picture.BackColor == Color.Yellow))
            {
                this.board.ResetBackgroundColors();

                List<Position> legalMoves = GetLegalMoves();
                foreach(Position move in legalMoves)
                {
                    move.background.BackColor = Color.LightGreen;
                    if (move.piece != null)
                    {
                        move.piece.picture.BackColor = Color.LightGreen;
                        move.piece.picture.Refresh();
                    }
                }

                
                this.picture.BackColor = Color.Yellow;
            }
            else if(this.picture.BackColor == Color.Yellow)
            {
                this.board.ResetBackgroundColors();
            }
            else
            {
                ChessPiece highlightedPiece = null;
                for(int i = 0;i < 8;i++)
                {
                    for(int x = 0; x < 8;x++)
                    {
                        if (board.squares[i][x].piece != null && board.squares[i][x].piece.picture.BackColor == Color.Yellow)
                        {
                            highlightedPiece = board.squares[i][x].piece;
                            break;
                        }
                    }
                }

                board.MovePiece(highlightedPiece.position, this.position);

                this.board.ResetBackgroundColors();
            }
        }

        public virtual List<Position> GetLegalMoves(bool checkPins = true)
        {
            return new List<Position>();
        }

        public void ReduceLegalMovesDueToPins(List<Position> moves)
        {
            //MessageBox.Show("Pin Check, isWhite = " + this.isWhite);
            for(int i = moves.Count - 1; i >= 0; i--)
            {
                ChessBoard copyBoard = board.CreateCopy();

                copyBoard.MovePiece(copyBoard.squares[this.position.y][this.position.x], copyBoard.squares[moves[i].y][moves[i].x], false);

                if (copyBoard.IsInCheck(this.isWhite)) moves.Remove(moves[i]);
            }
        }
    }

    public class Pawn : ChessPiece
    {
        public bool enpessantable = false;
        public override void SetPicture()
        {
            base.SetPicture();

            if (this.isWhite) picture.ImageLocation = @"C:\Personal Projects\Chess Game Library\Chess Game Library\Images\white pawn.png";
            else picture.ImageLocation = @"C:\Personal Projects\Chess Game Library\Chess Game Library\Images\black pawn.png";
        }
        public override List<Position> GetLegalMoves(bool checkPins = true)
        {
            List<Position> legalMoves = new List<Position>();

            if (board == null) return legalMoves;

            if (this.isWhite)
            {
                if (position.y < 7 && board.squares[position.y + 1][position.x].piece == null)
                {
                    legalMoves.Add(board.squares[position.y + 1][position.x]);
                    if(position.y == 1 && board.squares[position.y + 2][position.x].piece == null)
                    {
                        legalMoves.Add(board.squares[position.y + 2][position.x]);
                    }
                }

                if (position.y < 7 && position.x < 7 && board.squares[position.y + 1][position.x + 1].piece != null && board.squares[position.y + 1][position.x + 1].piece.isWhite != isWhite)
                {
                    legalMoves.Add(board.squares[position.y + 1][position.x + 1]);
                }
                if (position.y < 7 && position.x > 0 && board.squares[position.y + 1][position.x - 1].piece != null && board.squares[position.y + 1][position.x - 1].piece.isWhite != isWhite)
                {
                    legalMoves.Add(board.squares[position.y + 1][position.x - 1]);
                }

                //en pessant
                if (position.y == 4 && position.x < 7 && board.squares[position.y][position.x + 1].piece != null && board.squares[position.y][position.x + 1].piece is Pawn rightPawn && rightPawn.enpessantable)
                {
                    legalMoves.Add(board.squares[position.y + 1][position.x + 1]);
                }

                if (position.y == 4 && position.x > 0 && board.squares[position.y][position.x - 1].piece != null && board.squares[position.y][position.x - 1].piece is Pawn leftPawn && leftPawn.enpessantable)
                {
                    legalMoves.Add(board.squares[position.y + 1][position.x - 1]);
                }
            }
            else
            {
                if (position.y > 0 && board.squares[position.y - 1][position.x].piece == null)
                {
                    legalMoves.Add(board.squares[position.y - 1][position.x]);
                    if (position.y == 6 && board.squares[position.y - 2][position.x].piece == null)
                    {
                        legalMoves.Add(board.squares[position.y - 2][position.x]);
                    }
                }

                if (position.y > 0 && position.x < 7 && board.squares[position.y - 1][position.x + 1].piece != null && board.squares[position.y - 1][position.x + 1].piece.isWhite != isWhite)
                {
                    legalMoves.Add(board.squares[position.y - 1][position.x + 1]);
                }
                if (position.y > 0 && position.x > 0 && board.squares[position.y - 1][position.x - 1].piece != null && board.squares[position.y - 1][position.x - 1].piece.isWhite != isWhite)
                {
                    legalMoves.Add(board.squares[position.y - 1][position.x - 1]);
                }

                //en pessant
                if (position.y == 3 && position.x < 7 && board.squares[position.y][position.x + 1].piece != null && board.squares[position.y][position.x + 1].piece is Pawn rightPawn && rightPawn.enpessantable)
                {
                    legalMoves.Add(board.squares[position.y - 1][position.x + 1]);
                }

                if (position.y == 3 && position.x > 0 && board.squares[position.y][position.x - 1].piece != null && board.squares[position.y][position.x - 1].piece is Pawn leftPawn && leftPawn.enpessantable)
                {
                    legalMoves.Add(board.squares[position.y - 1][position.x - 1]);
                }
            }

            if (checkPins)
            {
                ReduceLegalMovesDueToPins(legalMoves);
            }

            return legalMoves;
        }
    }

    public class King : ChessPiece
    {
        public bool hasMoved = false;
        public override void SetPicture()
        {
            base.SetPicture();

            if (this.isWhite) picture.ImageLocation = @"C:\Personal Projects\Chess Game Library\Chess Game Library\Images\white king.png";
            else picture.ImageLocation = @"C:\Personal Projects\Chess Game Library\Chess Game Library\Images\black king.png";
        }

        public override List<Position> GetLegalMoves(bool checkPins = true)
        {
            List<Position> legalMoves = new List<Position>();

            //normal moving
            for(int y = position.y - 1; y <= position.y + 1; y++)
            {
                for(int x = position.x - 1; x <= position.x + 1;x++)
                {
                    if (!(position.x == x && position.y == y))
                    {
                        if (x >= 0 && y >= 0 && x < 8 && y < 8)
                        {
                            if (board.squares[y][x].piece == null || board.squares[y][x].piece.isWhite != isWhite)
                            {
                                legalMoves.Add(board.squares[y][x]);
                            }
                        }
                    }                   
                }
            }

            if (checkPins)
            {
                List<Position> potentialOtherSideMoves = board.GetLegalMovesOfSide(!isWhite);


                //castling
                if (hasMoved == false)
                {
                    if (board.squares[position.y][position.x + 1].piece == null && board.squares[position.y][position.x + 2].piece == null)
                    {
                        if (board.squares[position.y][7].piece != null && board.squares[position.y][7].piece is Rook rook && rook.hasMoved == false)
                        {
                            if (!(potentialOtherSideMoves.Contains(this.position) || potentialOtherSideMoves.Contains(board.squares[position.y][position.x + 1]) || potentialOtherSideMoves.Contains(board.squares[position.y][position.x + 2])))
                            {
                                legalMoves.Add(board.squares[position.y][position.x + 2]);
                            }
                        }
                    }

                    if (board.squares[position.y][position.x - 1].piece == null && board.squares[position.y][position.x - 2].piece == null && board.squares[position.y][position.x - 3].piece == null)
                    {
                        if (board.squares[position.y][0].piece != null && board.squares[position.y][0].piece is Rook rook && rook.hasMoved == false)
                        {
                            if (!(potentialOtherSideMoves.Contains(this.position) || potentialOtherSideMoves.Contains(board.squares[position.y][position.x - 1]) || potentialOtherSideMoves.Contains(board.squares[position.y][position.x - 2])))
                            {
                                legalMoves.Add(board.squares[position.y][position.x - 2]);
                            }
                        }
                    }
                }
            }

            if (checkPins) ReduceLegalMovesDueToPins(legalMoves);

            return legalMoves;
        }
    }

    public class Queen : ChessPiece
    {
        public override void SetPicture()
        {
            base.SetPicture();

            if (this.isWhite) picture.ImageLocation = @"C:\Personal Projects\Chess Game Library\Chess Game Library\Images\white queen.png";
            else picture.ImageLocation = @"C:\Personal Projects\Chess Game Library\Chess Game Library\Images\black queen.png";
        }

        public override List<Position> GetLegalMoves(bool checkPins = true)
        {
            List<Position> legalMoves = new List<Position>();

            for (int i = position.x + 1; i < 8; i++)
            {
                if (board.squares[position.y][i].piece == null) legalMoves.Add(board.squares[position.y][i]);
                else if (board.squares[position.y][i].piece.isWhite != this.isWhite)
                {
                    legalMoves.Add(board.squares[position.y][i]);
                    break;
                }
                else
                {
                    break;
                }
            }

            for (int i = position.x - 1; i >= 0; i--)
            {
                if (board.squares[position.y][i].piece == null) legalMoves.Add(board.squares[position.y][i]);
                else if (board.squares[position.y][i].piece.isWhite != this.isWhite)
                {
                    legalMoves.Add(board.squares[position.y][i]);
                    break;
                }
                else
                {
                    break;
                }
            }

            for (int i = position.y - 1; i >= 0; i--)
            {
                if (board.squares[i][position.x].piece == null) legalMoves.Add(board.squares[i][position.x]);
                else if (board.squares[i][position.x].piece.isWhite != this.isWhite)
                {
                    legalMoves.Add(board.squares[i][position.x]);
                    break;
                }
                else
                {
                    break;
                }
            }

            for (int i = position.y + 1; i < 8; i++)
            {
                if (board.squares[i][position.x].piece == null) legalMoves.Add(board.squares[i][position.x]);
                else if (board.squares[i][position.x].piece.isWhite != this.isWhite)
                {
                    legalMoves.Add(board.squares[i][position.x]);
                    break;
                }
                else
                {
                    break;
                }
            }


            int y = position.y + 1;
            for (int x = position.x + 1; x < 8 && y < 8; y++, x++)
            {
                if (board.squares[y][x].piece == null) legalMoves.Add(board.squares[y][x]);
                else if (board.squares[y][x].piece.isWhite != this.isWhite)
                {
                    legalMoves.Add(board.squares[y][x]);
                    break;
                }
                else
                {
                    break;
                }
            }

            y = position.y - 1;
            for (int x = position.x + 1; x < 8 && y >= 0; y--, x++)
            {
                if (board.squares[y][x].piece == null) legalMoves.Add(board.squares[y][x]);
                else if (board.squares[y][x].piece.isWhite != this.isWhite)
                {
                    legalMoves.Add(board.squares[y][x]);
                    break;
                }
                else
                {
                    break;
                }
            }

            y = position.y + 1;
            for (int x = position.x - 1; x >= 0 && y < 8; y++, x--)
            {
                if (board.squares[y][x].piece == null) legalMoves.Add(board.squares[y][x]);
                else if (board.squares[y][x].piece.isWhite != this.isWhite)
                {
                    legalMoves.Add(board.squares[y][x]);
                    break;
                }
                else
                {
                    break;
                }
            }

            y = position.y - 1;
            for (int x = position.x - 1; x >= 0 && y >= 0; y--, x--)
            {
                if (board.squares[y][x].piece == null) legalMoves.Add(board.squares[y][x]);
                else if (board.squares[y][x].piece.isWhite != this.isWhite)
                {
                    legalMoves.Add(board.squares[y][x]);
                    break;
                }
                else
                {
                    break;
                }
            }



            if (checkPins) ReduceLegalMovesDueToPins(legalMoves);

            return legalMoves;
        }
    }

    public class Bishop : ChessPiece
    {
        public override void SetPicture()
        {
            base.SetPicture();

            if (this.isWhite) picture.ImageLocation = @"C:\Personal Projects\Chess Game Library\Chess Game Library\Images\white bishop.png";
            else picture.ImageLocation = @"C:\Personal Projects\Chess Game Library\Chess Game Library\Images\black bishop.png";
        }

        public override List<Position> GetLegalMoves(bool checkPins = true)
        {
            List<Position> legalMoves = new List<Position>();

            int y = position.y + 1;
            for (int x = position.x + 1; x < 8 && y < 8; y++, x++)
            {
                if (board.squares[y][x].piece == null) legalMoves.Add(board.squares[y][x]);
                else if (board.squares[y][x].piece.isWhite != this.isWhite)
                {
                    legalMoves.Add(board.squares[y][x]);
                    break;
                }
                else
                {
                    break;
                }
            }

            y = position.y - 1;
            for (int x = position.x + 1; x < 8 && y >= 0; y--, x++)
            {
                if (board.squares[y][x].piece == null) legalMoves.Add(board.squares[y][x]);
                else if (board.squares[y][x].piece.isWhite != this.isWhite)
                {
                    legalMoves.Add(board.squares[y][x]);
                    break;
                }
                else
                {
                    break;
                }
            }

            y = position.y + 1;
            for (int x = position.x - 1; x >= 0 && y < 8; y++, x--)
            {
                if (board.squares[y][x].piece == null) legalMoves.Add(board.squares[y][x]);
                else if (board.squares[y][x].piece.isWhite != this.isWhite)
                {
                    legalMoves.Add(board.squares[y][x]);
                    break;
                }
                else
                {
                    break;
                }
            }

            y = position.y - 1;
            for (int x = position.x - 1; x >= 0 && y >= 0; y--, x--)
            {
                if (board.squares[y][x].piece == null) legalMoves.Add(board.squares[y][x]);
                else if (board.squares[y][x].piece.isWhite != this.isWhite)
                {
                    legalMoves.Add(board.squares[y][x]);
                    break;
                }
                else
                {
                    break;
                }
            }



            if (checkPins) ReduceLegalMovesDueToPins(legalMoves);

            return legalMoves;
        }
    }

    public class Knight : ChessPiece
    {
        public override void SetPicture()
        {
            base.SetPicture();

            if (this.isWhite) picture.ImageLocation = @"C:\Personal Projects\Chess Game Library\Chess Game Library\Images\white knight.png";
            else picture.ImageLocation = @"C:\Personal Projects\Chess Game Library\Chess Game Library\Images\black knight.png";
        }

        public override List<Position> GetLegalMoves(bool checkPins = true)
        {
            List<Position> legalMoves = new List<Position>();

            if(position.x > 1 && position.y > 0 && (board.squares[position.y - 1][position.x - 2].piece == null || board.squares[position.y - 1][position.x - 2].piece.isWhite != isWhite))
            {
                legalMoves.Add(board.squares[position.y - 1][position.x - 2]);
            }

            if (position.x > 0 && position.y > 1 && (board.squares[position.y - 2][position.x - 1].piece == null || board.squares[position.y - 2][position.x - 1].piece.isWhite != isWhite))
            {
                legalMoves.Add(board.squares[position.y - 2][position.x - 1]);
            }

            if (position.x > 1 && position.y < 7 && (board.squares[position.y + 1][position.x - 2].piece == null || board.squares[position.y + 1][position.x - 2].piece.isWhite != isWhite))
            {
                legalMoves.Add(board.squares[position.y + 1][position.x - 2]);
            }

            if (position.x < 7 && position.y > 1 && (board.squares[position.y - 2][position.x + 1].piece == null || board.squares[position.y - 2][position.x + 1].piece.isWhite != isWhite))
            {
                legalMoves.Add(board.squares[position.y - 2][position.x + 1]);
            }

            if (position.x < 7 && position.y < 6 && (board.squares[position.y + 2][position.x + 1].piece == null || board.squares[position.y + 2][position.x + 1].piece.isWhite != isWhite))
            {
                legalMoves.Add(board.squares[position.y + 2][position.x + 1]);
            }

            if (position.x > 0 && position.y < 6 && (board.squares[position.y + 2][position.x - 1].piece == null || board.squares[position.y + 2][position.x - 1].piece.isWhite != isWhite))
            {
                legalMoves.Add(board.squares[position.y + 2][position.x - 1]);
            }

            if (position.x < 6 && position.y < 7 && (board.squares[position.y + 1][position.x + 2].piece == null || board.squares[position.y + 1][position.x + 2].piece.isWhite != isWhite))
            {
                legalMoves.Add(board.squares[position.y + 1][position.x + 2]);
            }

            if (position.x < 6 && position.y > 0 && (board.squares[position.y - 1][position.x + 2].piece == null || board.squares[position.y - 1][position.x + 2].piece.isWhite != isWhite))
            {
                legalMoves.Add(board.squares[position.y - 1][position.x + 2]);
            }

            if (checkPins) ReduceLegalMovesDueToPins(legalMoves);

            return legalMoves;
        }
    }

    public class Rook : ChessPiece 
    {
        public bool hasMoved = false;
        public override void SetPicture()
        {
            base.SetPicture();

            if (this.isWhite) picture.ImageLocation = @"C:\Personal Projects\Chess Game Library\Chess Game Library\Images\white rook.png";
            else picture.ImageLocation = @"C:\Personal Projects\Chess Game Library\Chess Game Library\Images\black rook.png";
        }

        public override List<Position> GetLegalMoves(bool checkPins = true)
        {
            List<Position> legalMoves = new List<Position>();

            for(int i = position.x + 1; i < 8;i++)
            {
                if (board.squares[position.y][i].piece == null) legalMoves.Add(board.squares[position.y][i]);
                else if (board.squares[position.y][i].piece.isWhite != this.isWhite)
                {
                    legalMoves.Add(board.squares[position.y][i]);
                    break;
                }
                else
                {
                    break;
                }
            }

            for (int i = position.x - 1; i >=0; i--)
            {
                if (board.squares[position.y][i].piece == null) legalMoves.Add(board.squares[position.y][i]);
                else if (board.squares[position.y][i].piece.isWhite != this.isWhite)
                {
                    legalMoves.Add(board.squares[position.y][i]);
                    break;
                }
                else
                {
                    break;
                }
            }

            for (int i = position.y - 1; i >= 0; i--)
            {
                if (board.squares[i][position.x].piece == null) legalMoves.Add(board.squares[i][position.x]);
                else if (board.squares[i][position.x].piece.isWhite != this.isWhite)
                {
                    legalMoves.Add(board.squares[i][position.x]);
                    break;
                }
                else
                {
                    break;
                }
            }

            for (int i = position.y + 1; i < 8; i++)
            {
                if (board.squares[i][position.x].piece == null) legalMoves.Add(board.squares[i][position.x]);
                else if (board.squares[i][position.x].piece.isWhite != this.isWhite)
                {
                    legalMoves.Add(board.squares[i][position.x]);
                    break;
                }
                else
                {
                    break;
                }
            }

            if(checkPins) ReduceLegalMovesDueToPins(legalMoves);

            return legalMoves;
        }
    }
}
