using System;
using System.Diagnostics;
using Microsoft.VisualBasic;

namespace SharpFlame
{
	public sealed class TileOrientation
	{
		
		public static sTileOrientation Orientation_Clockwise = new sTileOrientation(true, false, true);
		public static sTileOrientation Orientation_CounterClockwise = new sTileOrientation(false, true, true);
		public static sTileOrientation Orientation_FlipX = new sTileOrientation(true, false, false);
		public static sTileOrientation Orientation_FlipY = new sTileOrientation(false, true, false);
		
		public struct sTileOrientation
		{
			public bool ResultXFlip;
			public bool ResultYFlip;
			public bool SwitchedAxes;
			
			public sTileOrientation(bool ResultXFlip, bool ResultZFlip, bool SwitchedAxes)
			{
				
				this.ResultXFlip = ResultXFlip;
				this.ResultYFlip = ResultZFlip;
				this.SwitchedAxes = SwitchedAxes;
			}
			
			public sTileOrientation GetRotated(sTileOrientation Orientation)
			{
				sTileOrientation ReturnResult = new sTileOrientation();
				
				ReturnResult.SwitchedAxes = SwitchedAxes ^ Orientation.SwitchedAxes;
				
				if (Orientation.SwitchedAxes)
				{
					if (Orientation.ResultXFlip)
					{
						ReturnResult.ResultXFlip = !ResultYFlip;
					}
					else
					{
						ReturnResult.ResultXFlip = ResultYFlip;
					}
					if (Orientation.ResultYFlip)
					{
						ReturnResult.ResultYFlip = !ResultXFlip;
					}
					else
					{
						ReturnResult.ResultYFlip = ResultXFlip;
					}
				}
				else
				{
					if (Orientation.ResultXFlip)
					{
						ReturnResult.ResultXFlip = !ResultXFlip;
					}
					else
					{
						ReturnResult.ResultXFlip = ResultXFlip;
					}
					if (Orientation.ResultYFlip)
					{
						ReturnResult.ResultYFlip = !ResultYFlip;
					}
					else
					{
						ReturnResult.ResultYFlip = ResultYFlip;
					}
				}
				
				return ReturnResult;
			}
			
			public void Reverse()
			{
				
				if (SwitchedAxes)
				{
					if (ResultXFlip ^ ResultYFlip)
					{
						ResultXFlip = !ResultXFlip;
						ResultYFlip = !ResultYFlip;
					}
				}
			}
			
			public void RotateClockwise()
			{
				
				SwitchedAxes = !SwitchedAxes;
				if (ResultXFlip ^ ResultYFlip)
				{
					ResultYFlip = !ResultYFlip;
				}
				else
				{
					ResultXFlip = !ResultXFlip;
				}
			}
			
			public void RotateAnticlockwise()
			{
				
				SwitchedAxes = !SwitchedAxes;
				if (ResultXFlip ^ ResultYFlip)
				{
					ResultXFlip = !ResultXFlip;
				}
				else
				{
					ResultYFlip = !ResultYFlip;
				}
			}
		}
		
		public struct sTileDirection
		{
			public byte X; //0-2, 1=middle
			public byte Y; //0-2, 1=middle
			
			public sTileDirection(byte NewX, byte NewY)
			{
				
				X = NewX;
				Y = NewY;
			}
			
			public sTileDirection GetRotated(sTileOrientation Orientation)
			{
				sTileDirection ReturnResult = new sTileDirection();
				
				if (Orientation.SwitchedAxes)
				{
					if (Orientation.ResultXFlip)
					{
						ReturnResult.X = (byte)(2 - Y);
					}
					else
					{
						ReturnResult.X = Y;
					}
					if (Orientation.ResultYFlip)
					{
						ReturnResult.Y = (byte)(2 - X);
					}
					else
					{
						ReturnResult.Y = X;
					}
				}
				else
				{
					if (Orientation.ResultXFlip)
					{
						ReturnResult.X = (byte)(2 - X);
					}
					else
					{
						ReturnResult.X = X;
					}
					if (Orientation.ResultYFlip)
					{
						ReturnResult.Y = (byte)(2 - Y);
					}
					else
					{
						ReturnResult.Y = Y;
					}
				}
				
				return ReturnResult;
			}
			
			public void FlipX()
			{
				
				X = (byte)(2 - X);
			}
			
