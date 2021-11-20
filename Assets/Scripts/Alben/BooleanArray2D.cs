/// <summary>
/// This script serves as a 2D array for boolean variables.
/// It can be used for objects like the script for the grid
/// of disappearing platforms.
/// Resources: https://youtu.be/uoHc-Lz9Lsc
/// https://docs.unity3d.com/Manual/editor-PropertyDrawers.html
/// Author: Alben Trang
/// </summary>
[System.Serializable]
public class BooleanArray2D
{
    private static int tempRows = 1;

    /// <summary>
    /// Class to hold rows and columns of 2D array
    /// </summary>
    [System.Serializable]
    public class BooleanArray2DInfo
    {
        public int rows;
        public int columns;

        public BooleanArray2DInfo()
        {
            tempRows = rows;
        }
    }

    public BooleanArray2DInfo info = new BooleanArray2DInfo();

    /// <summary>
    /// Creates a struct to hold a row of bools
    /// </summary>
    [System.Serializable]
    public struct Bool2D
    {
        public bool[] boolArray;
    }
    
    // Initialize the rows for the 2D array
    public Bool2D[] booleanArrays = new Bool2D[tempRows];
}
