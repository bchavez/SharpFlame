namespace FlaME
{
    using Microsoft.VisualBasic;
    using OpenTK.Graphics.OpenGL;
    using System;

    public class clsTextLabel
    {
        public sRGBA_sng Colour;
        public modMath.sXY_int Pos;
        public float SizeY;
        public string Text;
        public GLFont TextFont;

        public void Draw()
        {
            if (((this.Text != null) && (this.Text.Length != 0)) && (this.TextFont != null))
            {
                GL.Color4(this.Colour.Red, this.Colour.Green, this.Colour.Blue, this.Colour.Alpha);
                float y = this.Pos.Y;
                float num8 = this.Pos.Y + this.SizeY;
                float num3 = this.SizeY / 10f;
                float x = this.Pos.X;
                int num9 = this.Text.Length - 1;
                for (int i = 0; i <= num9; i++)
                {
                    int index = this.Text[i];
                    if ((index >= 0) & (index <= 0xff))
                    {
                        modMath.sXY_sng _sng;
                        float num4 = (this.SizeY * this.TextFont.Character[index].Width) / ((float) this.TextFont.Height);
                        _sng.X = (float) (((double) this.TextFont.Character[index].Width) / ((double) this.TextFont.Character[index].TexSize));
                        _sng.Y = (float) (((double) this.TextFont.Height) / ((double) this.TextFont.Character[index].TexSize));
                        float num6 = x + num4;
                        GL.BindTexture(TextureTarget.Texture2D, this.TextFont.Character[index].GLTexture);
                        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, 0x2100);
                        GL.Begin(BeginMode.Quads);
                        GL.TexCoord2((float) 0f, (float) 0f);
                        GL.Vertex2(x, y);
                        GL.TexCoord2(0f, _sng.Y);
                        GL.Vertex2(x, num8);
                        GL.TexCoord2(_sng.X, _sng.Y);
                        GL.Vertex2(num6, num8);
                        GL.TexCoord2(_sng.X, 0f);
                        GL.Vertex2(num6, y);
                        GL.End();
                        x = num6 + num3;
                    }
                }
            }
        }

        public float GetSizeX()
        {
            float num6;
            float num3 = this.SizeY / 10f;
            float num2 = this.SizeY / ((float) this.TextFont.Height);
            int num7 = this.Text.Length - 1;
            for (int i = 0; i <= num7; i++)
            {
                float num4 = this.TextFont.Character[Strings.Asc(this.Text[i])].Width * num2;
                num6 += num4;
            }
            return (num6 + (num3 * (this.Text.Length - 1)));
        }
    }
}

