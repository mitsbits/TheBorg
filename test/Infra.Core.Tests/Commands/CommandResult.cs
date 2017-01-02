using Borg.Infra.CQRS;
using Moq;
using Shouldly;
using System;
using Xunit;

namespace Infra.CQRS.Tests.Commands
{
    public class CommandResultTests
    {
        private Mock<IEntity<string>> _entity;
        private Guid _entityKey;
        private Mock<Exception> _exception;
        private string _exceptionText = "mock exception";
        private string _descriptionText = "some text";

        public CommandResultTests()
        {
            _entityKey = Guid.NewGuid();
            _entity = new Mock<IEntity<string>>();
            _entity.Setup(x => x.Id).Returns(_entityKey.ToString);
            _exception = new Mock<Exception>();
            _exception.Setup(x => x.ToString()).Returns(_exceptionText);
        }

        [Fact]
        public void check_that_static_factory_methods_return()
        {
            var res = CommandResult.Create(true);
            res.Success.ShouldBe(true);
            res.Description.ShouldBe(string.Empty);
            res = CommandResult.Create(false);
            res.Success.ShouldBe(false);
            res.Description.ShouldBe(string.Empty);
            res = CommandResult.Create(true, _descriptionText);
            res.Success.ShouldBe(true);
            res.Description.ShouldBe(_descriptionText);
            res = CommandResult.Create(false, _descriptionText);
            res.Success.ShouldBe(false);
            res.Description.ShouldBe(_descriptionText);
        }

        [Fact]
        public void check_that_static_factory_methods_return_with_entities()
        {
            var res = CommandResult<IEntity<string>>.Create(true, _entity.Object);
            res.Success.ShouldBe(true);
            res.Description.ShouldBe(string.Empty);
            var typedRes = res as ICommandResult<IEntity<string>>;
            typedRes.ShouldNotBeNull();
            typedRes.Entity.ShouldNotBeNull();
            typedRes.Entity.Id.ShouldBe(_entityKey.ToString());

            res = CommandResult<IEntity<string>>.Create(false, _entity.Object);
            res.Success.ShouldBe(false);
            res.Description.ShouldBe(string.Empty);
            typedRes = res as ICommandResult<IEntity<string>>;
            typedRes.ShouldNotBeNull();
            typedRes.Entity.ShouldNotBeNull();
            typedRes.Entity.Id.ShouldBe(_entityKey.ToString());
        }

        [Fact]
        public void check_that_static_factory_methods_returns_from_exception()
        {
            var res = CommandResult.Create(_exception.Object);
            res.Success.ShouldBe(false);
            res.Description.ShouldBe(_exceptionText);
        }
    }
}