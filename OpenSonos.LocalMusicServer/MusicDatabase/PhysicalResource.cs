using System.Linq;
using OpenSonos.LocalMusicServer.Browsing;

namespace OpenSonos.LocalMusicServer.MusicDatabase
{
    public abstract class PhysicalResource : IRepresentAResource
    {
        public SonosIdentifier Identifier { get; private set; }
        public string DisplayName { get { return Parts.Last(); } }
        public string[] Parts { get { return Identifier.Path.Split('\\'); } }

        protected PhysicalResource(SonosIdentifier identifier)
        {
            Identifier = identifier;
        }

        public static PhysicalResource FromId(SonosIdentifier identifier)
        {
            return identifier.IsDirectory 
                ? (PhysicalResource) new Container(identifier) 
                : new MusicFile(identifier);
        }
    }
}