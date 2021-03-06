﻿namespace Grove
{
  public enum CardColor
  {
    White = 0,
    Blue = 1,
    Black = 2,
    Red = 3,
    Green = 4,
    Colorless = 5,
    None = 6,
  }

  public static class CardColors
  {
    public static readonly CardColor[] All = new[]
      {
        CardColor.White,
        CardColor.Blue,
        CardColor.Black,
        CardColor.Red,
        CardColor.Green,
      };
  }
  
}