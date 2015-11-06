﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ApprovalTests.Reporters;
using HeterogeneousDataSources;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;

namespace HeterogeneousDataSource.Conventions.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkProtocolBuilderExtensionsTests {
        [Test]
        public void GetLinkedSourceTypes(){
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            var conventionStub = new ConventionStub();
            
            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSourceWithImage), typeof(LinkedSourceWithPerson) },
                new List<ILoadLinkExpressionConvention> { conventionStub }
            );

            Assert.That(
                conventionStub.LinkTargetPropertyNamesWhereConventionApplies,
                Is.EquivalentTo(new[] { "Image", "Person" })
            );
        }

        public class LinkedSourceWithImage : ILinkedSource<Model>{
            public Model Model { get; set; }
            public Image NotImage { get; set; }
            public Image Image { get; set; }
        }

        public class LinkedSourceWithPerson : ILinkedSource<Model> {
            public Model Model { get; set; }
            public Person Person { get; set; }
            public Person NotPerson { get; set; }
        }

        public class Model{
            public string Id { get; set; }
            public string ImageId { get; set; }
            public string PersonId { get; set; }
        }

        public class ConventionStub:ISingleValueConvention
        {
            public readonly List<string> LinkTargetPropertyNamesWhereConventionApplies = new List<string>();

            public string Id { get { return "Stub"; } }
            public bool DoesApply(
                PropertyInfo linkTargetProperty, 
                PropertyInfo linkedSourceModelProperty)
            {
                var matchingName = linkTargetProperty.Name + "Id";
                return matchingName == linkedSourceModelProperty.Name;
            }

            public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
                LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder, Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty,
                Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty, PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty)
            {
                LinkTargetPropertyNamesWhereConventionApplies.Add(linkTargetProperty.Name);
            }
        }
    }
}