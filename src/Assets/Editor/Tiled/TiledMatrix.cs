namespace Assets.Editor.Tiled
{
  public class Matrix<T>
  {
    private T[] _items;

    public Matrix(T[] items, int rows, int columns)
    {
      _items = items;
      Rows = rows;
      Columns = columns;
    }

    public T this[int index] { get { return _items[index]; } }

    public int Length { get { return _items.Length; } }

    public int Rows { get; private set; }

    public int Columns { get; private set; }

    public T GetItem(int rowIndex, int columnIndex)
    {
      return _items[rowIndex * Columns + columnIndex];
    }
  }
}