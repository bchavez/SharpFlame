using System;

namespace SharpFlame.Maths
{
    public sealed class Matrix3DMath
    {
        public static double MatrixAngleToMatrix(Matrix3D MatrixA, Matrix3D MatrixB)
        {
            Matrix3D matrixOut = new Matrix3D();
            MatrixInvert(MatrixA, matrixOut);
            double y = ((matrixOut.Values[0] * MatrixB.Values[2]) + (matrixOut.Values[1] * MatrixB.Values[5])) + (matrixOut.Values[2] * MatrixB.Values[8]);
            double x = ((matrixOut.Values[3] * MatrixB.Values[2]) + (matrixOut.Values[4] * MatrixB.Values[5])) + (matrixOut.Values[5] * MatrixB.Values[8]);
            double num3 = ((matrixOut.Values[6] * MatrixB.Values[2]) + (matrixOut.Values[7] * MatrixB.Values[5])) + (matrixOut.Values[8] * MatrixB.Values[8]);
            double a = Math.Atan2(y, x);
            double num5 = (Math.Sin(a) * y) + (Math.Cos(a) * x);
            return Math.Atan2(num5, num3);
        }

        public static void MatrixAngleToMatrix(Matrix3D MatrixA, Matrix3D MatrixB, ref double ResultArcAngle, ref double ResultDirectionAngle)
        {
            Matrix3D matrixOut = new Matrix3D();
            MatrixInvert(MatrixA, matrixOut);
            double x = ((matrixOut.Values[0] * MatrixB.Values[2]) + (matrixOut.Values[1] * MatrixB.Values[5])) + (matrixOut.Values[2] * MatrixB.Values[8]);
            double y = ((matrixOut.Values[3] * MatrixB.Values[2]) + (matrixOut.Values[4] * MatrixB.Values[5])) + (matrixOut.Values[5] * MatrixB.Values[8]);
            double num3 = ((matrixOut.Values[6] * MatrixB.Values[2]) + (matrixOut.Values[7] * MatrixB.Values[5])) + (matrixOut.Values[8] * MatrixB.Values[8]);
            ResultDirectionAngle = Math.Atan2(y, x);
            double a = Math.Atan2(x, y);
            double num4 = (Math.Sin(a) * x) + (Math.Cos(a) * y);
            ResultArcAngle = Math.Atan2(num4, num3);
        }

        public static double MatrixAngleToVector(Matrix3D Matrix, Position.XYZ_dbl Vector)
        {
            Matrix3D matrixOut = new Matrix3D();
            MatrixInvert(Matrix, matrixOut);
            double y = ((matrixOut.Values[0] * Vector.X) + (matrixOut.Values[1] * Vector.Y)) + (matrixOut.Values[2] * Vector.Z);
            double x = ((matrixOut.Values[3] * Vector.X) + (matrixOut.Values[4] * Vector.Y)) + (matrixOut.Values[5] * Vector.Z);
            double num3 = ((matrixOut.Values[6] * Vector.X) + (matrixOut.Values[7] * Vector.Y)) + (matrixOut.Values[8] * Vector.Z);
            double a = Math.Atan2(y, x);
            double num5 = (Math.Sin(a) * y) + (Math.Cos(a) * x);
            return Math.Atan2(num5, num3);
        }

        public static void MatrixAngleToVector(Matrix3D Matrix, Position.XYZ_dbl Vector, ref double ResultArcAngle, ref double ResultDirectionAngle)
        {
            Matrix3D matrixOut = new Matrix3D();
            MatrixInvert(Matrix, matrixOut);
            double x = ((matrixOut.Values[0] * Vector.X) + (matrixOut.Values[1] * Vector.Y)) + (matrixOut.Values[2] * Vector.Z);
            double y = ((matrixOut.Values[3] * Vector.X) + (matrixOut.Values[4] * Vector.Y)) + (matrixOut.Values[5] * Vector.Z);
            double num3 = ((matrixOut.Values[6] * Vector.X) + (matrixOut.Values[7] * Vector.Y)) + (matrixOut.Values[8] * Vector.Z);
            ResultDirectionAngle = Math.Atan2(y, x);
            double a = Math.Atan2(x, y);
            double num4 = (Math.Sin(a) * x) + (Math.Cos(a) * y);
            ResultArcAngle = Math.Atan2(num4, num3);
        }

