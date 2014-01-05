using System;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.FMap
{
    public class FMapInfo : Translator
    {
        public sXY_int TerrainSize = new sXY_int(-1, -1);
        public clsMap.clsInterfaceOptions InterfaceOptions = new clsMap.clsInterfaceOptions();
        public clsTileset Tileset;

        public override TranslatorResult Translate(Section.SectionProperty INIProperty)
        {
            if ( INIProperty.Name == "tileset" )
            {
                if ( INIProperty.Value.ToLower() == "arizona" )
                {
                    Tileset = App.Tileset_Arizona;
                }
                else if ( INIProperty.Value.ToLower() == "urban" )
                {
                    Tileset = App.Tileset_Urban;
                }
                else if ( INIProperty.Value.ToLower() == "rockies" )
                {
                    Tileset = App.Tileset_Rockies;
                }
                else
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "size" )
            {
                string[] CommaText = INIProperty.Value.Split(',');
                if ( CommaText.GetUpperBound(0) + 1 < 2 )
                {
                    return TranslatorResult.ValueInvalid;
                }
                int A = 0;
                for ( A = 0; A <= CommaText.GetUpperBound(0); A++ )
                {
                    CommaText[A] = CommaText[A].Trim();
                }
                sXY_int NewSize = new sXY_int();
                if ( !IOUtil.InvariantParse(CommaText[0], ref NewSize.X) )
                {
                    return TranslatorResult.ValueInvalid;
                }
                if ( !IOUtil.InvariantParse(CommaText[1], ref NewSize.Y) )
                {
                    return TranslatorResult.ValueInvalid;
                }
                if ( NewSize.X < 1 | NewSize.Y < 1 | NewSize.X > Constants.MapMaxSize | NewSize.Y > Constants.MapMaxSize )
                {
                    return TranslatorResult.ValueInvalid;
                }
                TerrainSize = NewSize;
            }
            else if ( (string)INIProperty.Name == "autoscrolllimits" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref InterfaceOptions.AutoScrollLimits) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "scrollminx" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref InterfaceOptions.ScrollMin.X) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "scrollminy" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref InterfaceOptions.ScrollMin.Y) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "scrollmaxx" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), InterfaceOptions.ScrollMax.X) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "scrollmaxy" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), InterfaceOptions.ScrollMax.Y) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "name" )
            {
                InterfaceOptions.CompileName = Convert.ToString(INIProperty.Value);
            }
            else if ( (string)INIProperty.Name == "players" )
            {
                InterfaceOptions.CompileMultiPlayers = Convert.ToString(INIProperty.Value);
            }
            else if ( (string)INIProperty.Name == "xplayerlev" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref InterfaceOptions.CompileMultiXPlayers) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "author" )
            {
                InterfaceOptions.CompileMultiAuthor = Convert.ToString(INIProperty.Value);
            }
            else if ( (string)INIProperty.Name == "license" )
            {
                InterfaceOptions.CompileMultiLicense = Convert.ToString(INIProperty.Value);
            }
            else if ( (string)INIProperty.Name == "camptime" )
            {
                //allow and ignore
            }
            else if ( (string)INIProperty.Name == "camptype" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref InterfaceOptions.CampaignGameType) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else
            {
                return TranslatorResult.NameUnknown;
            }
            return TranslatorResult.Translated;
        }
    }
}