			public void FlipY()
			{
				
				Y = (byte)(2 - Y);
			}
			
			public void RotateClockwise()
			{
				byte byteTemp = 0;
				
				byteTemp = X;
				X = (byte)(2 - Y);
				Y = byteTemp;
			}
			
			public void RotateAnticlockwise()
			{
				byte byteTemp = 0;
				
				byteTemp = X;
				X = Y;
				Y = (byte)(2 - byteTemp);
			}
			
			public void SwitchAxes()
			{
				byte byteTemp = 0;
				
				byteTemp = X;
				X = Y;
				Y = byteTemp;
			}
		}
		
		public static sTileDirection TileDirection_TopLeft = new sTileDirection(0, 0);
		public static sTileDirection TileDirection_Top = new sTileDirection(1, 0);
		public static sTileDirection TileDirection_TopRight = new sTileDirection(2, 0);
		public static sTileDirection TileDirection_Right = new sTileDirection(2, 1);
		public static sTileDirection TileDirection_BottomRight = new sTileDirection(2, 2);
		public static sTileDirection TileDirection_Bottom = new sTileDirection(1, 2);
		public static sTileDirection TileDirection_BottomLeft = new sTileDirection(0, 2);
		public static sTileDirection TileDirection_Left = new sTileDirection(0, 1);
		public static sTileDirection TileDirection_None = new sTileDirection(1, 1);
		
		public static clsMap.clsTerrain.Tile.sTexture OrientateTile(clsPainter.clsTileList.sTileOrientationChance TileChance, sTileDirection NewDirection)
		{
			clsMap.clsTerrain.Tile.sTexture ReturnResult = new clsMap.clsTerrain.Tile.sTexture();
			
			//use random for empty tiles
			if (TileChance.TextureNum < 0)
			{
				ReturnResult.Orientation.ResultXFlip = VBMath.Rnd() >= 0.5F;
				ReturnResult.Orientation.ResultYFlip = VBMath.Rnd() >= 0.5F;
				ReturnResult.Orientation.SwitchedAxes = VBMath.Rnd() >= 0.5F;
				ReturnResult.TextureNum = -1;
				return ReturnResult;
			}
			//stop invalid numbers
			if (TileChance.Direction.X > 2 | TileChance.Direction.Y > 2 | NewDirection.X > 2 | NewDirection.Y > 2)
			{
				Debugger.Break();
				return ReturnResult;
			}
			//stop different direction types
			if ((NewDirection.X == 1 ^ NewDirection.Y == 1) ^ (TileChance.Direction.X == 1 ^ TileChance.Direction.Y == 1))
			{
				Debugger.Break();
				return ReturnResult;
			}
			
			ReturnResult.TextureNum = TileChance.TextureNum;
			
			//if a direction is neutral then give a random orientation
			if ((NewDirection.X == 1 & NewDirection.Y == 1) || (TileChance.Direction.X == 1 & TileChance.Direction.Y == 1))
			{
				ReturnResult.Orientation.SwitchedAxes = VBMath.Rnd() >= 0.5F;
				ReturnResult.Orientation.ResultXFlip = VBMath.Rnd() >= 0.5F;
				ReturnResult.Orientation.ResultYFlip = VBMath.Rnd() >= 0.5F;
				return ReturnResult;
			}
			
			bool IsDiagonal = default(bool);
			
			IsDiagonal = NewDirection.X != 1 & NewDirection.Y != 1;
			if (IsDiagonal)
			{
				ReturnResult.Orientation.SwitchedAxes = false;
				//use flips to match the directions
				if (TileChance.Direction.X == 0 ^ NewDirection.X == 0)
				{
					ReturnResult.Orientation.ResultXFlip = true;
				}
				else
				{
					ReturnResult.Orientation.ResultXFlip = false;
				}
				if (TileChance.Direction.Y == 0 ^ NewDirection.Y == 0)
				{
					ReturnResult.Orientation.ResultYFlip = true;
				}
				else
				{
					ReturnResult.Orientation.ResultYFlip = false;
				}
				//randomly switch to the alternate orientation
				if (VBMath.Rnd() >= 0.5F)
				{
					ReturnResult.Orientation.SwitchedAxes = !ReturnResult.Orientation.SwitchedAxes;
					if ((NewDirection.X == 0 ^ NewDirection.Y == 0) ^ (ReturnResult.Orientation.ResultXFlip ^ ReturnResult.Orientation.ResultYFlip))
					{
						ReturnResult.Orientation.ResultXFlip = !ReturnResult.Orientation.ResultXFlip;
						ReturnResult.Orientation.ResultYFlip = !ReturnResult.Orientation.ResultYFlip;
					}
				}
			}
			else
			{
				//switch axes if the directions are on different axes
				ReturnResult.Orientation.SwitchedAxes = TileChance.Direction.X == 1 ^ NewDirection.X == 1;
				//use a flip to match the directions
				if (ReturnResult.Orientation.SwitchedAxes)
				{
					if (TileChance.Direction.Y != NewDirection.X)
					{
						ReturnResult.Orientation.ResultXFlip = true;
					}
					else
					{
						ReturnResult.Orientation.ResultXFlip = false;
					}
					if (TileChance.Direction.X != NewDirection.Y)
					{
						ReturnResult.Orientation.ResultYFlip = true;
					}
					else
					{
						ReturnResult.Orientation.ResultYFlip = false;
					}
				}
				else
				{
					if (TileChance.Direction.X != NewDirection.X)
					{
						ReturnResult.Orientation.ResultXFlip = true;
					}
					else
					{
						ReturnResult.Orientation.ResultXFlip = false;
					}
					if (TileChance.Direction.Y != NewDirection.Y)
					{
						ReturnResult.Orientation.ResultYFlip = true;
					}
					else
					{
						ReturnResult.Orientation.ResultYFlip = false;
					}
				}
				//randomly switch to the alternate orientation
				if (VBMath.Rnd() >= 0.5F)
				{
					if (NewDirection.X == 1)
					{
						ReturnResult.Orientation.ResultXFlip = !ReturnResult.Orientation.ResultXFlip;
					}
					else
					{
						ReturnResult.Orientation.ResultYFlip = !ReturnResult.Orientation.ResultYFlip;
					}
				}
			}
			
			return ReturnResult;
		}
		
