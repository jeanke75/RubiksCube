using System;

namespace Cube
{
    public sealed class Cubelet
    {
        public CubeletColor Front { get; private set; }
        public CubeletColor Back { get; private set; }
        public CubeletColor Left { get; private set; }
        public CubeletColor Right { get; private set; }
        public CubeletColor Top { get; private set; }
        public CubeletColor Bottom { get; private set; }

        public Cubelet(CubeletColor front, CubeletColor back, CubeletColor left, CubeletColor right, CubeletColor top, CubeletColor bottom)
        {
            if ((front != CubeletColor.Empty && back != CubeletColor.Empty) || (left != CubeletColor.Empty && right != CubeletColor.Empty) || (top != CubeletColor.Empty && bottom != CubeletColor.Empty))
                throw new Exception("Opposite sides of a cubelet can't both be colored!");

            Front = front;
            Back = back;
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public void RotateX(Rotation r)
        {
            CubeletColor tmpTop = Top;
            if (r == Rotation.Clockwise)
            {
                Top = Back;
                Back = Bottom;
                Bottom = Front;
                Front = tmpTop;
            }
            else
            {
                Top = Front;
                Front = Bottom;
                Bottom = Back;
                Back = tmpTop;
            }
        }

        public void RotateY(Rotation r)
        {
            CubeletColor tmpFront = Front;
            if (r == Rotation.Clockwise)
            {
                Front = Right;
                Right = Back;
                Back = Left;
                Left = tmpFront;
            }
            else
            {
                Front = Left;
                Left = Back;
                Back = Right;
                Right = tmpFront;
            }
        }

        public void RotateZ(Rotation r)
        {
            CubeletColor tmpTop = Top;
            if (r == Rotation.Clockwise)
            {
                Top = Left;
                Left = Bottom;
                Bottom = Right;
                Right = tmpTop;
            }
            else
            {
                Top = Right;
                Right = Bottom;
                Bottom = Left;
                Left = tmpTop;
            }
        }
    }
}