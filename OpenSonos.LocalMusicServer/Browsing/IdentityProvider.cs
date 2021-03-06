using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using OpenSonos.LocalMusicServer.Bootstrapping;

namespace OpenSonos.LocalMusicServer.Browsing
{
    public class IdentityProvider : IIdentityProvider
    {
	    private readonly ConcurrentDictionary<string, SonosIdentifier> _hashCache;
        private readonly ConvertPathsToSha1 _idGen;

        public IdentityProvider(ServerConfiguration config)
        {
			_hashCache = new ConcurrentDictionary<string, SonosIdentifier>();
            _idGen = new ConvertPathsToSha1();

            _hashCache.TryAdd("root", new SonosIdentifier
            {
                Id = "root",
                Path = config.MusicShare
            });
        }

	    public IdentityProvider(ServerConfiguration config, IEnumerable<KeyValuePair<string, SonosIdentifier>> backingStore)
			: this(config)
        {
	        if (backingStore == null)
            {
                return;
            }

            foreach (var item in backingStore)
            {
                _hashCache.TryAdd(item.Key, item.Value);
            }
        }

        public SonosIdentifier IdFor(string path)
        {
            if (_hashCache.ContainsKey(path))
            {
                return _hashCache[path];
            }

            var identifier = new SonosIdentifier
            {
                Id = _idGen.IdentifierFor(path),
                Path = path
            };

            _hashCache.TryAdd(path, identifier);
            return identifier;
        }

        public SonosIdentifier FromRequestId(string requestedId)
        {
            return _hashCache.SingleOrDefault(x => x.Value.Id == requestedId).Value;
        }
    }
}