        public static void MatrixCopy(Matrix3D MatrixIn, Matrix3D MatrixOut)
        {
            MatrixOut.Values[0] = MatrixIn.Values[0];
            MatrixOut.Values[1] = MatrixIn.Values[1];
            MatrixOut.Values[2] = MatrixIn.Values[2];
            MatrixOut.Values[3] = MatrixIn.Values[3];
            MatrixOut.Values[4] = MatrixIn.Values[4];
            MatrixOut.Values[5] = MatrixIn.Values[5];
            MatrixOut.Values[6] = MatrixIn.Values[6];
            MatrixOut.Values[7] = MatrixIn.Values[7];
            MatrixOut.Values[8] = MatrixIn.Values[8];
        }

        public static void MatrixInvert(Matrix3D MatrixIn, Matrix3D MatrixOut)
        {
            double num = MatrixIn.Values[0];
            double num3 = MatrixIn.Values[1];
            double num5 = MatrixIn.Values[2];
            double num7 = MatrixIn.Values[3];
            double num9 = MatrixIn.Values[4];
            double num11 = MatrixIn.Values[5];
            double num13 = MatrixIn.Values[6];
            double num15 = MatrixIn.Values[7];
            double num17 = MatrixIn.Values[8];
            double num2 = (num9 * num17) - (num15 * num11);
            double num4 = (num13 * num11) - (num7 * num17);
            double num6 = (num7 * num15) - (num13 * num9);
            double num8 = (num15 * num5) - (num3 * num17);
            double num10 = (num * num17) - (num13 * num5);
            double num12 = (num13 * num3) - (num * num15);
            double num14 = (num3 * num11) - (num9 * num5);
            double num16 = (num7 * num5) - (num * num11);
            double num18 = (num * num9) - (num7 * num3);
            double num19 = ((num * num2) + (num3 * num4)) + (num5 * num6);
            MatrixOut.Values[0] = num2 / num19;
            MatrixOut.Values[1] = num8 / num19;
            MatrixOut.Values[2] = num14 / num19;
            MatrixOut.Values[3] = num4 / num19;
            MatrixOut.Values[4] = num10 / num19;
            MatrixOut.Values[5] = num16 / num19;
            MatrixOut.Values[6] = num6 / num19;
            MatrixOut.Values[7] = num12 / num19;
            MatrixOut.Values[8] = num18 / num19;
        }

        public static void MatrixNormalize(Matrix3D Matrix)
        {
            Angles.AngleRPY erpy = new Angles.AngleRPY();
            MatrixToRPY(Matrix, ref erpy);
            MatrixSetToRPY(Matrix, erpy);
        }

        public static void MatrixRotationAroundAxis(Matrix3D MatrixIn, Matrix3D MatrixAxis, double TurnAngle, Matrix3D ResultMatrix)
        {
            Matrix3D matrixOut = new Matrix3D();
            Matrix3D resultMatrix = new Matrix3D();
            Matrix3D matrixd3 = new Matrix3D();
            MatrixInvert(MatrixAxis, matrixOut);
            MatrixRotationByMatrix(matrixOut, MatrixIn, resultMatrix);
            MatrixSetToZAngle(matrixOut, TurnAngle);
            MatrixRotationByMatrix(matrixOut, resultMatrix, matrixd3);
            MatrixRotationByMatrix(MatrixAxis, matrixd3, ResultMatrix);
        }