		public static void RotateDirection(sTileDirection InitialDirection, sTileOrientation Orientation, ref sTileDirection ResultDirection)
		{
			
			ResultDirection = InitialDirection;
			if (Orientation.SwitchedAxes)
			{
				ResultDirection.SwitchAxes();
			}
			if (Orientation.ResultXFlip)
			{
				ResultDirection.FlipX();
			}
			if (Orientation.ResultYFlip)
			{
				ResultDirection.FlipY();
			}
		}
		
		public static modMath.sXY_int GetTileRotatedOffset(sTileOrientation TileOrientation, modMath.sXY_int Pos)
		{
			modMath.sXY_int Result = new modMath.sXY_int();
			
			if (TileOrientation.SwitchedAxes)
			{
				if (TileOrientation.ResultXFlip)
				{
					Result.X = modProgram.TerrainGridSpacing - Pos.Y;
				}
				else
				{
					Result.X = Pos.Y;
				}
				if (TileOrientation.ResultYFlip)
				{
					Result.Y = modProgram.TerrainGridSpacing - Pos.X;
				}
				else
				{
					Result.Y = Pos.X;
				}
			}
			else
			{
				if (TileOrientation.ResultXFlip)
				{
					Result.X = modProgram.TerrainGridSpacing - Pos.X;
				}
				else
				{
					Result.X = Pos.X;
				}
				if (TileOrientation.ResultYFlip)
				{
					Result.Y = modProgram.TerrainGridSpacing - Pos.Y;
				}
				else
				{
					Result.Y = Pos.Y;
				}
			}
			
			return Result;
		}
		
		public static modMath.sXY_sng GetTileRotatedPos_sng(sTileOrientation TileOrientation, modMath.sXY_sng Pos)
		{
			modMath.sXY_sng ReturnResult = new modMath.sXY_sng();
			
			if (TileOrientation.SwitchedAxes)
			{
				if (TileOrientation.ResultXFlip)
				{
					ReturnResult.X = 1.0F - Pos.Y;
				}
				else
				{
					ReturnResult.X = Pos.Y;
				}
				if (TileOrientation.ResultYFlip)
				{
					ReturnResult.Y = 1.0F - Pos.X;
				}
				else
				{
					ReturnResult.Y = Pos.X;
				}
			}
			else
			{
				if (TileOrientation.ResultXFlip)
				{
					ReturnResult.X = 1.0F - Pos.X;
				}
				else
				{
					ReturnResult.X = Pos.X;
				}
				if (TileOrientation.ResultYFlip)
				{
					ReturnResult.Y = 1.0F - Pos.Y;
				}
				else
				{
					ReturnResult.Y = Pos.Y;
				}
			}
			
			return ReturnResult;
		}
		
