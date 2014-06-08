using SharpFlame.Core;
using SharpFlame.Mapping;

namespace SharpFlame.Mapping.IO
{
    public interface IIOLoader
    {
        GenericResult<Map> Load(string path, Map map = null);
    }

    public interface IIOSaver
    {
        Result Save(string path, Map map, bool overwrite, bool compress);
    }
}