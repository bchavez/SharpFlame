using SharpFlame.Core;

namespace SharpFlame.Mapping.IO
{
    public interface IIOLoader
    {
        Result Load(string path);
    }

    public interface IIOSaver
    {
        Result Save(string path, bool overwrite, bool compress);
    }
}