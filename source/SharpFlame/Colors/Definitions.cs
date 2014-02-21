using SharpFlame.FileIO;

namespace SharpFlame.Colors
{
    public struct sRGB_sng
    {
        public float Red;
        public float Green;
        public float Blue;

        public sRGB_sng(float red, float green, float blue)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }
    }

    public struct sRGBA_sng
    {
        public float Red;
        public float Green;
        public float Blue;
        public float Alpha;

        public sRGBA_sng( float red, float green, float blue, float alpha )
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.Alpha = alpha;
        }
    }

    public class clsRGB_sng
    {
        public float Red;
        public float Green;
        public float Blue;

        public clsRGB_sng( float red, float green, float blue )
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }

        public virtual string GetINIOutput()
        {
            return Red.ToStringInvariant() + ", " + Green.ToStringInvariant() + ", " + Blue.ToStringInvariant();
        }

        public virtual bool ReadINIText( SplitCommaText splitText )
        {
            if( splitText.PartCount < 3 )
            {
                return false;
            }

            sRGB_sng colour = new sRGB_sng();

            if( !IOUtil.InvariantParse( splitText.Parts[0], ref colour.Red ) )
            {
                return false;
            }
            if( !IOUtil.InvariantParse( splitText.Parts[1], ref colour.Green ) )
            {
                return false;
            }
            if( !IOUtil.InvariantParse( splitText.Parts[2], ref colour.Blue ) )
            {
                return false;
            }

            Red = colour.Red;
            Green = colour.Green;
            Blue = colour.Blue;

            return true;
        }
    }

    public class clsRGBA_sng : clsRGB_sng
    {
        public float Alpha;

        public clsRGBA_sng( float red, float green, float blue, float alpha )
            : base( red, green, blue )
        {
            this.Alpha = alpha;
        }

        public clsRGBA_sng( clsRGBA_sng copyItem )
            : base( copyItem.Red, copyItem.Green, copyItem.Blue )
        {
            Alpha = copyItem.Alpha;
        }

        public override string GetINIOutput()
        {
            return base.GetINIOutput() + ", " + Alpha.ToStringInvariant();
        }

        public override bool ReadINIText( SplitCommaText splitText )
        {
            if( !base.ReadINIText( splitText ) )
            {
                return false;
            }

            if( splitText.PartCount < 4 )
            {
                return false;
            }

            if( !IOUtil.InvariantParse( splitText.Parts[3], ref Alpha ) )
            {
                Alpha = 1.0F;
            }

            return true;
        }
    }
}