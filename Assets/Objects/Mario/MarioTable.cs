static public class MarioTable
{
    public const int speedRun = 6;
    public const float speedLongJump = 11.39f;
    public static readonly float[][] dataJump = new float[][] {
        new float[] { 7.94f, 1.05f, 2.58f },  // Jump 1
        new float[] { 8.73f, 1.365f, 3.12f }, // Jump 2
        new float[] { 11.57f, 3.75f, 5.5f }   // Jump 3
    };
    public const int maxMoveJump = 3;
    public const int speedRotJump = 3;
    public const int speedRotJumpTurn = 20;
};