        public static void MatrixRotationByMatrix(Matrix3D ChangeMatrix, Matrix3D OriginMatrix, Matrix3D ResultMatrix)
        {
            ResultMatrix.Values[0] = ((ChangeMatrix.Values[0] * OriginMatrix.Values[0]) + (ChangeMatrix.Values[1] * OriginMatrix.Values[3])) + (ChangeMatrix.Values[2] * OriginMatrix.Values[6]);
            ResultMatrix.Values[1] = ((ChangeMatrix.Values[0] * OriginMatrix.Values[1]) + (ChangeMatrix.Values[1] * OriginMatrix.Values[4])) + (ChangeMatrix.Values[2] * OriginMatrix.Values[7]);
            ResultMatrix.Values[2] = ((ChangeMatrix.Values[0] * OriginMatrix.Values[2]) + (ChangeMatrix.Values[1] * OriginMatrix.Values[5])) + (ChangeMatrix.Values[2] * OriginMatrix.Values[8]);
            ResultMatrix.Values[3] = ((ChangeMatrix.Values[3] * OriginMatrix.Values[0]) + (ChangeMatrix.Values[4] * OriginMatrix.Values[3])) + (ChangeMatrix.Values[5] * OriginMatrix.Values[6]);
            ResultMatrix.Values[4] = ((ChangeMatrix.Values[3] * OriginMatrix.Values[1]) + (ChangeMatrix.Values[4] * OriginMatrix.Values[4])) + (ChangeMatrix.Values[5] * OriginMatrix.Values[7]);
            ResultMatrix.Values[5] = ((ChangeMatrix.Values[3] * OriginMatrix.Values[2]) + (ChangeMatrix.Values[4] * OriginMatrix.Values[5])) + (ChangeMatrix.Values[5] * OriginMatrix.Values[8]);
            ResultMatrix.Values[6] = ((ChangeMatrix.Values[6] * OriginMatrix.Values[0]) + (ChangeMatrix.Values[7] * OriginMatrix.Values[3])) + (ChangeMatrix.Values[8] * OriginMatrix.Values[6]);
            ResultMatrix.Values[7] = ((ChangeMatrix.Values[6] * OriginMatrix.Values[1]) + (ChangeMatrix.Values[7] * OriginMatrix.Values[4])) + (ChangeMatrix.Values[8] * OriginMatrix.Values[7]);
            ResultMatrix.Values[8] = ((ChangeMatrix.Values[6] * OriginMatrix.Values[2]) + (ChangeMatrix.Values[7] * OriginMatrix.Values[5])) + (ChangeMatrix.Values[8] * OriginMatrix.Values[8]);
        }

        public static void MatrixSetToIdentity(Matrix3D Matrix)
        {
            Matrix.Values[0] = 1.0;
            Matrix.Values[1] = 0.0;
            Matrix.Values[2] = 0.0;
            Matrix.Values[3] = 0.0;
            Matrix.Values[4] = 1.0;
            Matrix.Values[5] = 0.0;
            Matrix.Values[6] = 0.0;
            Matrix.Values[7] = 0.0;
            Matrix.Values[8] = 1.0;
        }

        public static void MatrixSetToPY(Matrix3D Matrix, Angles.AnglePY AnglePY)
        {
            Matrix3D matrix = new Matrix3D();
            Matrix3D matrixd3 = new Matrix3D();
            Matrix3D matrixd = new Matrix3D();
            MatrixSetToXAngle(matrix, AnglePY.Pitch);
            MatrixSetToYAngle(matrixd3, AnglePY.Yaw);
            MatrixRotationByMatrix(matrixd3, matrix, Matrix);
        }

        public static void MatrixSetToRPY(Matrix3D Matrix, Angles.AngleRPY AngleRPY)
        {
            Matrix3D matrix = new Matrix3D();
            Matrix3D matrixd2 = new Matrix3D();
            Matrix3D matrixd3 = new Matrix3D();
            Matrix3D resultMatrix = new Matrix3D();
            MatrixSetToZAngle(matrix, AngleRPY.Roll);
            MatrixSetToXAngle(matrixd2, AngleRPY.Pitch);
            MatrixSetToYAngle(matrixd3, AngleRPY.Yaw);
            MatrixRotationByMatrix(matrixd2, matrix, resultMatrix);
            MatrixRotationByMatrix(matrixd3, resultMatrix, Matrix);
        }

        public static void MatrixSetToRPY(Matrix3D Matrix, double Roll, double Pitch, double Yaw)
        {
            Matrix3D matrix = new Matrix3D();
            Matrix3D matrixd2 = new Matrix3D();
            Matrix3D matrixd3 = new Matrix3D();
            Matrix3D resultMatrix = new Matrix3D();
            MatrixSetToZAngle(matrix, Roll);
            MatrixSetToXAngle(matrixd2, Pitch);
            MatrixSetToYAngle(matrixd3, Yaw);
            MatrixRotationByMatrix(matrixd2, matrix, resultMatrix);
            MatrixRotationByMatrix(matrixd3, resultMatrix, Matrix);
        }

        public static void MatrixSetToXAngle(Matrix3D Matrix, double X)
        {
            Matrix.Values[0] = 1.0;
            Matrix.Values[1] = 0.0;
            Matrix.Values[2] = 0.0;
            Matrix.Values[3] = 0.0;
            Matrix.Values[4] = Math.Cos(X);
            Matrix.Values[5] = -Math.Sin(X);
            Matrix.Values[6] = 0.0;
            Matrix.Values[7] = Math.Sin(X);
            Matrix.Values[8] = Math.Cos(X);
        }

