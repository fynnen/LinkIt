﻿#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests.Core {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class OptionalReferenceTests
    {
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.MediaId,
                    linkedSource => linkedSource.Media
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Test]
        public void LoadLink_WithValue_ShouldLinkMedia(){
            var actual = _sut.LoadLink<LinkedSource>().FromModel(
                new Model {
                    Id = "1",
                    MediaId = 32
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_WithoutValue_ShouldLinkNull() {
            var actual = _sut.LoadLink<LinkedSource>().FromModel(
                new Model {
                    Id = "1",
                    MediaId = null
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public Media Media { get; set; }
        }

        public class Model {
            public string Id { get; set; }
            public int? MediaId { get; set; }
        }

    }
}
