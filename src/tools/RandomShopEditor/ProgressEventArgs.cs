using System;

namespace RandomShopEditor
{
  public class ProgressEventArgs : EventArgs
  {
    public int CurrentItems { get; set; }
    public int TotalItems { get; set; }

    public ProgressEventArgs(int currentItems, int totalItems)
    {
      CurrentItems = currentItems;
      TotalItems = totalItems;
    }
  }
}
