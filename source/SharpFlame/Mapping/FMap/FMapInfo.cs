#region

using System;
using SharpFlame.Core.Domain;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;
using SharpFlame.Mapping.Tiles;

#endregion

namespace SharpFlame.Mapping.FMap
{
    public class FMapInfo : Translator
    {
        public clsInterfaceOptions InterfaceOptions = new clsInterfaceOptions();
        public XYInt TerrainSize = new XYInt(-1, -1);
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
            else if ( INIProperty.Name == "size" )
            {
                var CommaText = INIProperty.Value.Split(',');
                if ( CommaText.GetUpperBound(0) + 1 < 2 )
                {
                    return TranslatorResult.ValueInvalid;
                }
                var A = 0;
                for ( A = 0; A <= CommaText.GetUpperBound(0); A++ )
                {
                    CommaText[A] = CommaText[A].Trim();
                }
                var NewSize = new XYInt();
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
            else if ( INIProperty.Name == "autoscrolllimits" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref InterfaceOptions.AutoScrollLimits) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( INIProperty.Name == "scrollminx" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref InterfaceOptions.ScrollMin.X) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( INIProperty.Name == "scrollminy" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref InterfaceOptions.ScrollMin.Y) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( INIProperty.Name == "scrollmaxx" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref InterfaceOptions.ScrollMax.X) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( INIProperty.Name == "scrollmaxy" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref InterfaceOptions.ScrollMax.Y) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( INIProperty.Name == "name" )
            {
                InterfaceOptions.CompileName = Convert.ToString(INIProperty.Value);
            }
            else if ( INIProperty.Name == "players" )
            {
                InterfaceOptions.CompileMultiPlayers = Convert.ToString(INIProperty.Value);
            }
            else if ( INIProperty.Name == "xplayerlev" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref InterfaceOptions.CompileMultiXPlayers) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( INIProperty.Name == "author" )
            {
                InterfaceOptions.CompileMultiAuthor = Convert.ToString(INIProperty.Value);
            }
            else if ( INIProperty.Name == "license" )
            {
                InterfaceOptions.CompileMultiLicense = Convert.ToString(INIProperty.Value);
            }
            else if ( INIProperty.Name == "camptime" )
            {
                //allow and ignore
            }
            else if ( INIProperty.Name == "camptype" )
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