        public static void MatrixSetToYAngle(Matrix3D Matrix, double Y)
        {
            Matrix.Values[0] = Math.Cos(Y);
            Matrix.Values[1] = 0.0;
            Matrix.Values[2] = Math.Sin(Y);
            Matrix.Values[3] = 0.0;
            Matrix.Values[4] = 1.0;
            Matrix.Values[5] = 0.0;
            Matrix.Values[6] = -Math.Sin(Y);
            Matrix.Values[7] = 0.0;
            Matrix.Values[8] = Math.Cos(Y);
        }

        public static void MatrixSetToZAngle(Matrix3D Matrix, double Z)
        {
            Matrix.Values[0] = Math.Cos(Z);
            Matrix.Values[1] = -Math.Sin(Z);
            Matrix.Values[2] = 0.0;
            Matrix.Values[3] = Math.Sin(Z);
            Matrix.Values[4] = Math.Cos(Z);
            Matrix.Values[5] = 0.0;
            Matrix.Values[6] = 0.0;
            Matrix.Values[7] = 0.0;
            Matrix.Values[8] = 1.0;
        }

        public static void MatrixToPY(Matrix3D Matrix, ref Angles.AnglePY ResultPY)
        {
            Position.XYZ_dbl _dbl = new Position.XYZ_dbl();
            VectorForwardsRotationByMatrix(Matrix, ref _dbl);
            VectorToPY(_dbl, ref ResultPY);
        }

        public static void MatrixToRPY(Matrix3D Matrix, ref Angles.AngleRPY ResultRPY)
        {
            Angles.AnglePY epy = new Angles.AnglePY();
            Position.XYZ_dbl _dbl = new Position.XYZ_dbl();
            Position.XYZ_dbl _dbl2 = new Position.XYZ_dbl();
            Matrix3D matrix = new Matrix3D();
            Matrix3D matrixd = new Matrix3D();
            VectorForwardsRotationByMatrix(Matrix, ref _dbl2);
            VectorToPY(_dbl2, ref epy);
            ResultRPY.PY = epy;
            VectorRightRotationByMatrix(Matrix, ref _dbl2);
            MatrixSetToXAngle(matrixd, -epy.Pitch);
            MatrixSetToYAngle(matrix, -epy.Yaw);
            VectorRotationByMatrix(matrix, _dbl2, ref _dbl);
            VectorRotationByMatrix(matrixd, _dbl, ref _dbl2);
            ResultRPY.Roll = Math.Atan2(_dbl2.Y, _dbl2.X);
        }

        public static double VectorAngleToVector(Position.XYZ_dbl VectorA, Position.XYZ_dbl VectorB)
        {
            Angles.AnglePY epy = new Angles.AnglePY();
            Matrix3D matrix = new Matrix3D();
            VectorToPY(VectorA, ref epy);
            MatrixSetToPY(matrix, epy);
            return MatrixAngleToVector(matrix, VectorB);
        }

        public static void VectorBackwardsRotationByMatrix(Matrix3D Matrix, ref Position.XYZ_dbl ResultVector)
        {
            ResultVector.X = -Matrix.Values[2];
            ResultVector.Y = -Matrix.Values[5];
            ResultVector.Z = -Matrix.Values[8];
        }

        public static void VectorBackwardsRotationByMatrix(Matrix3D Matrix, double Scale, ref Position.XYZ_dbl ResultVector)
        {
            ResultVector.X = Matrix.Values[2] * -Scale;
            ResultVector.Y = Matrix.Values[5] * -Scale;
            ResultVector.Z = Matrix.Values[8] * -Scale;
        }

        public static void VectorCrossProduct(Position.XYZ_dbl VectorA, Position.XYZ_dbl VectorB, ref Position.XYZ_dbl ResultVector)
        {
            ResultVector.X = (VectorA.Y * VectorB.Z) - (VectorB.Y * VectorA.Z);
            ResultVector.Y = (VectorA.Z * VectorB.X) - (VectorB.Z * VectorA.X);
            ResultVector.Z = (VectorA.X * VectorB.Y) - (VectorB.X * VectorA.Y);
        }

        public static void VectorDownRotationByMatrix(Matrix3D Matrix, ref Position.XYZ_dbl ResultVector)
        {
            ResultVector.X = -Matrix.Values[1];
            ResultVector.Y = -Matrix.Values[4];
            ResultVector.Z = -Matrix.Values[7];
        }