		public static Matrix3D.Position.XY_dbl GetTileRotatedPos_dbl(sTileOrientation TileOrientation, Matrix3D.Position.XY_dbl Pos)
		{
			Matrix3D.Position.XY_dbl ReturnResult = default(Matrix3D.Position.XY_dbl);
			
			if (TileOrientation.SwitchedAxes)
			{
				if (TileOrientation.ResultXFlip)
				{
					ReturnResult.X = 1.0D - Pos.Y;
				}
				else
				{
					ReturnResult.X = Pos.Y;
				}
				if (TileOrientation.ResultYFlip)
				{
					ReturnResult.Y = 1.0D - Pos.X;
				}
				else
				{
					ReturnResult.Y = Pos.X;
				}
			}
			else
			{
				if (TileOrientation.ResultXFlip)
				{
					ReturnResult.X = 1.0D - Pos.X;
				}
				else
				{
					ReturnResult.X = Pos.X;
				}
				if (TileOrientation.ResultYFlip)
				{
					ReturnResult.Y = 1.0D - Pos.Y;
				}
				else
				{
					ReturnResult.Y = Pos.Y;
				}
			}
			
			return ReturnResult;
		}
		
		public static modMath.sXY_int GetRotatedPos(sTileOrientation Orientation, modMath.sXY_int Pos, modMath.sXY_int Limits)
		{
			modMath.sXY_int Result = new modMath.sXY_int();
			
			if (Orientation.SwitchedAxes)
			{
				if (Orientation.ResultXFlip)
				{
					Result.X = Limits.Y - Pos.Y;
				}
				else
				{
					Result.X = Pos.Y;
				}
				if (Orientation.ResultYFlip)
				{
					Result.Y = Limits.X - Pos.X;
				}
				else
				{
					Result.Y = Pos.X;
				}
			}
			else
			{
				if (Orientation.ResultXFlip)
				{
					Result.X = Limits.X - Pos.X;
				}
				else
				{
					Result.X = Pos.X;
				}
				if (Orientation.ResultYFlip)
				{
					Result.Y = Limits.Y - Pos.Y;
				}
				else
				{
					Result.Y = Pos.Y;
				}
			}
			
			return Result;
		}
		
		public static double GetRotatedAngle(sTileOrientation Orientation, double Angle)
		{
			Matrix3D.Position.XY_dbl XY_dbl = default(Matrix3D.Position.XY_dbl);
			
			XY_dbl = GetTileRotatedPos_dbl(Orientation, new Matrix3D.Position.XY_dbl((Math.Cos(Angle) + 1.0D) / 2.0D, (Math.Sin(Angle) + 1.0D) / 2.0D));
			XY_dbl.X = XY_dbl.X * 2.0D - 1.0D;
			XY_dbl.Y = XY_dbl.Y * 2.0D - 1.0D;
			return Math.Atan2(XY_dbl.Y, XY_dbl.X);
		}
		
		public static void GetTileRotatedTexCoords(sTileOrientation TileOrientation, modMath.sXY_sng CoordA, modMath.sXY_sng CoordB, modMath.sXY_sng CoordC, modMath.sXY_sng CoordD)
		{
			sTileOrientation ReverseOrientation = new sTileOrientation();
			
			ReverseOrientation = TileOrientation;
			ReverseOrientation.Reverse();
			
			if (ReverseOrientation.SwitchedAxes)
			{
				if (ReverseOrientation.ResultXFlip)
				{
					CoordA.X = 1.0F;
					CoordB.X = 1.0F;
					CoordC.X = 0.0F;
					CoordD.X = 0.0F;
				}
				else
				{
					CoordA.X = 0.0F;
					CoordB.X = 0.0F;
					CoordC.X = 1.0F;
					CoordD.X = 1.0F;
				}
				if (ReverseOrientation.ResultYFlip)
				{
					CoordA.Y = 1.0F;
					CoordB.Y = 0.0F;
					CoordC.Y = 1.0F;
					CoordD.Y = 0.0F;
				}
				else
				{
					CoordA.Y = 0.0F;
					CoordB.Y = 1.0F;
					CoordC.Y = 0.0F;
					CoordD.Y = 1.0F;
				}
			}
			else
			{
				if (ReverseOrientation.ResultXFlip)
				{
					CoordA.X = 1.0F;
					CoordB.X = 0.0F;
					CoordC.X = 1.0F;
					CoordD.X = 0.0F;
				}
				else
				{
					CoordA.X = 0.0F;
					CoordB.X = 1.0F;
					CoordC.X = 0.0F;
					CoordD.X = 1.0F;
				}
				if (ReverseOrientation.ResultYFlip)
				{
					CoordA.Y = 1.0F;
					CoordB.Y = 1.0F;
					CoordC.Y = 0.0F;
					CoordD.Y = 0.0F;
				}
				else
				{
					CoordA.Y = 0.0F;
					CoordB.Y = 0.0F;
					CoordC.Y = 1.0F;
					CoordD.Y = 1.0F;
				}
			}
		}
		
