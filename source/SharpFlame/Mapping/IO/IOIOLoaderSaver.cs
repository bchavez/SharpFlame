using System;

namespace SharpFlame.Mapping.IO
{
    interface IIOLoader {
        clsResult Load(string path);
    }

    interface IIOSaver {
        clsResult Save(string path, bool overwrite, bool compress);
    }
}