        public static void VectorDownRotationByMatrix(Matrix3D Matrix, double Scale, ref Position.XYZ_dbl ResultVector)
        {
            ResultVector.X = Matrix.Values[1] * -Scale;
            ResultVector.Y = Matrix.Values[4] * -Scale;
            ResultVector.Z = Matrix.Values[7] * -Scale;
        }

        public static void VectorForwardsRotationByMatrix(Matrix3D Matrix, ref Position.XYZ_dbl ResultVector)
        {
            ResultVector.X = Matrix.Values[2];
            ResultVector.Y = Matrix.Values[5];
            ResultVector.Z = Matrix.Values[8];
        }

        public static void VectorForwardsRotationByMatrix(Matrix3D Matrix, double Scale, ref Position.XYZ_dbl ResultVector)
        {
            ResultVector.X = Matrix.Values[2] * Scale;
            ResultVector.Y = Matrix.Values[5] * Scale;
            ResultVector.Z = Matrix.Values[8] * Scale;
        }

        public static void VectorLeftRotationByMatrix(Matrix3D Matrix, ref Position.XYZ_dbl ResultVector)
        {
            ResultVector.X = -Matrix.Values[0];
            ResultVector.Y = -Matrix.Values[3];
            ResultVector.Z = -Matrix.Values[6];
        }

        public static void VectorLeftRotationByMatrix(Matrix3D Matrix, double Scale, ref Position.XYZ_dbl ResultVector)
        {
            ResultVector.X = Matrix.Values[0] * -Scale;
            ResultVector.Y = Matrix.Values[3] * -Scale;
            ResultVector.Z = Matrix.Values[6] * -Scale;
        }

        public static void VectorRightRotationByMatrix(Matrix3D Matrix, ref Position.XYZ_dbl ResultVector)
        {
            ResultVector.X = Matrix.Values[0];
            ResultVector.Y = Matrix.Values[3];
            ResultVector.Z = Matrix.Values[6];
        }

        public static void VectorRightRotationByMatrix(Matrix3D Matrix, double Scale, ref Position.XYZ_dbl ResultVector)
        {
            ResultVector.X = Matrix.Values[0] * Scale;
            ResultVector.Y = Matrix.Values[3] * Scale;
            ResultVector.Z = Matrix.Values[6] * Scale;
        }

        public static void VectorRotationByMatrix(Matrix3D Matrix, Position.XYZ_dbl Vector, ref Position.XYZ_dbl ResultVector)
        {
            ResultVector.X = ((Vector.X * Matrix.Values[0]) + (Vector.Y * Matrix.Values[1])) + (Vector.Z * Matrix.Values[2]);
            ResultVector.Y = ((Vector.X * Matrix.Values[3]) + (Vector.Y * Matrix.Values[4])) + (Vector.Z * Matrix.Values[5]);
            ResultVector.Z = ((Vector.X * Matrix.Values[6]) + (Vector.Y * Matrix.Values[7])) + (Vector.Z * Matrix.Values[8]);
        }

        public static void VectorToPY(Position.XYZ_dbl Vector, ref Angles.AnglePY ResultPY)
        {
            ResultPY.Pitch = Math.Atan2(-Vector.Y, Math.Sqrt((Vector.X * Vector.X) + (Vector.Z * Vector.Z)));
            if (ResultPY.Pitch > 1.5707963267948966)
            {
                ResultPY.Pitch = 3.1415926535897931 - ResultPY.Pitch;
            }
            else if (ResultPY.Pitch < -1.5707963267948966)
            {
                ResultPY.Pitch = -ResultPY.Pitch - 3.1415926535897931;
            }
            ResultPY.Yaw = Math.Atan2(Vector.X, Vector.Z);
        }

        public static void VectorUpRotationByMatrix(Matrix3D Matrix, ref Position.XYZ_dbl ResultVector)
        {
            ResultVector.X = Matrix.Values[1];
            ResultVector.Y = Matrix.Values[4];
            ResultVector.Z = Matrix.Values[7];
        }

        public static void VectorUpRotationByMatrix(Matrix3D Matrix, double Scale, ref Position.XYZ_dbl ResultVector)
        {
            ResultVector.X = Matrix.Values[1] * Scale;
            ResultVector.Y = Matrix.Values[4] * Scale;
            ResultVector.Z = Matrix.Values[7] * Scale;
        }

        public class Matrix3D
        {
            public double[] Values = new double[9];
        }
    }
}

