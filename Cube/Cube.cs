using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Cube
{
    public sealed class Cube : Form
    {
        private static readonly int Cubelet_Size = 25;
        private static readonly int Cubelet_Padding = 2;
        public static Random r = new Random();

        private readonly Cubelet[,,] cubelets = new Cubelet[3, 3, 3];

        public Cube()
        {
            // prepare form
            Initialize();

            // prepare cube
            Reset();
        }

        #region Interaction logic
        /**
         * Spin one of the cube's faces
         */
        public void SpinFace(Move move)
        {
            switch (move.Face)
            {
                case CubeFace.Front:
                    RotateZ(move.Rotation, 0);
                    break;
                case CubeFace.Back:
                    // inverse rotation
                    if (move.Rotation == Rotation.Clockwise)
                        RotateZ(Rotation.CounterClockwise, 2);
                    else
                        RotateZ(Rotation.Clockwise, 2);
                    break;
                case CubeFace.Left:
                    RotateX(move.Rotation, 0);
                    break;
                case CubeFace.Right:
                    // inverse rotation
                    if (move.Rotation == Rotation.Clockwise)
                        RotateX(Rotation.CounterClockwise, 2);
                    else
                        RotateX(Rotation.Clockwise, 2);
                    break;
                case CubeFace.Top:
                    RotateY(move.Rotation, 0);
                    break;
                case CubeFace.Bottom:
                    // inverse rotation
                    if (move.Rotation == Rotation.Clockwise)
                        RotateY(Rotation.CounterClockwise, 2);
                    else
                        RotateY(Rotation.Clockwise, 2);
                    break;
            }
        }

        /**
         * Scramble the cube
         */
        public void Scramble()
        {
            var moves = "F,B,L,R,U,D".Split(',');

            string lastMove3 = "";
            string lastMove2 = "";
            string lastMove = "";
            int count = 0;

            while (count != 25)
            {
                string move = moves[r.Next(0, moves.Length)];
                
                if (move == lastMove && move == lastMove2)
                {
                    // if a move has been executed 3 times already, generate a new move
                    if (move == lastMove3) continue;
                    // if a move is executed for the third time, remove 2 steps from the counter since it's just an inverse move;
                    count -= 2;
                }

                // execute move
                SpinFace(new Move(GetFace(move), Rotation.Clockwise));

                // update move history
                lastMove3 = lastMove2;
                lastMove2 = lastMove;
                lastMove = move;

                // raise the move counter
                count++;
            }
        }

        /**
         * Reset the cube to it's solved state
         */
        public void Reset()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    for (int depth = 0; depth < 3; depth++)
                    {
                        CubeletColor front = (depth == 0 ? CubeletColor.Red : CubeletColor.Empty);
                        CubeletColor back = (depth == 2 ? CubeletColor.Orange : CubeletColor.Empty);
                        CubeletColor left = (col == 0 ? CubeletColor.Blue : CubeletColor.Empty);
                        CubeletColor right = (col == 2 ? CubeletColor.Green : CubeletColor.Empty);
                        CubeletColor top = (row == 0 ? CubeletColor.Yellow : CubeletColor.Empty);
                        CubeletColor bottom = (row == 2 ? CubeletColor.White : CubeletColor.Empty);

                        cubelets[col, row, depth] = new Cubelet(front, back, left, right, top, bottom);
                    }
                }
            }
        }
        #endregion

        #region Core logic
        private int GetFaceSize()
        {
            return 3 * Cubelet_Size + 4 * Cubelet_Padding;
        }

        private CubeFace GetFace(string face)
        {
            switch (face)
            {
                case "F":
                    return CubeFace.Front;
                case "B":
                    return CubeFace.Back;
                case "L":
                    return CubeFace.Left;
                case "R":
                    return CubeFace.Right;
                case "U":
                    return CubeFace.Top;
                case "D":
                    return CubeFace.Bottom;
                default:
                    throw new Exception("Invalid face!");
            }
        }

        private void RotateX(Rotation rotation, int layer)
        {
            if (rotation == Rotation.Clockwise)
            {
                Cubelet tmpTopFront = cubelets[layer, 0, 0];
                Cubelet tmpTop = cubelets[layer, 0, 1];

                // move cubelets to correct position
                cubelets[layer, 0, 0] = cubelets[layer, 0, 2];
                cubelets[layer, 0, 1] = cubelets[layer, 1, 2];
                cubelets[layer, 0, 2] = cubelets[layer, 2, 2];
                cubelets[layer, 1, 2] = cubelets[layer, 2, 1];
                cubelets[layer, 2, 2] = cubelets[layer, 2, 0];
                cubelets[layer, 2, 1] = cubelets[layer, 1, 0];
                cubelets[layer, 2, 0] = tmpTopFront;
                cubelets[layer, 1, 0] = tmpTop;
            }
            else
            {
                Cubelet tmpTopBack = cubelets[layer, 0, 2];
                Cubelet tmpTop = cubelets[layer, 0, 1];

                // move cubelets to correct position
                cubelets[layer, 0, 2] = cubelets[layer, 0, 0];
                cubelets[layer, 0, 1] = cubelets[layer, 1, 0];
                cubelets[layer, 0, 0] = cubelets[layer, 2, 0];
                cubelets[layer, 1, 0] = cubelets[layer, 2, 1];
                cubelets[layer, 2, 0] = cubelets[layer, 2, 2];
                cubelets[layer, 2, 1] = cubelets[layer, 1, 2];
                cubelets[layer, 2, 2] = tmpTopBack;
                cubelets[layer, 1, 2] = tmpTop;
            }

            // correct cubelet rotation
            cubelets[layer, 0, 2].RotateX(rotation);
            cubelets[layer, 0, 1].RotateX(rotation);
            cubelets[layer, 0, 0].RotateX(rotation);
            cubelets[layer, 1, 0].RotateX(rotation);
            cubelets[layer, 2, 0].RotateX(rotation);
            cubelets[layer, 2, 1].RotateX(rotation);
            cubelets[layer, 2, 2].RotateX(rotation);
            cubelets[layer, 1, 2].RotateX(rotation);
        }

        private void RotateY(Rotation rotation, int layer)
        {
            if (rotation == Rotation.Clockwise)
            {
                Cubelet tmpFrontLeft= cubelets[0, layer, 0];
                Cubelet tmpFront = cubelets[1, layer, 0];

                // move cubelets to correct position
                cubelets[0, layer, 0] = cubelets[2, layer, 0];
                cubelets[1, layer, 0] = cubelets[2, layer, 1];
                cubelets[2, layer, 0] = cubelets[2, layer, 2];
                cubelets[2, layer, 1] = cubelets[1, layer, 2];
                cubelets[2, layer, 2] = cubelets[0, layer, 2];
                cubelets[1, layer, 2] = cubelets[0, layer, 1];
                cubelets[0, layer, 2] = tmpFrontLeft;
                cubelets[0, layer, 1] = tmpFront;
            }
            else
            {
                Cubelet tmpFrontRight = cubelets[2, layer, 0];
                Cubelet tmpFront = cubelets[1, layer, 0];

                // move cubelets to correct position
                cubelets[2, layer, 0] = cubelets[0, layer, 0];
                cubelets[1, layer, 0] = cubelets[0, layer, 1];
                cubelets[0, layer, 0] = cubelets[0, layer, 2];
                cubelets[0, layer, 1] = cubelets[1, layer, 2];
                cubelets[0, layer, 2] = cubelets[2, layer, 2];
                cubelets[1, layer, 2] = cubelets[2, layer, 1];
                cubelets[2, layer, 2] = tmpFrontRight;
                cubelets[2, layer, 1] = tmpFront;
            }

            // correct cubelet rotation
            cubelets[0, layer, 2].RotateY(rotation);
            cubelets[0, layer, 1].RotateY(rotation);
            cubelets[0, layer, 0].RotateY(rotation);
            cubelets[1, layer, 0].RotateY(rotation);
            cubelets[2, layer, 0].RotateY(rotation);
            cubelets[2, layer, 1].RotateY(rotation);
            cubelets[2, layer, 2].RotateY(rotation);
            cubelets[1, layer, 2].RotateY(rotation);
        }

        private void RotateZ(Rotation rotation, int layer)
        {
            if (rotation == Rotation.Clockwise)
            {
                Cubelet tmpTopRight = cubelets[2, 0, layer];
                Cubelet tmpTop = cubelets[1, 0, layer];

                // move cubelets to correct position
                cubelets[2, 0, layer] = cubelets[0, 0, layer];
                cubelets[1, 0, layer] = cubelets[0, 1, layer];
                cubelets[0, 0, layer] = cubelets[0, 2, layer];
                cubelets[0, 1, layer] = cubelets[1, 2, layer];
                cubelets[0, 2, layer] = cubelets[2, 2, layer];
                cubelets[1, 2, layer] = cubelets[2, 1, layer];
                cubelets[2, 2, layer] = tmpTopRight;
                cubelets[2, 1, layer] = tmpTop;
            }
            else
            {
                Cubelet tmpTopLeft = cubelets[0, 0, layer];
                Cubelet tmpTop = cubelets[1, 0, layer];

                // move cubelets to correct position
                cubelets[0, 0, layer] = cubelets[2, 0, layer];
                cubelets[1, 0, layer] = cubelets[2, 1, layer];
                cubelets[2, 0, layer] = cubelets[2, 2, layer];
                cubelets[2, 1, layer] = cubelets[1, 2, layer];
                cubelets[2, 2, layer] = cubelets[0, 2, layer];
                cubelets[1, 2, layer] = cubelets[0, 1, layer];
                cubelets[0, 2, layer] = tmpTopLeft;
                cubelets[0, 1, layer] = tmpTop;
            }

            // correct cubelet rotation
            cubelets[2, 0, layer].RotateZ(rotation);
            cubelets[1, 0, layer].RotateZ(rotation);
            cubelets[0, 0, layer].RotateZ(rotation);
            cubelets[0, 1, layer].RotateZ(rotation);
            cubelets[0, 2, layer].RotateZ(rotation);
            cubelets[1, 2, layer].RotateZ(rotation);
            cubelets[2, 2, layer].RotateZ(rotation);
            cubelets[2, 1, layer].RotateZ(rotation);
        }
        #endregion

        #region Drawing logic
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            Graphics g = e.Graphics;

            int face_size = GetFaceSize();

            DrawFace(g, GetFaceArray(CubeFace.Top), new Point(face_size, 0));
            DrawFace(g, GetFaceArray(CubeFace.Left), new Point(0, face_size));
            DrawFace(g, GetFaceArray(CubeFace.Front), new Point(face_size, face_size));
            DrawFace(g, GetFaceArray(CubeFace.Right), new Point(face_size * 2, face_size));
            DrawFace(g, GetFaceArray(CubeFace.Back), new Point(face_size * 3, face_size));
            DrawFace(g, GetFaceArray(CubeFace.Bottom), new Point(face_size, face_size * 2));
        }

        private void DrawFace(Graphics graphics, CubeletColor[,] face, Point faceOffset)
        {
            // draw face
            int face_size = GetFaceSize();
            graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(faceOffset, new Size(face_size, face_size)));

            // draw cubelets
            faceOffset.X += Cubelet_Padding;
            faceOffset.Y += Cubelet_Padding;

            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    Point cubeletOffset = new Point(faceOffset.X + (Cubelet_Size + Cubelet_Padding) * x, faceOffset.Y + (Cubelet_Size + Cubelet_Padding) * y);

                    Point[] points = new Point[4];
                    points[0] = new Point(cubeletOffset.X, cubeletOffset.Y);
                    points[1] = new Point(cubeletOffset.X, cubeletOffset.Y + Cubelet_Size);
                    points[2] = new Point(cubeletOffset.X + Cubelet_Size, cubeletOffset.Y + Cubelet_Size);
                    points[3] = new Point(cubeletOffset.X + Cubelet_Size, cubeletOffset.Y);
                    graphics.FillPolygon(GetColor(face[x, y]), points);
                }
            }
        }

        private CubeletColor[,] GetFaceArray(CubeFace face)
        {
            CubeletColor[,] colors = new CubeletColor[3,3];
            switch (face)
            {
                case CubeFace.Front:
                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            colors[x, y] = cubelets[x, y, 0].Front;
                        }
                    }
                    break;
                case CubeFace.Back:
                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            colors[x, y] = cubelets[2 - x, y, 2].Back;
                        }
                    }
                    break;
                case CubeFace.Left:
                    for (int y = 0; y < 3; y++)
                    {
                        for (int z = 0; z < 3; z++)
                        {
                            colors[z, y] = cubelets[0, y, 2 - z].Left;
                        }
                    }
                    break;
                case CubeFace.Right:
                    for (int y = 0; y < 3; y++)
                    {
                        for (int z = 0; z < 3; z++)
                        {
                            colors[z, y] = cubelets[2, y, z].Right;
                        }
                    }
                    break;
                case CubeFace.Top:
                    for (int z = 0; z < 3; z++)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            colors[x, z] = cubelets[x, 0, 2 - z].Top;
                        }
                    }
                    break;
                case CubeFace.Bottom:
                    for (int z = 0; z < 3; z++)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            colors[x, z] = cubelets[x, 2, z].Bottom;
                        }
                    }
                    break;
            }

            return colors;
        }

        private Brush GetColor(CubeletColor color)
        {
            switch (color)
            {
                case CubeletColor.White:
                    return new SolidBrush(Color.White);
                case CubeletColor.Yellow:
                    return new SolidBrush(Color.Yellow);
                case CubeletColor.Orange:
                    return new SolidBrush(Color.Orange);
                case CubeletColor.Red:
                    return new SolidBrush(Color.Red);
                case CubeletColor.Green:
                    return new SolidBrush(Color.Green);
                case CubeletColor.Blue:
                    return new SolidBrush(Color.Blue);
                default:
                    throw new System.Exception("Invalid cubelet color!");
            }
        }
        #endregion

        #region Form logic
        private void Initialize()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Cube));

            FlowLayoutPanel flpButtons = new FlowLayoutPanel();
            flpButtons.SuspendLayout();
            SuspendLayout();

            // create move buttons
            int buttonSize = 30;
            Padding buttonMargin = new Padding(0);
            var moves = "F,B,L,R,U,D".Split(',');
            foreach (string move in moves)
            {
                CubeFace face = GetFace(move);

                // create clockwise button
                Button btnCW = new Button
                {
                    Name = "btn" + move + "CW",
                    Size = new Size(buttonSize, buttonSize),
                    Text = move,
                    Tag = new Move(face, Rotation.Clockwise),
                    UseVisualStyleBackColor = true,
                    Margin = buttonMargin
                };
                btnCW.Click += new EventHandler(Move_Click);
                flpButtons.Controls.Add(btnCW);

                // create counter-clockwise button
                Button btnCCW = new Button
                {
                    Name = "btn" + move + "CCW",
                    Size = new Size(buttonSize, buttonSize),
                    Text = move + "\'",
                    Tag = new Move(face, Rotation.CounterClockwise),
                    UseVisualStyleBackColor = true,
                    Margin = buttonMargin
                };
                btnCCW.Click += new EventHandler(Move_Click);
                flpButtons.Controls.Add(btnCCW);
            }

            // create reset button
            Button btnReset = new Button
            {
                Name = "btnReset",
                Size = new Size(2 * buttonSize + buttonMargin.Left + buttonMargin.Right, buttonSize),
                Text = "Reset",
                UseVisualStyleBackColor = true,
                Margin = buttonMargin
            };
            btnReset.Click += Reset_Click;
            flpButtons.Controls.Add(btnReset);

            // create scramble button
            Button btnScramble = new Button
            {
                Name = "btnScramble",
                Size = new Size(2 * buttonSize + buttonMargin.Left + buttonMargin.Right, buttonSize),
                Text = "Scramble",
                UseVisualStyleBackColor = true,
                Margin = buttonMargin
            };
            btnScramble.Click += Scramble_Click;
            flpButtons.Controls.Add(btnScramble);

            // position button panel
            flpButtons.Dock = DockStyle.Right;
            flpButtons.Location = new Point(GetFaceSize() + 20, 0);
            flpButtons.Name = "flpButtons";
            flpButtons.Size = new Size(2 * buttonSize, moves.Length * buttonSize);
            flpButtons.TabIndex = 0;

            // hide border and make background transparant
            FormBorderStyle = FormBorderStyle.None;
            TransparencyKey = Color.Turquoise;
            BackColor = Color.Turquoise;

            ClientSize = new Size(GetFaceSize() * 4 + 10 + flpButtons.Width, GetFaceSize() * 3);
            Controls.Add(flpButtons);
            Name = "Cube";
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            CenterToScreen();
            MouseDown += Form_MouseDown;
            KeyDown += Form_KeyDown;

            flpButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        /**
         * Make the form draggable
         **/
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            Rotation rotation = (e.Control ? Rotation.CounterClockwise : Rotation.Clockwise);

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    break;
                case Keys.F:
                    SpinFace(new Move(CubeFace.Front, rotation));
                    break;
                case Keys.B:
                    SpinFace(new Move(CubeFace.Back, rotation));
                    break;
                case Keys.L:
                    SpinFace(new Move(CubeFace.Left, rotation));
                    break;
                case Keys.R:
                    SpinFace(new Move(CubeFace.Right, rotation));
                    break;
                case Keys.U:
                    SpinFace(new Move(CubeFace.Top, rotation));
                    break;
                case Keys.D:
                    SpinFace(new Move(CubeFace.Bottom, rotation));
                    break;
            }

            Refresh();
        }

        private void Move_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var move = btn.Tag as Move;
            SpinFace(move);

            Refresh();
        }

        private void Scramble_Click(object sender, EventArgs e)
        {
            Scramble();
            Refresh();
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            Reset();
            Refresh();
        }
        #endregion
    }
}