using System.Collections.Generic;
using System.Linq;

public class CustomStack<T> : List<T>
{
  public void Push(T obj)
  {
    Add(obj);
  }

  public T Pop()
  {
    if (Count > 0)
    {
      T item = this[Count - 1];

      Remove(item);

      return item;
    }

    return default(T);
  }

  public T Peek()
  {
    if (Count > 0)
    {
      return this[Count - 1];
    }

    return default(T);
  }

  public T Exchange(int index, T item)
  {
    if (index < Count)
    {
      T removedItem = this[index];

      Insert(index, item);

      RemoveAt(index + 1);

      return removedItem;
    }

    return default(T);
  }

  public override string ToString()
  {
    if (!this.Any())
    {
      return "STACK IS EMPTY";
    }

    if (Count == 1)
    {
      return this.First().ToString();
    }

    return this.Select(item => item.ToString())
      .Aggregate((i, j) => i + ", " + j);
  }
}