		public static void TileOrientation_To_OldOrientation(sTileOrientation TileOrientation, ref byte OutputRotation, ref bool OutputFlipX)
		{
			
			if (TileOrientation.SwitchedAxes)
			{
				if (TileOrientation.ResultXFlip)
				{
					OutputRotation = (byte) 1;
				}
				else
				{
					OutputRotation = (byte) 3;
				}
				OutputFlipX = !(TileOrientation.ResultXFlip ^ TileOrientation.ResultYFlip);
			}
			else
			{
				if (TileOrientation.ResultYFlip)
				{
					OutputRotation = (byte) 2;
				}
				else
				{
					OutputRotation = (byte) 0;
				}
				OutputFlipX = TileOrientation.ResultXFlip ^ TileOrientation.ResultYFlip;
			}
		}
		
		public static void OldOrientation_To_TileOrientation(byte OldRotation, bool OldFlipX, bool OldFlipZ, sTileOrientation Result)
		{
			
			if (OldRotation == 0)
			{
				Result.SwitchedAxes = false;
				Result.ResultXFlip = false;
				Result.ResultYFlip = false;
			}
			else if (OldRotation == 1)
			{
				Result.SwitchedAxes = true;
				Result.ResultXFlip = true;
				Result.ResultYFlip = false;
			}
			else if (OldRotation == 2)
			{
				Result.SwitchedAxes = false;
				Result.ResultXFlip = true;
				Result.ResultYFlip = true;
			}
			else if (OldRotation == 3)
			{
				Result.SwitchedAxes = true;
				Result.ResultXFlip = false;
				Result.ResultYFlip = true;
			}
			if (OldFlipX)
			{
				if (Result.SwitchedAxes)
				{
					Result.ResultYFlip = !Result.ResultYFlip;
				}
				else
				{
					Result.ResultXFlip = !Result.ResultXFlip;
				}
			}
			if (OldFlipZ)
			{
				if (Result.SwitchedAxes)
				{
					Result.ResultXFlip = !Result.ResultXFlip;
				}
				else
				{
					Result.ResultYFlip = !Result.ResultYFlip;
				}
			}
		}
		
		public static bool IdenticalTileDirections(sTileDirection TileOrientationA, sTileDirection TileOrientationB)
		{
			
			return TileOrientationA.X == TileOrientationB.X & TileOrientationA.Y == TileOrientationB.Y;
		}
		
		public static bool DirectionsOnSameSide(sTileDirection DirectionA, sTileDirection DirectionB)
		{
			
			if (DirectionA.X == 0)
			{
				if (DirectionB.X == 0)
				{
					return true;
				}
			}
			if (DirectionA.X == 2)
			{
				if (DirectionB.X == 2)
				{
					return true;
				}
			}
			if (DirectionA.Y == 0)
			{
				if (DirectionB.Y == 0)
				{
					return true;
				}
			}
			if (DirectionA.Y == 2)
			{
				if (DirectionB.Y == 2)
				{
					return true;
				}
			}
			return false;
		}
		
		public static bool DirectionsAreInLine(sTileDirection DirectionA, sTileDirection DirectionB)
		{
			
			if (DirectionA.X == DirectionB.X)
			{
				return true;
			}
			if (DirectionA.Y == DirectionB.Y)
			{
				return true;
			}
			return false;
		}
	}
	
}
