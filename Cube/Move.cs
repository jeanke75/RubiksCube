namespace Cube
{
    public sealed class Move
    {
        public CubeFace Face { get; private set; }
        public Rotation Rotation { get; private set; }

        public Move(CubeFace face, Rotation rotation)
        {
            Face = face;
            Rotation = rotation;
        }
    }
}