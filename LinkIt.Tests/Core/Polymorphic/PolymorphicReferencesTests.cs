﻿using System.Collections.Generic;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests.Core.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class PolymorphicReferencesTests {
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.For<LinkedSource>()
                .PolymorphicLoadLinkForList(
                    linkedSource => linkedSource.Model.Target,
                    linkedSource => linkedSource.Target,
                    link => link.Type,
                    includes => includes
                        .Include<Image>().AsReferenceById(
                            "image",
                            link=>link.Id
                        )
                        .Include<Person>().AsReferenceById(
                            "person",
                            link => link.Id
                        )
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Test]
        public void LoadLink_PolymorphicReferenceWithImage() {
            var actual = _sut.LoadLink<LinkedSource>().FromModel(
                new Model {
                    Id = "1",
                    Target = new List<PolymorphicReference>
                    {
                        new PolymorphicReference {
                            Type = "person",
                            Id = "a"
                        },
                        new PolymorphicReference{
                            Type = "image",
                            Id = "a"
                        }
                    }
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public List<object> Target { get; set; }
        }

        public class Model{
            public string Id { get; set; }
            public List<PolymorphicReference> Target { get; set; }
        }

        //stle: should be shared
        public class PolymorphicReference {
            public string Type { get; set; }
            public string Id { get; set; }
        }
    }
}