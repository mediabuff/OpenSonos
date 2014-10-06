﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using OpenSonos.LocalMusicServer.Browsing;

namespace OpenSonos.LocalMusicServer.Test.Unit.Browsing
{
    [TestFixture]
    public class GuidIdentityProviderTests
    {
        private string _uncompressedId;
        private string _compressedId;
        private IIdentityProvider _provider;

        [SetUp]
        public void SetUp()
        {
            _uncompressedId = "\\\\some\\smb\\path";
            _compressedId = Guid.NewGuid().ToString();

            var backing = new Dictionary<string, SonosIdentifier>
            {
                {_uncompressedId, new SonosIdentifier {Id = _compressedId, Path = "\\\\some\\smb\\path"}}
            };

            _provider = new GuidIdentityProvider(backing);
        }

        [Test]
        public void FromPath_WithPath_IdAndPathSetCorrectly()
        {
            var identifier = _provider.FromPath(_uncompressedId);

            Assert.That(identifier.Id, Is.EqualTo(_compressedId));
        }

        [Test]
        public void FromPath_WithMaxWindowsPathLength_CanCreateCompressedId()
        {
            var identifier = _provider.FromPath("\\abcdefghjklmnopqrstuvwxyz\\ABCDEFGHIJKLMNOPQRSTUVWXYZ\\AbCdEfGhIjLlMnoPqRsTuVwZyZ\\the quick brown fox jumped ov\\er the lazy dog and this i\\s strin  a long string th\\at should be very hard toc\\ompress reliably for gzip \\compression to handle well and q");

            Assert.That(identifier.Id, Is.Not.Null);
        }

        [Test]
        public void FromPath_WhenPathHasLotsOfEntropy_CanCreateCompressedId()
        {
            var identifier = _provider.FromPath("Justin Timberlake - The 20-20 Experience (Deluxe Edition) 2013 Pop 320kbps CBR MP3 [VX]");

            Assert.That(identifier.Id, Is.Not.Null);
        }

        [Test]
        public void FromPath_WhenPathIsProvidedTwice_SameIdReturned()
        {
            var identifier1 = _provider.FromPath("\\something\\here");
            var identifier2 = _provider.FromPath("\\something\\here");

            Assert.That(identifier1.Id, Is.EqualTo(identifier2.Id));
            Assert.That(identifier1.Path, Is.EqualTo(identifier2.Path));
        }

        [Test]
        public void FromRequestId_WithValidCompressedId_PathIsCorrect()
        {
            var identifier = _provider.FromRequestId(_compressedId);

            Assert.That(identifier.Path, Is.EqualTo(_uncompressedId));
        }

        [Test]
        public void FromRequestId_WithNullEmptyId_PathAndIdAreEmpty()
        {
            var identifier = _provider.FromRequestId("");

            Assert.That(identifier.Path, Is.EqualTo(""));
            Assert.That(identifier.Id, Is.EqualTo(""));
        }

        [Test]
        public void FromRequestId_WithRootSonosValueProvided_PathAndIdAreEmpty()
        {
            var identifier = _provider.FromRequestId("root");

            Assert.That(identifier.Path, Is.EqualTo(""));
            Assert.That(identifier.Id, Is.EqualTo(""));
        }

        [Test]
        public void FromRequestId_WithDirectoryPath_IsDirectoryIsTrue()
        {
            var identifier = _provider.FromRequestId(_compressedId);

            Assert.That(identifier.IsDirectory, Is.EqualTo(true));
        }
    }
}