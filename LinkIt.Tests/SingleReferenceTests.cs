﻿using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.LinkedSources.Interfaces;
using LinkIt.Protocols;
using LinkIt.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class SingleReferenceTests
    {
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<SingleReferenceLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Test]
        public void LoadLink_SingleReference()
        {
            var actual = _sut.LoadLink<SingleReferenceLinkedSource>().FromModel(
                new SingleReferenceContent {
                    Id = "1",
                    SummaryImageId = "a"
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_SingleReferenceWithoutReferenceId_ShouldLinkNull() {
            var actual = _sut.LoadLink<SingleReferenceLinkedSource>().FromModel(
                new SingleReferenceContent {
                    Id = "1",
                    SummaryImageId = null
                }
            );

            Assert.That(actual.SummaryImage, Is.Null);
        }

        [Test]
        public void LoadLink_SingleReferenceCannotBeResolved_ShouldLinkNull() {
            var actual = _sut.LoadLink<SingleReferenceLinkedSource>().FromModel(
                new SingleReferenceContent {
                    Id = "1",
                    SummaryImageId = "cannot-be-resolved"
                }
            );

            Assert.That(actual.SummaryImage, Is.Null);
        }
    }

    public class SingleReferenceLinkedSource: ILinkedSource<SingleReferenceContent>
    {
        public SingleReferenceContent Model { get; set; }
        public Image SummaryImage{ get; set; }
    }

    public class SingleReferenceContent {
        public string Id { get; set; }
        public string SummaryImageId { get; set; }
    }
}